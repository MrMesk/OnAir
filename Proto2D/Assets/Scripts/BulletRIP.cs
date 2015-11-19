using UnityEngine;
using System.Collections;

public class BulletRIP : MonoBehaviour 
{
	public float lifeTime;
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
		if(toDestroy)
		{
			lifeTimer += Time.deltaTime;
			if (lifeTimer >= lifeTime )
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
