using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float dodgeCD;
	public float dodgeDuration;
	public float shotCD;
	public float shotSpeed;
	public float playerSpeed;
	public float lifePoints;
	public float dodgeSpeed;
	public GameObject shot;

	public GameObject trailLeft;
	public GameObject trailRight;
	public GameObject trailMid;
	public float fxTime;

	float fxTimer;
	Ray ray;
	float dodgeTimer;
	float dodgeCDTimer;
	float shotTimer;
	bool canShoot;
	bool canDodge;
	bool isDodging;
	Vector3 dir;
	string weapon;
	


	// Use this for initialization
	void Start () 
	{
		canDodge = true;
		dir = transform.position;
		weapon = "Gun";
		dodgeTimer = 0f;
		dodgeCDTimer = 0f;
	}
	
	// Update is called once per frame
	void Update () 
	{

		if ( isDodging == false )
		{

			if (fxTimer >= fxTime )
			{
				trailLeft.SetActive(false);
				trailRight.SetActive(false);
				trailMid.SetActive(false);

			}
			else fxTimer += Time.deltaTime;

		}

		else 
		{
			trailLeft.SetActive(true);
			trailRight.SetActive(true);
			trailMid.SetActive(true);
			fxTimer = 0f;
		}



		dir = transform.position;

		if (Input.GetKeyDown(KeyCode.Space) && canDodge == true)
		{
			isDodging = true;
			canDodge = false;

		}
		if ( isDodging )
		{
			dodgeTimer += Time.deltaTime;
			if ( dodgeTimer >= dodgeDuration )
			{
				isDodging = false;
				dodgeTimer = 0f;
			}
		}
		if ( canDodge == false )
		{
			dodgeCDTimer += Time.deltaTime;
			if ( dodgeCDTimer >= dodgeCD )
			{
				dodgeCDTimer = 0;
				canDodge = true;
			}
		}
		if (canShoot == false)
		{
			shotTimer += Time.deltaTime;
			if(shotTimer >= shotCD)
			{
				canShoot = true;
				shotTimer = 0f;
			}
		}

		// Axe Horizontal
		if (Input.GetAxis ("Horizontal") > 0 || Input.GetAxis ("Horizontal") < 0)
		{
			dir.x += Input.GetAxis("Horizontal") * playerSpeed * Time.deltaTime;
		}

		// Axe Vertical
		if (Input.GetAxis ("Vertical") > 0 || Input.GetAxis ("Vertical") < 0)
		{
			dir.y += Input.GetAxis("Vertical") * playerSpeed * Time.deltaTime;
		}

		if (isDodging)
		{
			dir.x += Input.GetAxis("Horizontal") * dodgeSpeed * Time.deltaTime;
			dir.y += Input.GetAxis("Vertical") * dodgeSpeed * Time.deltaTime;
		}
		transform.position = dir;


		// Système de tir

		RaycastHit hit;
		
		if (Input.GetMouseButton(0) && canShoot && !isDodging)
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast(ray, out hit))
			{
				Vector3 shotLocation = hit.point;
				Vector3 shotDir = shotLocation - transform.position;
				shotDir.z = 0f;
				shotDir = shotDir.normalized;

				GameObject clone = Instantiate (shot, transform.position, transform.rotation) as GameObject;
				//Debug.Log (shotDir);
				clone.GetComponent<Rigidbody>().AddForce(shotDir * shotSpeed);
				canShoot = false;
			}
			
		}

	}
}
