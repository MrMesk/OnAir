using UnityEngine;
using System.Collections;

public class BossBattle : MonoBehaviour
{
	[Header("General")]
	// Public
	public int lifePoints = 1800;
	public float timeBetweenSwitch = 15f;
	public float multiplier = 1f;
	public Color hitColor = Color.red;
	public AudioClip ouch;
	public GameObject particleDeath;
	public GameObject particleSwitch;
	public GameObject particleSpawn;
	public float timeBetweenActions = 2;

	// Private
	int actualLifePoints;
	float timerBetweenActions = 0;
	Color normalColor;
	AudioSource audioPlayer;

	[Header("RedMask")]
	// Public
	public int maxLifeRed = 600;
	public float rateofFire;
	public float shotSpeed;
	public GameObject redShot;
	// Private
	int actualLifeRed;

	[Header("GreenMask")]
	// Public
	public int maxLifeGreen = 600;
	public GameObject enemy1;
	public GameObject enemy2;
	public GameObject enemy3;
	public float spawnRate = 3f;
	// Private
	int actualLifeGreen;
	int enemyNumber = 1;

	[Header("BlueMask")]
	// Public
	public int maxLifeBlue = 600;
	public float aoeRate;
	public GameObject blueShot;
	// Private
	int actualLifeBlue;

	//
	GameObject mask;
	GameObject leftHand;
	GameObject rightHand;

	bool isRedAlive = true;
	bool isBlueAlive = true;
	bool isGreenAlive = true;

	bool left = true;

	float timer;
	float changeTimer;
	int step;

