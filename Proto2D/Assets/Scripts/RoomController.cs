using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomController : MonoBehaviour
{
	GameObject closeDoors;
	GameObject openDoors;
	GameObject enemies;
	int nbEnemies;
	bool isClosed = false;
	// Use this for initialization
	void Start ()
	{
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
		}
	}


}
