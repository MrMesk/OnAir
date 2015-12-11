using UnityEngine;
using System.Collections;

public class AttackManager : MonoBehaviour
{
	Ray ray;
	public GameObject weapSprite;
	float actualScale;
	Vector3 locScale;
	// Use this for initialization
	void Start ()
	{
		locScale = weapSprite.transform.localScale;
		actualScale = weapSprite.transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update ()
	{
		RaycastHit hit;
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit))
		{
			Vector3 dir;
			dir = (hit.point - transform.position).normalized;
			dir.y = 0f;
			transform.forward = dir;
		}
		InvertScale();		
	}

	void InvertScale()
	{
		if (transform.eulerAngles.y > 180f && transform.eulerAngles.y < 360f && locScale.x != actualScale * -1)
		{
			locScale.x = actualScale * -1;
			weapSprite.transform.localScale = locScale;
		}
		if (transform.eulerAngles.y <= 180f && transform.eulerAngles.y >= 0f && locScale.x != actualScale)
		{
			locScale.x = actualScale;
			weapSprite.transform.localScale = locScale;
		}
	}
}
