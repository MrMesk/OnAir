using UnityEngine;
using System.Collections;

public class SpawnEnemy : MonoBehaviour
{
	public GameObject enemy;
	Transform parent;

	// Use this for initialization
	void Awake ()
	{
		GameObject mob = Instantiate(enemy, transform.position, transform.rotation) as GameObject;
		parent = transform.parent;
		mob.transform.parent = parent.transform.Find("Enemies");
		Destroy(this.gameObject);
	}

}
