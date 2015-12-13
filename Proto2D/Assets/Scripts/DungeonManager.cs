using UnityEngine;
using System.Collections;

public class DungeonManager : MonoBehaviour
{
	public int nbRooms;
	GameObject[] gameManagers;
	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad(this);
	}
	void Awake()
	{
		if (GameObject.FindGameObjectsWithTag(gameObject.tag).Length > 1)
		{
			Destroy(gameObject);
		}
	}
}
