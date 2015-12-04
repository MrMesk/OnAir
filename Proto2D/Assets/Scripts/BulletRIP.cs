using UnityEngine;

public class BulletRIP : MonoBehaviour 
{
	public float lifeTime;
	public float bouncingLife;
    public int attackPower;

    float bounceTimer;
	bool toDestroy;
	float lifeTimer;

	// Use this for initialization
	protected virtual void OnEnable () 
	{
		toDestroy = false;
		lifeTimer = 0f;
		bounceTimer = 0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		lifeTimer += Time.deltaTime;
		if (lifeTimer >= lifeTime )
		{
			lifeTimer = 0f;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			ObjectPooler.current.PoolObject(gameObject);
		}
		if(toDestroy)
		{
			bounceTimer += Time.deltaTime;
			if (bounceTimer >= bouncingLife )
			{
				toDestroy = false;
				bounceTimer = 0f;
				GetComponent<Rigidbody>().velocity = Vector3.zero;
                ObjectPooler.current.PoolObject(gameObject);
			}
		}
	}

	void OnCollisionEnter (Collision other)
	{
		if ( other.gameObject.tag == "Enemy" )
		{
            other.gameObject.GetComponent<LifeManager>().StartCoroutine(other.gameObject.GetComponent<LifeManager>().damage(attackPower));
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			ObjectPooler.current.PoolObject(gameObject);
        }
		else toDestroy = true;

	}
}
