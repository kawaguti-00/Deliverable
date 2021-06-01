using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MoveSceneManager))]
[RequireComponent(typeof(SaveManager))]
[RequireComponent(typeof(SoundManager))]
[DefaultExecutionOrder(-5)]
public class GameManager : SingletonMonoBehaviour<GameManager>
{

    [Header("シーンロード時に自動生成するプレハブを登録")]
    [SerializeField]
    GameObject[] prefabs = null;

    [Header("UIの設定")]
    [SerializeField]
    GameObject clearCanvasPrefab = null;
    [SerializeField]
    GameObject gameOverCanvasPrefab = null;
    [SerializeField]                    //「次のステージへ進む」ボタンを参照
    string nextButtonName = "NextButton";
    [SerializeField]                    //「リトライ」ボタンを参照
    string retryButtonName = "RetryButton";
    [SerializeField]                    //「タイトルに戻る」ボタンを参照
    string titleButtonName = "TitleButton";
    [SerializeField]                    //コイン取得数を表示させるtextの名前を指定
    string coinTextName = "coinTextName";

    MoveSceneManager moveSceneManager;
    SaveManager saveManager;
    SoundManager soundManager;

    TimeManager timeManager;
    Text coinText;

    //変数宣言
    public bool isClear = false;        //ステージクリアフラグ
    bool isGameOver = false;            //ゲームオーバーフラグ
    int numOfCoins = 0;                 //ステージクリアに必要なコイン数
    int correctedCoins = 0;             //取得したコイン数


    [Header("サウンドの設定")]
    [SerializeField]                    //BGM
    AudioClip bgm = null;
    [SerializeField]                    //ステージ1BGM
    AudioClip bgm1 = null;
    [SerializeField]                    //ステージ2BGM
    AudioClip bgm2 = null;
    [SerializeField]                    //ステージ3BGM
    AudioClip bgm3 = null;
    [SerializeField]                    //ステージクリア
    AudioClip clearSe = null;
    [SerializeField]                    //ゲームオーバー
    AudioClip GameOverBGM = null;

    //コイン取得数計算
    public int CorrectedCoins
    {
        set
        {
            correctedCoins = value;
            UpdateCoinText();

            if (correctedCoins >= numOfCoins)
            {
                Clear();
            }
        }
        get
        {
            return correctedCoins;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        if (Debug.isDebugBuild)
        {

        }
        moveSceneManager = GetComponent<MoveSceneManager>();
        saveManager = GetComponent<SaveManager>();
        soundManager = GetComponent<SoundManager>();

    }
    void Start()
    {
        if (Debug.isDebugBuild)
        {
            InstantiateWhenLoadScene();
            InitGame(); 
        }

        soundManager.StopBgm();
        if (bgm != null && moveSceneManager.SceneName == "Title")
        {
            soundManager.PlayBgmByName(bgm.name);
        }
    }
    void Update()
    {

    }

    //シーン読み込み
    public void InstantiateWhenLoadScene()
    {
        if (moveSceneManager.SceneName == "Title")
        {
            return;
        }
        foreach (GameObject prefab in prefabs)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }
    
    //ゲーム初期化
    public void InitGame()
    {
        isClear = false;
        isGameOver = false;
        numOfCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
        correctedCoins = 0;

        soundManager.StopBgm();
        if (bgm != null && moveSceneManager.SceneName == "Title")
        {
            soundManager.PlayBgmByName(bgm.name);
        }
        if (bgm1 != null && moveSceneManager.SceneName == "Stage1")
        {
            soundManager.PlayBgmByName(bgm1.name);
        }
        if (bgm2 != null && moveSceneManager.SceneName == "Stage2")
        {
            soundManager.PlayBgmByName(bgm2.name);
        }
        if (bgm3 != null && moveSceneManager.SceneName == "Stage3")
        {
            soundManager.PlayBgmByName(bgm3.name);
        }
        if (moveSceneManager.SceneName != "Title")
        {
            timeManager = GameObject.FindGameObjectWithTag("TimeManager").GetComponent<TimeManager>();
            coinText = GameObject.Find(coinTextName).GetComponent<Text>();
            UpdateCoinText();
        }
    }

    //ゲームクリア
    public void Clear()
    {
        if (isClear || isGameOver)
        {
            return;
        }

        if (clearSe != null)
        {
            soundManager.PlaySeByName(clearSe.name);
        }

        isClear = true;

        //プレイヤーを操作不能に変更
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().isActive = false;

        //クリア画面のキャンバスを生成
        Instantiate(clearCanvasPrefab, transform.position, Quaternion.identity);

        //ボタンのコンポーネントを取得
        Button nextButton = GameObject.Find(nextButtonName).GetComponent<Button>();
        Button titleButton = GameObject.Find(titleButtonName).GetComponent<Button>();

        //ボタンに、クリックしたときの処理を登録
        //ただし「次のステージ」ボタンは次のステージがないときは押せないようにする
        if (moveSceneManager.CurrentSceneNum < moveSceneManager.NumOfScene - 1)
        {
            nextButton.onClick.AddListener(() => moveSceneManager.MoveToScene(moveSceneManager.CurrentSceneNum + 1));
        }
        else
        {
            nextButton.interactable = false;
        }
        titleButton.onClick.AddListener(() => moveSceneManager.MoveToScene(0)); //タイトル画面に戻るので、シーン番号は0番
    }

    public void GameOver()
    {
        if (isGameOver || isClear)
        {
            return;
        }
        if (GameOverBGM != null)
        {
            //soundManager.PlaySeByName(GameOverBGM.name);
        }

        isGameOver = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().isActive = false;

        //ゲームオーバー画面のキャンバスを生成
        Instantiate(gameOverCanvasPrefab, transform.position, Quaternion.identity);

        //ボタンのコンポーネントを取得
        Button retryButton = GameObject.Find(retryButtonName).GetComponent<Button>();
        Button titleButton = GameObject.Find(titleButtonName).GetComponent<Button>();

        //ボタンに、クリックしたときの処理を登録
        retryButton.onClick.AddListener(() => moveSceneManager.MoveToScene(moveSceneManager.CurrentSceneNum));  //リトライなので、今と同じシーンを再読み込み
        titleButton.onClick.AddListener(() => moveSceneManager.MoveToScene(0));  //タイトル画面に戻るので、シーン番号は0番
    }

    void UpdateCoinText()
    {
        coinText.text = "残り" + (numOfCoins - correctedCoins).ToString() + "枚";
    }
}