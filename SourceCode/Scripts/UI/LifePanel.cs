using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePanel : MonoBehaviour
{
    //体力ゲージプレハブ
    [SerializeField]
    private GameObject LifeObj;

    int Life = 3;

    //体力ゲージHP分作成
    public void Start()
    {
        //体力を表示
        for ( int i = 0; i < 3; i++)
        {
            Instantiate<GameObject>(LifeObj, transform);
        }
    }

    //ダメージ分だけ削除
    public void Reduce()
    {
        Destroy(transform.GetChild(0).gameObject);
    }

}