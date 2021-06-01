using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{

    [System.NonSerialized]
    public bool isActive = false;

    //設定欄
    [SerializeField]    //制限時間-分
    int minutes = 1;
    [SerializeField]    //制限時間-病
    float seconds = 0;
    [SerializeField]    //timerTextの名前を指定
    string timerTextName = "TimerText";
    [SerializeField]    //CountDownTextの名前を指定
    string countDownTextName = "CountDownText";

    //変数宣言
    Text timerText;
    Text countDownText;
    float currentTime;
    float oldSeconds;
    float totalTime;
    GameManager gameManager;

    void Start()
    {
        timerText = GameObject.Find(timerTextName).GetComponent<Text>();
        countDownText = GameObject.Find(countDownTextName).GetComponent<Text>();
        countDownText.enabled = false;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        InitTime();
        StartCoroutine("CountDown");
    }

    void Update()
    {
        Timer();
    }

    //制限時間計算
    void InitTime()     
    {
        oldSeconds = 0f;
        totalTime = minutes * 60 + seconds;
        currentTime = totalTime;

        SetTimeText();
    }

    //タイマー処理
    void Timer()
    {
        if(!isActive || currentTime <= 0f)
        {
            return;
        }

        currentTime = minutes * 60 + seconds;
        currentTime -= Time.deltaTime;

        minutes = (int)currentTime / 60;
        seconds = currentTime - minutes * 60;

        if((int)seconds != (int)oldSeconds)
        {
            SetTimeText();
        }

        oldSeconds = seconds;

        if(currentTime <= 0f)
        {
            isActive = false;

            //ここに制限時間終了時の処理
            gameManager.GameOver();
        }
    }

    //ゲーム開始前のカウントダウン処理
    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(2);     //1秒待機

        countDownText.enabled = true;
        countDownText.text = "READY";               //3を表示
        yield return new WaitForSeconds(2);     //1秒待機

        countDownText.text = "GO!!";            //GO!を表示
        isActive = true;                        //タイマー開始
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().isActive = true;  //プレイヤーを操作可能状態に変更
        yield return new WaitForSeconds(1);     //1秒待機

        countDownText.enabled = false;
    }

    //画面上に制限時間を表示処理
    void SetTimeText()                          
    {
        timerText.text = "TIME\n" + minutes.ToString("D2") + ":" + ((int)seconds).ToString("D2");
    }

}