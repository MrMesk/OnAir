using UnityEngine;
using System.Collections;

public class EnemyBullet : MonoBehaviour
{
	[Header("Bullet Stats")]
	public float lifeTime;
	public int dmg;

	[Header("Shards")]
	public GameObject shards;
	public float shardSpeed;
	public int nbShards;
	public bool doesExplode;

	float timer;

	void OnEnable()
	{
		timer = 0f;
	}

	void FixedUpdate ()
	{
		timer += Time.deltaTime;

		if(timer >= lifeTime)
		{
			if(doesExplode)
			{
				timer = 0f;
				SpawnShardsExplode();
			}
			timer = 0f;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			ObjectPooler.current.PoolObject(gameObject);
		}
	}


	void SpawnShardsExplode()
	{
		timer = 0f;
		Vector3 posShard = Vector3.zero;

		for (int i = 0; i < nbShards; i++)
		{
			if (i == 0)
			{
				posShard = Vector3.forward;
			}
			else
			{
				posShard = Quaternion.AngleAxis((360 / nbShards) * i, transform.forward) * posShard;
			}

			GameObject shard = ObjectPooler.current.GetObject(shards);
			shard.SetActive(true);
			shard.transform.position = transform.position;
			shard.transform.rotation = transform.rotation;

			shard.GetComponent<Rigidbody>().velocity = posShard * shardSpeed;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			other.gameObject.GetComponent<LifeManager>().StartCoroutine(other.gameObject.GetComponent<LifeManager>().damage(dmg));

			GetComponent<Rigidbody>().velocity = Vector3.zero;
			ObjectPooler.current.PoolObject(gameObject);
		}
		else if(other.gameObject.tag == "Wall")
		{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			ObjectPooler.current.PoolObject(gameObject);
		}
	}
}
