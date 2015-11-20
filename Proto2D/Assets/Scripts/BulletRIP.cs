using UnityEngine;
using System.Collections;

public class BulletRIP : MonoBehaviour 
{
	public float lifeTime;
	public float bouncingLife;
	float bounceTimer;
	bool toDestroy;
	float lifeTimer;

	// Use this for initialization
	void Start () 
	{
		toDestroy = false;
		lifeTimer = 0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		lifeTimer += Time.deltaTime;
		if (lifeTimer >= lifeTime )
		{
			Destroy (this.gameObject);
		}
		if(toDestroy)
		{
			bounceTimer += Time.deltaTime;
			if (bounceTimer >= bouncingLife )
			{
				Destroy (this.gameObject);
			}
		}
	}

	void OnCollisionEnter (Collision other)
	{
		if ( other.gameObject.tag == "Enemy" )
		{
			Destroy (this.gameObject);
			Destroy (other.gameObject);
		}
		else toDestroy = true;

	}
}