	void Awake()
	{
		audioPlayer = GetComponent<AudioSource>();
		leftHand = GameObject.Find("LeftHand");
		rightHand = GameObject.Find("RightHand");
		mask = GameObject.Find("Mask");
		normalColor = mask.GetComponent<Renderer>().material.color;
		actualLifePoints = lifePoints;
		actualLifeBlue = maxLifeBlue;
		actualLifeGreen = maxLifeGreen;
		actualLifeRed = maxLifeRed;

		step = Random.Range(1, 4);
	}
	// Use this for initialization
	void Start ()
	{
		switch (step)
		{
			case 1:
				mask.GetComponent<SpriteRenderer>().sprite = Resources.Load("RedMask", typeof(Sprite)) as Sprite;
				break;

			case 2:
				mask.GetComponent<SpriteRenderer>().sprite = Resources.Load("BlueMask", typeof(Sprite)) as Sprite;
				break;

			case 3:
				mask.GetComponent<SpriteRenderer>().sprite = Resources.Load("GreenMask", typeof(Sprite)) as Sprite;
				break;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(GameObject.Find("Player") != null)
		{
			changeTimer += Time.deltaTime;
			if (changeTimer >= timeBetweenSwitch)
			{
				MaskSwitcher();
				timerBetweenActions = timeBetweenActions;
			}

			// State machine
			switch (step)
			{
				case 1:
					if (timerBetweenActions <= 0)
					{
						if (leftHand.GetComponent<HandManager>().isMoving == false)
						{
							leftHand.GetComponent<HandManager>().isMoving = true;
						}
						if (rightHand.GetComponent<HandManager>().isMoving == false)
						{
							rightHand.GetComponent<HandManager>().isMoving = true;
						}

						timer += Time.deltaTime;
						if (timer > rateofFire / multiplier)
						{
							RedMaskAttack();
							timer = 0f;
						}
					}
					else timerBetweenActions -= Time.deltaTime;

					break;

				case 2:
					if (timerBetweenActions <= 0)
					{
						if (leftHand.GetComponent<HandManager>().isMoving == false)
						{
							leftHand.GetComponent<HandManager>().isMoving = true;
						}
						if (rightHand.GetComponent<HandManager>().isMoving == false)
						{
							rightHand.GetComponent<HandManager>().isMoving = true;
						}

						timer += Time.deltaTime;
						if (timer > aoeRate / multiplier)
						{
							BlueMaskAttack();
							timer = 0f;
						}
					}
					else timerBetweenActions -= Time.deltaTime;

					break;

				case 3:
					if (timerBetweenActions <= 0)
					{
						if (leftHand.GetComponent<HandManager>().isMoving == false)
						{
							leftHand.GetComponent<HandManager>().isMoving = true;
						}
						if (rightHand.GetComponent<HandManager>().isMoving == false)
						{
							rightHand.GetComponent<HandManager>().isMoving = true;
						}
						timer += Time.deltaTime;
						if (timer > spawnRate / multiplier)
						{
							GreenMaskAttack();
							timer = 0f;
						}
					}
					else timerBetweenActions -= Time.deltaTime;
					break;
			}

			if (actualLifePoints <= 0)
			{
				Instantiate(particleDeath, transform.position, transform.rotation);
				Destroy(this.gameObject);
			}
		}	
	}

	void GreenMaskAttack()
	{
		switch(enemyNumber)
		{
			case 1:
				GameObject mob1l = Instantiate(enemy1, leftHand.transform.position - Vector3.forward * 5, enemy1.transform.rotation) as GameObject;
				Instantiate(particleSpawn, leftHand.transform.position - Vector3.forward * 5, transform.rotation);
				mob1l.transform.parent = transform.parent;

				GameObject mob1r = Instantiate(enemy1, rightHand.transform.position - Vector3.forward * 5, enemy1.transform.rotation) as GameObject;
				Instantiate(particleSpawn, rightHand.transform.position - Vector3.forward * 5, transform.rotation);
				mob1r.transform.parent = transform.parent;

				enemyNumber++;

				break;

			case 2:
				GameObject mob2l = Instantiate(enemy2, leftHand.transform.position - Vector3.forward * 5, enemy2.transform.rotation) as GameObject;
				Instantiate(particleSpawn, leftHand.transform.position - Vector3.forward * 5, transform.rotation);
				mob2l.transform.parent = transform.parent;

				GameObject mob2r = Instantiate(enemy2, rightHand.transform.position - Vector3.forward * 5, enemy2.transform.rotation) as GameObject;
				Instantiate(particleSpawn, rightHand.transform.position - Vector3.forward * 5, transform.rotation);
				mob2r.transform.parent = transform.parent;

				enemyNumber++;
				break;
			case 3:
				GameObject mob3l = Instantiate(enemy3, leftHand.transform.position - Vector3.forward * 5, enemy3.transform.rotation) as GameObject;
				Instantiate(particleSpawn, leftHand.transform.position - Vector3.forward * 5, transform.rotation);
				mob3l.transform.parent = transform.parent;

				GameObject mob3r = Instantiate(enemy3, rightHand.transform.position - Vector3.forward * 5, enemy3.transform.rotation) as GameObject;
				mob3r.transform.parent = transform.parent;
				Instantiate(particleSpawn, rightHand.transform.position - Vector3.forward * 5, transform.rotation);
				enemyNumber = 1;
				break;
		}
      
		
	}
	void BlueMaskAttack()
	{
		GameObject player;
		player = GameObject.Find("Player");

		if(left)
		{
			Vector3 leftDir;
			leftDir = player.transform.position - leftHand.transform.position;
			GameObject lshot = ObjectPooler.current.GetObject(blueShot);
			lshot.SetActive(true);
			lshot.transform.position = leftHand.transform.position;
			lshot.transform.rotation = leftHand.transform.rotation;
			lshot.GetComponent<Rigidbody>().velocity = leftDir.normalized * shotSpeed;

			left = false;
		}
		else
		{
			
			Vector3 rightDir;
			rightDir = player.transform.position - rightHand.transform.position;
			GameObject rshot = ObjectPooler.current.GetObject(blueShot);
			rshot.SetActive(true);
			rshot.transform.position = rightHand.transform.position;
			rshot.transform.rotation = rightHand.transform.rotation;
			rshot.GetComponent<Rigidbody>().velocity = rightDir.normalized * shotSpeed;
			left = true;
		}
		

		
	}

	void RedMaskAttack()
	{
		GameObject player;
		player = GameObject.Find("Player");

		// Tir de la main droite
		Vector3 rightDir;
		rightDir = player.transform.position - rightHand.transform.position;
		GameObject rshot = ObjectPooler.current.GetObject(redShot);
		rshot.SetActive(true);
		rshot.transform.position = rightHand.transform.position;
		rshot.transform.rotation = rightHand.transform.rotation;
		rshot.GetComponent<Rigidbody>().velocity = rightDir.normalized * shotSpeed;

		// Tir de la main gauche
		Vector3 leftDir;
		leftDir = player.transform.position - leftHand.transform.position;
		GameObject lshot = ObjectPooler.current.GetObject(redShot);
		lshot.SetActive(true);
		lshot.transform.position = leftHand.transform.position;
		lshot.transform.rotation = leftHand.transform.rotation;
		lshot.GetComponent<Rigidbody>().velocity = leftDir.normalized * shotSpeed;
	}


	void MaskSwitcher()
	{
		switch (step)
		{
			case 1:
				if (isBlueAlive)
				{
					step = 2;
					mask.GetComponent<SpriteRenderer>().sprite = Resources.Load("BlueMask", typeof(Sprite)) as Sprite;
				}
				else if (isGreenAlive)
				{
					step = 3;
					mask.GetComponent<SpriteRenderer>().sprite = Resources.Load("GreenMask", typeof(Sprite)) as Sprite;
				}
				else step = 1;

				break;

			case 2:
				if (isGreenAlive)
				{
					step = 3;
					mask.GetComponent<SpriteRenderer>().sprite = Resources.Load("GreenMask", typeof(Sprite)) as Sprite;
				}
				else if (isRedAlive)
				{
					step = 1;
					mask.GetComponent<SpriteRenderer>().sprite = Resources.Load("RedMask", typeof(Sprite)) as Sprite;
				}
				else step = 2;

				break;

			case 3:
				if (isRedAlive)
				{
					step = 1;
					mask.GetComponent<SpriteRenderer>().sprite = Resources.Load("RedMask", typeof(Sprite)) as Sprite;
				}
				else if (isBlueAlive)
				{
					step = 2;
					mask.GetComponent<SpriteRenderer>().sprite = Resources.Load("BlueMask", typeof(Sprite)) as Sprite;
				}
				else step = 3;

				break;
		}
		changeTimer = 0f;
		Instantiate(particleSwitch, transform.position, transform.rotation);
		leftHand.GetComponent<HandManager>().isMoving = false;
		rightHand.GetComponent<HandManager>().isMoving = false;

	}
	void MaskManagement ()
	{
		if (actualLifeRed <= 0 && isRedAlive)
		{
			isRedAlive = false;
			multiplier += 0.5f;
			timerBetweenActions = timeBetweenActions;
			MaskSwitcher();
		}	
		if(actualLifeGreen <= 0 && isGreenAlive)
		{
			isGreenAlive = false;
			multiplier += 0.5f;
			timerBetweenActions = timeBetweenActions;
			MaskSwitcher();
		}
		if (actualLifeBlue <= 0 && isBlueAlive)
		{
			isBlueAlive = false;
			multiplier += 0.5f;
			timerBetweenActions = timeBetweenActions;
			MaskSwitcher();
		}

	}

	public IEnumerator damageBoss(int damage)
	{
		audioPlayer.PlayOneShot(ouch);
		//Debug.Log("Enemy is taking " + damage + " damage");
		mask.GetComponent<Renderer>().material.color = hitColor;
		yield return new WaitForSeconds(0.2f);
		mask.GetComponent<Renderer>().material.color = normalColor;

		actualLifePoints -= damage;
		switch(step)
		{
			case 1:
				actualLifeRed -= damage;
				MaskManagement();
				break;

			case 2:
				actualLifeBlue -= damage;
				MaskManagement();
				break;

			case 3:
				actualLifeGreen -= damage;
				MaskManagement();
				break;
		}
	}
}
