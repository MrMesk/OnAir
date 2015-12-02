﻿using UnityEngine;
using System.Collections;

public class Boomerang : MonoBehaviour
{
	public float speed = 5f;
	public float hitRate = 0.2f;
	public int damage = 3;
	public float maxDistance = 5f;

	float timer = 0f;
	bool isGoing = true; // Pour savoir si le boomerang est au stade aller ou retour

	Vector3 dest;
	GameObject player;

	// Use this for initialization
	void Start ()
	{
		player = GameObject.Find("Player");
	}
	
	void OnEnable()
	{
		timer = 0f;
		isGoing = true; // On 
	}

	// Update is called once per frame
	void Update ()
	{
		if(!isGoing)
		{
			GoToPlayer();
		}
		else if(Vector3.Distance(transform.position, player.transform.position) > maxDistance)
		{
			isGoing = false;
			GoToPlayer();
		}
		if (Vector3.Distance(transform.position, dest) < 0.5f )
		{
			if(!isGoing)
			{
				ObjectPooler.current.PoolObject(gameObject);
			}
			else
			{
				isGoing = false;
				GoToPlayer();
			}

		}

		transform.position = Vector3.MoveTowards(transform.position, dest, speed * Time.deltaTime);
	}

	public void GoToClick ( Vector3 destination )
	{
		dest = destination;
		dest.y = 0f;
	}

	public void GoToPlayer ()
	{
		dest = player.transform.position;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy") other.gameObject.GetComponent<EnemyController>().StartCoroutine(other.gameObject.GetComponent<EnemyController>().damage(damage));
		else if (other.tag == "Wall") isGoing = false;
	}
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Enemy")
		{
			timer += Time.deltaTime;
			if (timer >= hitRate)
			{
				other.gameObject.GetComponent<EnemyController>().StartCoroutine(other.gameObject.GetComponent<EnemyController>().damage(damage));
				timer = 0f;
			}
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Enemy") timer = 0f;
	}
}