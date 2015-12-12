using UnityEngine;
using System.Collections;

public class NoDoublons : MonoBehaviour
{

	// Use this for initialization
	void Awake()
	{
		if (GameObject.FindGameObjectsWithTag(gameObject.tag).Length > 1)
		{
			Destroy(gameObject);
		}
	}
}
