using UnityEngine;
using System.Collections;

public class EnemyBullet : MonoBehaviour
{
	public gameDifficulty difficulty;
	[Header("Bullet Stats")]
	public float lifeTime;
	public int dmg;

	[Header("Shards")]
	public GameObject shards;
	public float shardSpeed;
	public int nbShards;
	public bool doesExplode;

	float timer;
	
	public enum gameDifficulty
	{
		normal,
		hard
	};

	void OnEnable()
	{
		timer = 0f;
		
	}

	Vector3 trajectory;

	// Use this for initialization
	void Start ()
	{
		trajectory = GetComponent<Rigidbody>().velocity;
		trajectory = trajectory.normalized;
		//Debug.Log(trajectory);
	}
	
	// Update is called once per frame
	void Update ()
	{
		timer += Time.deltaTime;

		if(timer >= lifeTime)
		{
			if(doesExplode)
			{
				SpawnShardsExplode();
			}
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			ObjectPooler.current.PoolObject(gameObject);
		}
	}

	void SpawnShardsWall()
	{
		int dirShard = 1;
		Vector3 posShard;

		for (int i = 0; i < nbShards; i++)
		{
			if (i == 0)
			{
				posShard = trajectory * -1;
			}
			else
			{
				posShard = Quaternion.AngleAxis((90/nbShards) * dirShard, transform.up) * (trajectory * -1);
				if (dirShard > 0) dirShard = dirShard * -1;
				else dirShard = (dirShard - 1) * -1;
			}
			GameObject shard = ObjectPooler.current.GetObject(shards);
			shard.SetActive(true);
			shard.transform.position = transform.position;
			shard.transform.rotation = transform.rotation;

			shard.GetComponent<Rigidbody>().velocity = posShard * shardSpeed;
		}
	}

	void SpawnShardsExplode()
	{
		Vector3 posShard = Vector3.forward;

		for (int i = 0; i < nbShards; i++)
		{
			if (i == 0)
			{
				posShard = Vector3.forward;
			}
			else
			{
				posShard = Quaternion.AngleAxis((360 / nbShards) * i, transform.up) * posShard;
			}

			GameObject shard = ObjectPooler.current.GetObject(shards);
			shard.SetActive(true);
			shard.transform.position = transform.position;
			shard.transform.rotation = transform.rotation;

			shard.GetComponent<Rigidbody>().velocity = posShard * shardSpeed;
		}
	}

	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.tag == "Player")
		{
			other.gameObject.GetComponent<PlayerController>().damagePlayer(dmg);
			ObjectPooler.current.PoolObject(gameObject);
		}
		else if(other.gameObject.tag == "Wall")
		{
			switch(difficulty)
			{
				case gameDifficulty.normal:
					GetComponent<Rigidbody>().velocity = Vector3.zero;
					ObjectPooler.current.PoolObject(gameObject);
					break;
				case gameDifficulty.hard:

					SpawnShardsWall();
					GetComponent<Rigidbody>().velocity = Vector3.zero;
					ObjectPooler.current.PoolObject(gameObject);
					break;
			}
		}
	}
}
