using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : BaseCharacterController
{
    //設定欄
    [SerializeField]
    string horizontalInputName = "Horizontal";
    [SerializeField]
    string jumpButtonName = "Jump";
    //効果音設定
    [SerializeField]    //ダメージボイス
    AudioClip damageVoice = null;
    [SerializeField]    //ジャンプボイス
    AudioClip JumpVoice = null;
    [SerializeField]    //KOボイス
    AudioClip KoVoice = null;

    [SerializeField]    //ジャンプ音
    AudioClip JumpSE = null;
    [SerializeField]    //ダメージ音
    AudioClip DamageSE = null;
    [SerializeField]    //敵死亡音,一旦こちらで鳴らす
    AudioClip KDDeathSE = null;

    //変数宣言
    bool jump = false;
    bool damage = false;
    float inputH = 0;
    float velY = 0;
    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidBody2D;
    SoundManager soundManager;

    private GameObject LP;      //体力ゲージのオブジェクトを入れておくための場所
    LifePanel LPscript;         //体力ゲージのスクリプトのクラス

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        soundManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SoundManager>();

        //体力ゲージを変数に格納
        LP = GameObject.Find ("LifePanel");
        //スクリプトLifePanelを格納
        LPscript = LP.GetComponent<LifePanel>(); 
    }

    protected override void Update()
    {
        velY = rigidBody2D.velocity.y;
        GetInput();
    }

    protected override void FixedUpdate()
    {
        Move();
    }

    //プレイヤーの入力を受け付ける
    void GetInput()
    {
        if (!isActive)
        {
            return;
        }

        inputH = Input.GetAxisRaw(horizontalInputName);
        jump = Input.GetButtonDown(jumpButtonName);
    }

    //移動処理
    protected override void Move()
    {
        if (!isActive)
        {
            return;
        }

        GroundCheck();          //接地判定

        if (inputH != 0 && !damage)        //移動速度の計算処理
        {
            direction = Mathf.Sign(inputH);
            speed = defaultSpeed * direction;

            spriteRenderer.flipX = direction < 0 ? true : false;         //入力がマイナスならスプライトの向きを反転させる
        }
        else
        {
            speed = 0;
        }

        
        if (jump && isGrounded)         //ジャンプの速度計算
        {
            if (JumpSE != null)         //ジャンプ音
            {
                soundManager.PlaySeByName(JumpSE.name);
            }
            if (JumpVoice != null)      //ジャンプボイス
            {
                soundManager.PlaySeByName(JumpVoice.name);
            }

            rigidBody2D.velocity = Vector3.up * jumpPower;
        }

        
        UpdateAnimation();  //アニメーションを更新
        rigidBody2D.velocity = new Vector2(speed, rigidBody2D.velocity.y); //移動処理
    }

    //ダメージ処理
    protected override void Damage()
    {
        //rigidBody2D.velocity = new Vector2(-1 * direction, 4);
        if (damage) //既にダメージ状態（＝無敵時間中）なら終了
        {
            return;
        }

        if (damageVoice != null)        //ダメージSE
        {
            soundManager.PlaySeByName(DamageSE.name);
        }

        Hp--;
        //体力ゲージを減らす
        LPscript.Reduce();
        if (Hp > 0)
        {   
            
            if (damageVoice != null)        //ダメージボイス
            {
                soundManager.PlaySeByName(damageVoice.name);
            }

            StartCoroutine("DamageTimer");
        }
        else
        {
            Dead();
        }
    }

    //ダメージを受けた瞬間の無敵時間のタイマー
    protected IEnumerator DamageTimer()
    {
        //既にダメージ状態なら終了
        if (damage)
        {
            yield break;
        }
        damage = true;
        animator.SetTrigger("Damage");
        //無敵時間中の点滅
        for (int i = 0; i < 10; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
        damage = false;
    }


    //接触判定系
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Coin")
        {
            gameManager.CorrectedCoins++;   //集めたコインの数を1つ増やす
            return;
        }


        if (collision.gameObject.tag == "GameOverZone" && Hp > 0)
        {
            Hp = 0;
            Dead();
        }
    }

    //敵に触れた時
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        //倒せない敵と接触した時
        if (collision.gameObject.CompareTag("Enemy") && Hp > 0)
        {
            Damage();
        }

        //倒せる飛ぶ敵と接触した時
        if (collision.gameObject.CompareTag("KDEnemy") && Hp > 0)
        {
            //KnockDownEnemyを参照
            KnockDownEnemy EnemyJ = collision.gameObject.GetComponent<KnockDownEnemy>();
            //踏んだ時
            if (this.transform.position.y > EnemyJ.transform.position.y)
            {
                if (KDDeathSE != null)        //撃破SE
                {
                    soundManager.PlaySeByName(KDDeathSE.name);
                }

                rigidBody2D.velocity = Vector3.up * 6;
                EnemyJ.DestroyEnemy();
            }
            else
            {
                Damage();
            }
        }

        //倒せる歩く敵と接触した時
        if (collision.gameObject.CompareTag("KDEnemy2") && Hp > 0)
        {
            //KnockDownEnemyを参照
            WalkEnemy EnemyJ2 = collision.gameObject.GetComponent<WalkEnemy>();
            //踏んだ時
            if (this.transform.position.y > EnemyJ2.transform.position.y + 1)
            {
                if (KDDeathSE != null)        //撃破SE
                {
                    soundManager.PlaySeByName(KDDeathSE.name);
                }

                rigidBody2D.velocity = Vector3.up * 6;
                EnemyJ2.DestroyEnemy();
            }
            else
            {
                Damage();
            }
        }
    }

    

    //死亡時
    protected override void Dead()
    {
        isActive = false;
        animator.SetBool("Dead", true);

        if (KoVoice != null)        //KOボイス
        {
            soundManager.PlaySeByName(KoVoice.name);
        }

        rigidBody2D.velocity = Physics2D.gravity / rigidBody2D.gravityScale;
        gameManager.GameOver();
    }

    //アニメ変更
    protected override void UpdateAnimation()
    {
        if (gameManager.isClear && isGrounded)  //ステージクリア時のアニメ変更処理
        {
            speed = 0;
            rigidBody2D.velocity = new Vector2(0, rigidBody2D.velocity.y);
            isActive = false;
            animator.SetBool("Clear", gameManager.isClear);
        }
        animator.SetFloat("Speed", Mathf.Abs(speed));
        animator.SetBool("Jump", !isGrounded);
        
    }

}