﻿using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
	public Vector3 rotateSpeed;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate(rotateSpeed * Time.deltaTime);
	}
}