using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent (typeof (AudioSource))]
public class RoomController : MonoBehaviour
{
	GameObject closeDoors;
	GameObject openDoors;
	GameObject enemies;
	AudioSource audioPlayer;
	int nbEnemies;
	bool isClosed = false;
	bool hasSpawned = false;
	float spawnChance = 0.5f;
	// Use this for initialization
	void Start ()
	{
		audioPlayer = GetComponent<AudioSource>();
		closeDoors = transform.Find("Closed").gameObject;
		openDoors = transform.Find("Open").gameObject;
		enemies = transform.Find("Enemies").gameObject;
		nbEnemies = enemies.transform.childCount;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(isClosed)
		{
			closeDoors.SetActive(true);
			openDoors.SetActive(false);
			nbEnemies = enemies.transform.childCount;
			if(nbEnemies == 0)
			{
				isClosed = false;
				SpawnRewardTry();
			}
		}
		else
		{
			closeDoors.SetActive(false);
			openDoors.SetActive(true);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag =="Player" && nbEnemies > 0)
		{
			isClosed = true;
			audioPlayer.PlayOneShot(Resources.Load("Sounds/lock", typeof(AudioClip)) as AudioClip);
		}
	}

	void SpawnRewardTry()
	{
		if(!hasSpawned)
		{
			if(Random.value < spawnChance)
			{
				GameObject heal = Resources.Load("life") as GameObject;
				Instantiate(heal, transform.position, heal.transform.rotation);
			}
			hasSpawned = false;
			
		}
	}


}
