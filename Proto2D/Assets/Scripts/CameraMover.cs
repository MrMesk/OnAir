using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour
{
	GameObject mainCamera;
	// Use this for initialization
	void Start ()
	{
		mainCamera = GameObject.Find("Main Camera");
		transform.Find("Room").gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	void OnTriggerEnter(Collider other)
	{
		if(other.tag =="Player")
		{
			mainCamera.GetComponent<CameraLerp>().CameraMove(transform.position);
			//Transform roomParent;
			//roomParent = transform.parent;
			transform.Find("Room").gameObject.SetActive(true);
		}
	}
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player")
		{
			transform.Find("Room").gameObject.SetActive(true);
		}
    }
}
