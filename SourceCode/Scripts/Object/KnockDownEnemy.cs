using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockDownEnemy : MonoBehaviour
{

	[SerializeField]	//X方向への移動距離
	float moveX = 0;
	[SerializeField]	//Y方向への移動距離
	float moveY = 0;
	[SerializeField]	//Z方向への移動距離
	float moveZ = 0;
	[SerializeField]	//速度
	float speed = 0;
	[SerializeField]	//待機時間
	float waitTime = 0;
	[SerializeField]    //死亡アニメ
	GameObject DeathEffect = null;
	

	bool stop = false;
	float step;
	bool goBack = false;
	Vector3 origin;
	Vector3 destination;


	void Start()
	{
		origin = transform.position;
		destination = new Vector3(origin.x - moveX, origin.y - moveY, origin.z - moveZ);
	}

	//移動処理
	void Update()
	{
		if (stop)
		{
			return;
		}
		
		step = speed * Time.deltaTime;

		if (!goBack)
		{
			transform.position = Vector3.MoveTowards(transform.position, destination, step);

			if (transform.position == destination)
			{
				goBack = true;
				if(moveX != 0)
                {
					transform.localScale = new Vector3(-1, 1, 1);
				}
			}
		}
		else
		{
			transform.position = Vector3.MoveTowards(transform.position, origin, step);

			if (transform.position == origin)
			{
				goBack = false;
				if (moveX != 0)
				{
					transform.localScale = new Vector3(1, 1, 1);
				}
			}
		}
	}

	//倒されたとき
	public void DestroyEnemy()
	{
		
		Instantiate(DeathEffect, this.transform.position, this.transform.rotation);
		Destroy(this.gameObject);
	}

}
