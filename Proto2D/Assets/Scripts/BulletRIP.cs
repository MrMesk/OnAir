using UnityEngine;
using System.Collections;

public class BulletRIP : MonoBehaviour 
{
	public float lifeTime;
	float lifeTimer;

	// Use this for initialization
	void Start () 
	{
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
	}

	void OnTriggerEnter (Collider other)
	{
		if ( other.tag == "Enemy" )
		{
			Destroy (other.gameObject);
		}
		Destroy (this.gameObject);
	}
}
