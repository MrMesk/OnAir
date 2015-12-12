using UnityEngine;
using System.Collections;

public class HandManager : MonoBehaviour
{
	public float speed;
	public Vector3 destination;
	Vector3 initialPos;
	Vector3 dir;
	public bool isMoving;
	// Use this for initialization
	void Start ()
	{
		initialPos = transform.localPosition;
		dir = destination;
		isMoving = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Debug.Log(this.name + " Is Moving : " + isMoving); 
		if(isMoving)
		{
			if(transform.localPosition != dir)
			{
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, dir, speed * Time.deltaTime);
			}
			else
			{
				if (dir == initialPos)
				{
					dir = destination;
				}
				else if (dir == destination)
				{
					dir = initialPos;
				}
			}
		}
		else if(transform.localPosition != initialPos)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, initialPos, speed * 4 * Time.deltaTime);
		}
	}
}
