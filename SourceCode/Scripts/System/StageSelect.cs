using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{

    [SerializeField]
    GameObject buttonPrefab = null;

    GameObject gameManagerObj;
    MoveSceneManager moveSceneManager;
    GameObject buttonObj;
    Button button;
    Text text;

    void Start()
    {
        gameManagerObj = GameObject.FindGameObjectWithTag("GameController");
        moveSceneManager = gameManagerObj.GetComponent<MoveSceneManager>();

        for (int i = 1; i < moveSceneManager.NumOfScene; i++)
        {
            buttonObj = Instantiate(buttonPrefab, transform.position, Quaternion.identity, transform);
            buttonObj.transform.SetParent(gameObject.transform);
            button = buttonObj.GetComponent<Button>();
            text = buttonObj.GetComponentInChildren<Text>();

            //イベントリスナーに引数を設定するために一工夫
            //ループのインデックスを引数として渡すと、変数そのものが渡されるので全部ループ終了後のインデックス値が設定されてしまう
            //そのため、一時変数を用意してそれを引数に渡すようにする
            int i_temp = i;
            button.onClick.AddListener(() => moveSceneManager.MoveToScene(i_temp));

            text.text = "ステージ" + i.ToString();
        }
    }

}
