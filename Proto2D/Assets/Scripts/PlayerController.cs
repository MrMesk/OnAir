﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public enum weaponTypes
	{
		Sword,
		Gun,
		ShotGun,
		PsyBall,
		Boomerang
	};
	public weaponTypes weapon;

	[Header("Audio")]
	public AudioClip gunSound;
	public AudioClip swordWhoosh;
	public AudioClip shotGunSound;
	AudioSource audioPlayer;

	[Header("Shotgun")]
	public GameObject shotgunShell;
	public float shotgunCD;
	public float nbShells;
	public float shotgunSpread;

	[Header("Gun")]
	public GameObject shot;
	public float gunCD;
	public float shotSpeed;

	[Header("Psy Ball")]
	public GameObject psyBall;

	[Header("Boomerang Ball")]
	public GameObject boomerangBall;
	public float boomerangCD;

	[Header("Sword")]
	public float swordCD;
	public float swordRadius;
	public float swordForce;
	public int swordDmg;

	[Header("Player Stats")]
	public float playerSpeed;
	public float dodgeSpeed;
	public float dodgeCD;
	public float dodgeDuration;

	[Header("Trail effects")]
	public GameObject trailLeft;
	public GameObject trailRight;
	public GameObject trailMid;
	public float fxTime;

	float shotCD;
	float fxTimer;
	float dodgeTimer;
	float dodgeCDTimer;
	float shotTimer;

	bool canShoot;
	bool canDodge;
	bool isDodging;

	Vector3 moveDir;
	Vector3 shotDir;
	Vector3 toEnemy;
	Ray ray;

	// Use this for initialization
	void Start () 
	{
		audioPlayer = GetComponent<AudioSource>();
		canDodge = true;
		moveDir = transform.position;
		dodgeTimer = 0f;
		dodgeCDTimer = 0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		DodgeManage();
		MoveManage();
		WeaponManage();
    }

	void DodgeManage()
	{
		if (isDodging == false)
		{
			if (fxTimer >= fxTime)
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
		if (isDodging)
		{
			dodgeTimer += Time.deltaTime;
			if (dodgeTimer >= dodgeDuration)
			{
				isDodging = false;
				dodgeTimer = 0f;
			}
		}
		if (canDodge == false)
		{
			dodgeCDTimer += Time.deltaTime;
			if (dodgeCDTimer >= dodgeCD)
			{
				dodgeCDTimer = 0;
				canDodge = true;
			}
		}
	}

	void MoveManage()
	{
		// A remplacer par quelque chose de plus avancé
		// Axe Horizontal
		if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Horizontal") < 0)
		{
			moveDir.x += Input.GetAxis("Horizontal") * playerSpeed * Time.deltaTime;
		}

		// Axe Vertical
		if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("Vertical") < 0)
		{
			moveDir.z += Input.GetAxis("Vertical") * playerSpeed * Time.deltaTime;
		}

		if (isDodging)
		{
			moveDir.x += Input.GetAxis("Horizontal") * dodgeSpeed * Time.deltaTime;
			moveDir.z += Input.GetAxis("Vertical") * dodgeSpeed * Time.deltaTime;
		}
		transform.position = moveDir;
	}

	void WeaponManage()
	{
		if (canShoot == false)
		{
			shotTimer += Time.deltaTime;
			if (shotTimer >= shotCD)
			{
				canShoot = true;
				shotTimer = 0f;
			}
		}

		// Gestion des armes et du tir

		if (weapon == weaponTypes.PsyBall)
		{
			if (GameObject.Find("PsyBall(Clone)") == null)
			{
				Instantiate(psyBall, transform.position, transform.rotation);
			}
		}
		else
		{
			Destroy(GameObject.Find("PsyBall(Clone)"));
		}
		RaycastHit hit;

		if (Input.GetMouseButton(0) && canShoot && !isDodging)
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
			{
				Vector3 shotLocation = hit.point;
				shotDir = shotLocation - transform.position;
				shotDir = shotDir.normalized;

				switch (weapon)
				{

					case weaponTypes.Gun:

						shotCD = gunCD;

						GameObject clone = ObjectPooler.current.GetObject(shot);
						clone.SetActive(true);
						clone.transform.position = transform.position;
						clone.transform.rotation = transform.rotation;
						clone.GetComponent<Rigidbody>().velocity = (shotDir * shotSpeed);

						audioPlayer.PlayOneShot(gunSound);
						canShoot = false;

						break;

					case weaponTypes.Sword:

						shotCD = swordCD;
						Collider[] potentialTargets = Physics.OverlapSphere(transform.position, swordRadius, 1 << 10);
						for (int i = 0; i < potentialTargets.Length; i++)
						{
							Vector3 enemyPos = potentialTargets[i].transform.position - transform.position;
							if (Vector3.Angle(enemyPos.normalized, shotDir.normalized) < 45)
							{
								potentialTargets[i].gameObject.GetComponent<Rigidbody>().AddForce((potentialTargets[i].transform.position - transform.position).normalized * swordForce);
								potentialTargets[i].gameObject.GetComponent<LifeManager>().StartCoroutine(potentialTargets[i].gameObject.GetComponent<LifeManager>().damage(swordDmg));
							}
						}
						audioPlayer.PlayOneShot(swordWhoosh);
						canShoot = false;
						break;

					case weaponTypes.ShotGun:

						shotCD = shotgunCD;
						for (int i = 0; i < nbShells; i++)
						{
							float angle = Random.Range(shotgunSpread / -2, shotgunSpread / 2);
							Vector3 dirShot = Quaternion.AngleAxis(angle, transform.up) * shotDir;
							GameObject shotgun = ObjectPooler.current.GetObject(shotgunShell);
							shotgun.SetActive(true);
							shotgun.transform.position = transform.position;
							shotgun.transform.rotation = transform.rotation;
							shotgun.GetComponent<Rigidbody>().velocity = (dirShot * shotSpeed);
						}
						audioPlayer.PlayOneShot(shotGunSound);
						canShoot = false;
						break;

					case weaponTypes.Boomerang:

						shotCD = boomerangCD;
						GameObject boomerang = ObjectPooler.current.GetObject(boomerangBall);
						boomerang.SetActive(true);
						boomerang.transform.position = transform.position;
						boomerang.transform.rotation = transform.rotation;

						boomerang.GetComponent<Boomerang>().GoToClick(hit.point);
						canShoot = false;
						break;
				}
			}
		}
	}

	void OnDrawGizmos()
	{
		//Gizmos.color = Color.red;
		//Gizmos.DrawLine(transform.position, transform.position + shotDir);
	}

}