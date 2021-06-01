using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkEnemy : MonoBehaviour
{

	[SerializeField]    //速度
	float speed = 0;
	[SerializeField]    //死亡アニメ
	GameObject DeathEffect = null;

	Rigidbody2D rigidbody2D;

	void Start()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	//移動処理
	void Update()
	{
		rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);
	}

	//倒されたとき
	public void DestroyEnemy()
	{

		Instantiate(DeathEffect, this.transform.position, this.transform.rotation);
		Destroy(this.gameObject);
	}
}
