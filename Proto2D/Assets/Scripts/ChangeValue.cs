using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeValue : MonoBehaviour 
{
	ProceduralCreate dungeonCreator;
	public InputField nbRooms;
	int newNbRooms;

	void Start()
	{
		dungeonCreator = GameObject.Find("DungeonCreator").GetComponent<ProceduralCreate>();
	}
	void LockInput()
	{
		newNbRooms = int.Parse(nbRooms.text.ToString());
		Debug.Log (newNbRooms);
		dungeonCreator.nbSalles = newNbRooms;
	}

}
