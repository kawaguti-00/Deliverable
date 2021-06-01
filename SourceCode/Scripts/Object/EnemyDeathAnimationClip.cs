using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathAnimationClip : MonoBehaviour
{
    public void OnCompleteAnimation()
    {
        Destroy(this.gameObject);
    }
}
