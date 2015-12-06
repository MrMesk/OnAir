using UnityEngine;
using System.Collections;

public class PyroShaman : MonoBehaviour
{
	public LayerMask visionMask;
	public GameObject pyroBall;
	public float aggroRange;

	public enum EnemyStates
	{
		Idle,
		Attack
	}
	EnemyStates state;
	GameObject _player;

	bool hasSpawned = false;

	Ray ray;

	// Use this for initialization
	void Start ()
	{
		state = EnemyStates.Idle;
		_player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (state)
		{
			case EnemyStates.Idle:
				Idle();
				break;


			case EnemyStates.Attack:
				Attack();
				break;
		}
	}

	void Idle()
	{
		if(Vector3.Distance(_player.transform.position, transform.position) < aggroRange)
		{
			RaycastHit hit;
			if(Physics.Raycast(transform.position, (_player.transform.position - transform.position), out hit, float.PositiveInfinity, visionMask))
			{
				if(hit.collider.tag == "Player")
				{
					state = EnemyStates.Attack;
				}
			}
		}
		return;
	}

	void Attack()
	{
		if (!hasSpawned)
		{
			Vector3 toPlayer;
			toPlayer = _player.transform.position - transform.position;
			toPlayer = toPlayer.normalized;

			GameObject pyro = (GameObject)Instantiate(pyroBall, transform.position + toPlayer, transform.rotation);
			pyro.transform.parent = transform;
			hasSpawned = true;
		}
		else state = EnemyStates.Idle;
		
	}


}
