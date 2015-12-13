using UnityEngine;
using System.Collections;

public class HealOnPickup : MonoBehaviour
{

	public int amount;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			other.GetComponent<LifeManager>().StartCoroutine(other.GetComponent<LifeManager>().heal(amount));
			Destroy(this.gameObject);
		}
	}
}
