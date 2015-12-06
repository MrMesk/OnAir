using UnityEngine;
using System.Collections;

public class CameraLerp : MonoBehaviour
{
	Vector3 destination = new Vector3(0, 10, 0);
	public float speed;

	void Update ()
	{
		if(transform.position != destination)
		{
			Vector3 dir;
			dir = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
			dir.y = 10f;
			transform.position = dir;
		}
	}

	public void CameraMove(Vector3 newDestination)
	{
		destination = newDestination;
	}
}
