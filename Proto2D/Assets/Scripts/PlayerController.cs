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

	public float swordRadius;
	public enum weaponTypes
	{
		Sword,
		Gun
	};
	public weaponTypes weapon;

	float fxTimer;
	Ray ray;
	float dodgeTimer;
	float dodgeCDTimer;
	float shotTimer;
	bool canShoot;
	bool canDodge;
	bool isDodging;
	Vector3 moveDir;
	Vector3 shotDir;
	Vector3 toEnemy;
	//public string weapon;
	public float swordForce;

	public float swordCD;
	public float gunCD;
	


	// Use this for initialization
	void Start () 
	{
		canDodge = true;
		moveDir = transform.position;
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



		moveDir = transform.position;

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
			moveDir.x += Input.GetAxis("Horizontal") * playerSpeed * Time.deltaTime;
		}

		// Axe Vertical
		if (Input.GetAxis ("Vertical") > 0 || Input.GetAxis ("Vertical") < 0)
		{
			moveDir.z += Input.GetAxis("Vertical") * playerSpeed * Time.deltaTime;
		}

		if (isDodging)
		{
			moveDir.x += Input.GetAxis("Horizontal") * dodgeSpeed * Time.deltaTime;
			moveDir.z += Input.GetAxis("Vertical") * dodgeSpeed * Time.deltaTime;
		}
		transform.position = moveDir;


		// Système de tir
		RaycastHit hit;
		
		if (Input.GetMouseButton(0) && canShoot && !isDodging)
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
			{
				Vector3 shotLocation = hit.point;
				shotDir = shotLocation - transform.position;
				shotDir = shotDir.normalized;

				switch(weapon)
				{
				case weaponTypes.Gun:
					shotCD = 0.15f;
					GameObject clone = Instantiate (shot, transform.position, transform.rotation) as GameObject;
					clone.GetComponent<Rigidbody>().AddForce(shotDir * shotSpeed);
					canShoot = false;
				
				break;

				case weaponTypes.Sword:
					shotCD = swordCD;
					Collider[] potentialTargets = Physics.OverlapSphere(transform.position,swordRadius, 1<<10);
					for (int i =0; i< potentialTargets.Length; i++) 
					{
						Vector3 enemyPos = potentialTargets[i].transform.position - transform.position;
						if(Vector3.Angle(enemyPos.normalized,shotDir.normalized) < 45)
						{
							potentialTargets[i].gameObject.GetComponent<Rigidbody>().AddForce((potentialTargets[i].transform.position - transform.position).normalized * swordForce/*,ForceMode.Impulse*/);
						}
					}
					canShoot = false;
				break;
				}

			
			}

		}
		if(Input.GetMouseButtonDown (1))
		{
			if(weapon == weaponTypes.Sword)
			{
				weapon = weaponTypes.Gun;
			}
			else weapon = weaponTypes.Sword;
		}
	}
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + shotDir);
	}
}