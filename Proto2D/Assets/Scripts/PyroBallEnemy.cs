using UnityEngine;
using System.Collections;

public class PyroBallEnemy : MonoBehaviour
{
	public float speed;
	public float accel;
	public float maxSpeed;
	public float hitRate;
	public int dmg;

	float timer;
	float actualSpeed;
	GameObject player;

	// Use this for initialization
	void Start ()
	{
		actualSpeed = speed;
		player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GameObject.Find("Player") != null)
		{
			actualSpeed += Time.deltaTime * accel;
			actualSpeed = Mathf.Clamp(actualSpeed, speed, maxSpeed);

			transform.position = Vector3.MoveTowards(transform.position, player.transform.position, actualSpeed * Time.deltaTime);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			actualSpeed = speed;
			other.gameObject.GetComponent<LifeManager>().StartCoroutine(other.gameObject.GetComponent<LifeManager>().damage(dmg));
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player")
		{
			timer += Time.deltaTime;
			if (timer >= hitRate)
			{
				other.gameObject.GetComponent<LifeManager>().StartCoroutine(other.gameObject.GetComponent<LifeManager>().damage(dmg));
				//other.gameObject.GetComponent<PlayerController>().damagePlayer(dmg);
				timer = 0f;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			timer = 0f;
		}
	}
}
