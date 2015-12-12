using UnityEngine;
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
	public AudioClip switchWeapon;
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
	public LayerMask swordLayer;

	[Header("Player Stats")]
	public float playerSpeed;
	public float dodgeSpeed;
	public float dodgeCD;
	public float dodgeDuration;
	public GameObject weaponSprite;

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
	public int indexWeapons = 0;
	int nbWeapons;

	// Use this for initialization
	void Start () 
	{
		nbWeapons = weaponTypes.GetNames(typeof(weaponTypes)).Length;
		switch(weapon)
		{
			case weaponTypes.Sword:
				indexWeapons = 0;
				break;
			case weaponTypes.Gun:
				indexWeapons = 1;
				break;
			case weaponTypes.ShotGun:
				indexWeapons = 2;
				break;
			case weaponTypes.PsyBall:
				indexWeapons = 3;
				break;
			case weaponTypes.Boomerang:
				indexWeapons = 4;
				break;
		}
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
		WeaponSwitch();
    }

	void WeaponSwitch()
	{
		if(Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			if (indexWeapons == nbWeapons - 1) indexWeapons = 0;
			else indexWeapons++;
			audioPlayer.PlayOneShot(switchWeapon);
		}
		else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
			if (indexWeapons == 0) indexWeapons = nbWeapons -1;
			else indexWeapons--;
			audioPlayer.PlayOneShot(switchWeapon);
		}

		if(Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			switch (indexWeapons)
			{
				case 0:
					weapon = weaponTypes.Sword;
					weaponSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load("Weapons/sword", typeof(Sprite)) as Sprite;
					break;
				case 1:
					weapon = weaponTypes.Gun;
					weaponSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load("Weapons/gun", typeof(Sprite)) as Sprite;
					break;
				case 2:
					weapon = weaponTypes.ShotGun;
					weaponSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load("Weapons/shotgun", typeof(Sprite)) as Sprite;
					break;
				case 3:
					weapon = weaponTypes.PsyBall;
					weaponSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load("Weapons/remote", typeof(Sprite)) as Sprite;
					break;
				case 4:
					weapon = weaponTypes.Boomerang;
					weaponSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load("Weapons/crossMerang", typeof(Sprite)) as Sprite;
					break;
			}
		}
		
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
		if (Input.GetAxis("Horizontal") > 0)
		{
			moveDir.x += Input.GetAxis("Horizontal") * playerSpeed * Time.deltaTime;

		}
		else if (Input.GetAxis("Horizontal") < 0)
		{
			moveDir.x += Input.GetAxis("Horizontal") * playerSpeed * Time.deltaTime;
		}

		// Axe Vertical
		if (Input.GetAxis("Vertical") > 0)
		{
			moveDir.z += Input.GetAxis("Vertical") * playerSpeed * Time.deltaTime;
		}
		else if (Input.GetAxis("Vertical") < 0)
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
				shotDir = shotLocation - weaponSprite.transform.position;
				shotDir = shotDir.normalized;

				switch (weapon)
				{

					case weaponTypes.Gun:

						shotCD = gunCD;

						GameObject clone = ObjectPooler.current.GetObject(shot);
						clone.SetActive(true);
						clone.transform.position = weaponSprite.transform.position;
						clone.transform.rotation = weaponSprite.transform.rotation;
						clone.GetComponent<Rigidbody>().velocity = (shotDir * shotSpeed);

						audioPlayer.PlayOneShot(gunSound);
						canShoot = false;

						break;

					case weaponTypes.Sword:

						shotCD = swordCD;
						Collider[] potentialTargets = Physics.OverlapSphere(transform.position, swordRadius, swordLayer);
						for (int i = 0; i < potentialTargets.Length; i++)
						{
							Vector3 enemyPos = potentialTargets[i].transform.position - transform.position;
							if (Vector3.Angle(enemyPos.normalized, shotDir.normalized) < 90)
							{
								if(potentialTargets[i].tag == "Enemy")
								{
									potentialTargets[i].gameObject.GetComponent<Rigidbody>().velocity = ((potentialTargets[i].transform.position - transform.position).normalized * swordForce);
									potentialTargets[i].gameObject.GetComponent<LifeManager>().StartCoroutine(potentialTargets[i].gameObject.GetComponent<LifeManager>().damage(swordDmg));
								}
								else if (potentialTargets[i].tag == "Boss")
								{
									potentialTargets[i].gameObject.GetComponent<BossBattle>().StartCoroutine(potentialTargets[i].gameObject.GetComponent<BossBattle>().damageBoss(swordDmg));
								}
								else if(potentialTargets[i].tag == "EnemyBullet")
								{
									potentialTargets[i].gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                                    ObjectPooler.current.PoolObject(potentialTargets[i].gameObject);
								}

							}
						}
						StartCoroutine(animSword());
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
							shotgun.transform.position = weaponSprite.transform.position;
							shotgun.transform.rotation = weaponSprite.transform.rotation;
							shotgun.GetComponent<Rigidbody>().velocity = (dirShot * shotSpeed);
						}
						audioPlayer.PlayOneShot(shotGunSound);
						canShoot = false;
						break;

					case weaponTypes.Boomerang:

						shotCD = boomerangCD;
						GameObject boomerang = ObjectPooler.current.GetObject(boomerangBall);
						boomerang.SetActive(true);
						boomerang.transform.position = weaponSprite.transform.position;
						boomerang.transform.rotation = weaponSprite.transform.rotation;

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

	public IEnumerator animSword()
	{
		weaponSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load("Weapons/swordSlash", typeof(Sprite)) as Sprite;
		yield return new WaitForSeconds(0.1f);
		weaponSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load("Weapons/sword", typeof(Sprite)) as Sprite;
	}

}