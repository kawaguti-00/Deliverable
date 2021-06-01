using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{
	public bool isActive = false; 

	//設定欄
	[SerializeField]							//最大HP
	protected int maxHP = 1;
	[SerializeField]							//移動速度
	protected float defaultSpeed = 0;
	[SerializeField]							//攻撃力
	protected int defaultPower = 0;
	[SerializeField]							//ジャンプ力
	protected float jumpPower = 0;
	[SerializeField]							//着地判定用オブジェクト設定
	protected GameObject[] groundCheckObjects = new GameObject[3];

	//変数宣言
	protected int hp = 0;						//HP
	protected float speed = 0;					//速度
	protected int power = 0;					//
	protected GameObject gameManagerObj;
	protected GameManager gameManager;
	protected bool isGrounded = false;			//着地判定
	protected bool isGroundedPrev = false;		//着地
	protected float direction = 1;				//プレイヤーの向き

	//HP処理
	public int Hp
	{
		set
		{
			hp = Mathf.Clamp(value, 0, maxHP);

			if(hp <= 0)
			{
				Dead();
			}
		}
		get
		{
			return hp;
		}
	}

	//アニメ変更用速度処理
	public float Speed
	{
		set
		{
			speed = value;
		}
		get
		{
			return speed;
		}
	}

	public int Power
	{
		set
		{
			power = Mathf.Max(value, 0);
		}
		get
		{
			return power;
		}
	}


    protected virtual void Start()
    {
		gameManagerObj = GameObject.FindGameObjectWithTag("GameController");
		gameManager = gameManagerObj.GetComponent<GameManager>();

		InitCharacter();
    }

    protected virtual void Update()
    {
        
    }

	protected virtual void FixedUpdate()
	{
		FixedUpdateCharacter();
	}

	protected virtual void FixedUpdateCharacter()
	{

	}

	protected virtual void InitCharacter()
	{
		Hp = maxHP;
		Speed = defaultSpeed;
		direction = transform.localScale.x;

		//isActive = true;
	}

	protected virtual void Move()
	{

	}

	protected virtual void Damage()
	{

	}

	protected virtual void Dead()
	{

	}

	protected virtual void UpdateAnimation()
	{

	}

	//着地判定
	protected void GroundCheck()
	{
		isGroundedPrev = isGrounded;
		Collider2D[] groundCheckCollider = new Collider2D[groundCheckObjects.Length];
		//着地してるか判定
		for (int i = 0; i < groundCheckObjects.Length; i++)
		{
			groundCheckCollider[i] = Physics2D.OverlapPoint(groundCheckObjects[i].transform.position);
			//接地判定オブジェクトのうち、1つでも何かに重なっていたら接地しているものとして終了
			if (groundCheckCollider[i] != null)
			{
				isGrounded = true;
				return;
			}
		}
		//接地していないと判断
		isGrounded = false;
	}

}
