using UnityEngine;
using System.Collections;

public class ChangeWeapon : MonoBehaviour
{
	public int weaponToLoad;

	void OnTriggerStay(Collider other)
	{
		if(other.tag == "Player")
		{
			if(other.GetComponent<PlayerController>().indexWeapons != weaponToLoad)
			{
				other.GetComponent<PlayerController>().indexWeapons = weaponToLoad;
            }
		}
	}
}
