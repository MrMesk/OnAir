using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour
{
	/*
	For optimization purposes we can SetActive(False) the room when the player has exited it. 
	We can use a timer, so the disappearing won't occur while the camera is moving

	*/
	public float timeBeforeDeactivate = 3f;
	float timer;
	bool toDeactivate = false;

	GameObject mainCamera;
	// Use this for initialization
	void Awake()
	{
		mainCamera = GameObject.Find("Main Camera");
		
	}
	void Start()
	{
		if (this.tag != "Starter") transform.Find("Room").gameObject.SetActive(false);
	}
	// Update is called once per frame
	void Update()
	{
		if (toDeactivate)
		{
			timer += Time.deltaTime;
			if (timer >= timeBeforeDeactivate)
			{
				transform.Find("Room").gameObject.SetActive(false);
				timer = 0f;
				toDeactivate = false;
			}
		}
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			mainCamera.GetComponent<CameraLerp>().CameraMove(transform.position);
			transform.Find("Room").gameObject.SetActive(true);
			toDeactivate = false;
			timer = 0f;
		}
	}
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player")
		{
			transform.Find("Room").gameObject.SetActive(true);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			toDeactivate = true;
		}
	}
}
