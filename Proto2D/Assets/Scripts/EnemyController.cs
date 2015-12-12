using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    /* Variable pour les distances */
    [Header("Distances")]
    public float distanceVision = 20;
    public float distanceAttaque = 10;
    public float fourchette = 2;
    public LayerMask visionMask;

    // Variable pour le declacement
    [Header("Deplacement et vitesse")]
    private GameObject _player;
    private Vector3 destination;
    public float shoulderMultiplier = 0.5f;
    public float rayDistance = 2.0f;
    public float turnSpeed = 30.0f;
    public float moveSpeed = 6.0f;

    private bool dontLook = false;

    Vector3 pos;

    private Rigidbody myRigidbody;
    private Vector3 desiredVelocity;

    /* Variable pour le Roam */
    [Header("Roam")]
    public float RoamRadius = 3;
    public float timeBetweenRoam = 1;
    private float timerRoam;
    private Vector3 controlPoint;


    //Timer
    private float timer;

    /* Variable pour l'attaque */
    [Header("Variables d'attaque")]
    public float timeToAction =1;
    public GameObject shot;
    public float shotSpeed = 40;
    public float shotInterval = 0.2f;
    private Vector3 dirJoueurEnemy;
    private Vector3 shotDir;
    private bool canShoot;
    public int nbShot = 3;
    private int shotDone;
    private bool shooting;


    private bool moving;
    public float distMove = 4;
    private bool isPaused;

    public float dodgeRate = 0.5f;
    public float dodgeDist = 5;
    public float dodgeForce = 10;
    public float dodgeAngle = 15;
    private bool isDodging;
    private Vector3 dir;


    public float dodgeCD = 1;
    private float timerDodge;

    /* State Machine de l'ennemi */
    public enum GlobalStates
    {
        Roam,
        Attack
    }
    private GlobalStates states;

    /* State Machine de l'ennemi en mode combat */
    public enum AttackStates
    {
        Wait,
        GetCloser,
        Shoot,
        Move
    }
    private AttackStates actStates;
    private AttackStates nextStates;

    //animator
    [Header("Variables pour l'animation")]
    public Animator animator;

    // Use this for initialization
    void Start()
    {
        _player = GameObject.Find("Player");
        myRigidbody = GetComponent<Rigidbody>();

        controlPoint = transform.position;
        destination = transform.position;

        states = GlobalStates.Roam;
        actStates = AttackStates.Wait;

        isPaused = false;

        animator.SetBool("Move", false);
        animator.SetInteger("Direction", 3);
    }




    // Update is called once per frame
    void Update()
    {
		if (GameObject.Find("Player") != null)
		{
			switch (states)
			{
				case GlobalStates.Roam:
					//Debug.Log("L'IA est en mode roaming.");
					Roam();
					break;


				case GlobalStates.Attack:
					Attack();
					break;
			}


			if (!isPaused && !shooting)
			{
				Moving();
			}
			else
			{
				desiredVelocity = Vector3.zero;
			}

			if (!isPaused && !shooting)
			{
				animator.SetBool("Move", true);
				Moving();
			}
			else
			{
				desiredVelocity = Vector3.zero;
				animator.SetBool("Move", false);
			}
		}
    }




    void FixedUpdate()
    {
        if (isDodging)
        {
            //Debug.Log("is dodging");
            myRigidbody.velocity = desiredVelocity + (dir * dodgeForce);
            isDodging = false;
        }
        else
        {
            myRigidbody.velocity = desiredVelocity;
        }

        if (myRigidbody.velocity.x == myRigidbody.velocity.z)
        {

        }
        else if (Mathf.Abs(myRigidbody.velocity.x) > Mathf.Abs(myRigidbody.velocity.z))
        {
            if (myRigidbody.velocity.x > 0)
            {
               // transform.localScale = new Vector3( 0.7f, 1.57f, 0.7f);
                animator.SetInteger("Direction", 2);
            }
            else
            {
                //transform.localScale = new Vector3(-0.7f, 1.57f, 0.7f);
                animator.SetInteger("Direction", 4);
            }
        }
        else
        {
            if (myRigidbody.velocity.z > 0)
            {
                //transform.localScale = new Vector3(0.7f, 1.57f, 0.7f);
                animator.SetInteger("Direction", 1);
            }
            else
            {
                //transform.localScale = new Vector3(0.7f, 1.57f, 0.7f);
                animator.SetInteger("Direction", 3);
            }
        }
    }




    void Moving()
    {
        Vector3 lookDirection = (destination - transform.position).normalized;

        RaycastHit hit;

        Vector3 leftRayPos = transform.position - (transform.right * shoulderMultiplier);
        Vector3 rightRayPos = transform.position + (transform.right * shoulderMultiplier);

        if (Physics.Raycast(leftRayPos, transform.forward, out hit, rayDistance))
        {
            if (!dontLook && (hit.collider.tag == "Wall" || hit.collider.tag == "Hole"))
            {
                Debug.DrawRay(leftRayPos, hit.point, Color.red);
                lookDirection += hit.normal * 50.0f;
            }

        }
        else if (Physics.Raycast(rightRayPos, transform.forward, out hit, rayDistance))
        {
            if (!dontLook && (hit.collider.tag == "Wall" || hit.collider.tag == "Hole"))
            {
                Debug.DrawRay(leftRayPos, hit.point, Color.red);
                lookDirection += hit.normal * 50.0f;
            }
        }
        else
        {
            Debug.DrawRay(leftRayPos, transform.forward * rayDistance, Color.yellow);
            Debug.DrawRay(rightRayPos, transform.forward * rayDistance, Color.yellow);
        }

        Quaternion lookRot = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, turnSpeed * Time.deltaTime);

        desiredVelocity = transform.forward * moveSpeed;
        desiredVelocity.y = myRigidbody.velocity.y;
    }


    bool IsFree(Vector3 point)
    {
        RaycastHit hit;
        if (Physics.Raycast(point, Vector3.up, out hit, 5))
        {
            //Debug.Log(hit.collider.name);
            //Debug.Log(point + " isn't free");
            return false;
        }
        else
        {
            //Debug.Log(point + " is free");
            return true;
        }
    }


    // Fonction qui gere le roaming de l'ennemi
    void Roam()
    {

        if ((Vector3.Distance(transform.position, destination) < 0.7f))
        {
            isPaused = true;
            if (timerRoam > timeBetweenRoam)
            {
                Vector3 point = Random.insideUnitSphere;
                point.y = 0;
                if (IsFree(controlPoint + (point.normalized * RoamRadius)))
                {
                    destination = controlPoint + (point.normalized * RoamRadius);
                    timerRoam = 0;
                    isPaused = false;
                    dontLook = false;
                }
            }
            timerRoam += Time.deltaTime;
        }


        Collider[] targets = Physics.OverlapSphere(this.transform.position, distanceVision);

        foreach (Collider target in targets)
        {
            if (target.name == "Player")
            {
                Vector3 direction = _player.transform.position - transform.position;

                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction.normalized, out hit, distanceVision))
                {
                    if (hit.collider.gameObject == _player)
                    {
                        states = GlobalStates.Attack;
                        actStates = AttackStates.Wait;
                        nextStates = AttackStates.GetCloser;
                    }
                }

            }
        }
    }


    // Fonction qui gere l'attaque de l'ennemi
    void Attack()
    {
        switch (actStates)
        {
            case AttackStates.Wait:     /* L'ennemi est en etat d'attente */
                //Debug.Log("wait");
                destination = transform.position;
                isPaused = true;
                //transform.LookAt(_player.transform.position);
                timer += Time.deltaTime;
                if (timer > timeToAction)
                {
                    timer = 0;
                    isPaused = false;
                    actStates = nextStates;
                }
                break;

            case AttackStates.GetCloser:    /* L'ennemi se rapproche */
                //Debug.Log("get closer");
                destination = _player.transform.position;

                Collider[] targets = Physics.OverlapSphere(this.transform.position, distanceAttaque + Random.Range(-fourchette, fourchette));
                foreach (Collider target in targets)
                {
                    if (target.name == "Player")
                    {
                        nextStates = AttackStates.Shoot;
                        actStates = AttackStates.Wait;
                    }
                }
                break;

            case AttackStates.Shoot:    /* L'ennemi attaque si il peut */
                //Debug.Log("shoot");
                if (VisionOk())
                {
                    Tir(nbShot);
                }

                if (!shooting)
                {
                    nextStates = AttackStates.Move;
                    actStates = AttackStates.Wait;
                }

                break;

            case AttackStates.Move:
                //Debug.Log("Move");
                //Debug.Log(moving);

                if (!moving)
                {
                    Vector3 dir = _player.transform.position - transform.position;
                    float tmp = dir.z;
                    dir.z = -dir.x;
                    dir.x = tmp;

                    float halfchoice = Random.Range(0.0f, 1.0f);
                    if (halfchoice < 0.5f)
                    {
                        dir = -dir;
                    }

                    if (IsFree(transform.position + (dir.normalized * distMove)))
                    {
                        destination = transform.position + (dir.normalized * distMove);
                        moving = true;
                    }

                }

                if (Vector3.Distance(transform.position, destination) < 0.7f && moving)
                {
                    moving = false;
                    actStates = AttackStates.GetCloser;
                    dontLook = false;
                }
                break;

        }

        Dodge();
    }


    /* Fonction pour savoir si l'ennemi peut tirer sur le joueur */
    bool VisionOk()
    {
        dirJoueurEnemy = _player.transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dirJoueurEnemy, out hit, 50.0f, visionMask))
        {
            if (hit.transform.gameObject.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }


    void Dodge()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, dodgeDist);
        foreach (Collider coll in colls)
        {
            if (coll.gameObject.tag == "PlayerBullet")
            {
                //Debug.Log("Bullet in area");
                if (Vector3.Angle(coll.GetComponent<Rigidbody>().velocity.normalized, (transform.position - coll.transform.position).normalized) <= dodgeAngle)
                {
                    //Debug.Log("in angle");

                    dir = coll.transform.position - transform.position;
                    float tmp = dir.z;
                    dir.z = -dir.x;
                    dir.x = tmp;

                    float halfchoice = Random.Range(0.0f, 1.0f);
                    if (halfchoice < 0.5f)
                    {
                        dir = -dir;
                    }


                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, dir, out hit, dodgeForce / 2))
                    {
                        //Debug.Log("plop");
                        dir = -dir;
                    }

                    float changeDodge = Random.Range(0.0f, 1.0f);
                    if (changeDodge < dodgeRate && timerDodge > dodgeCD)
                    {
                        //Debug.Log("dodge ?");
                        isDodging = true;
                        timerDodge = 0;
                    }
                }
            }
        }
        timerDodge += Time.deltaTime;
    }

    
    void Tir (int nbTir)
    {
        if (!shooting)
        {
            shotDone = 0;
            shooting = true;
            dirJoueurEnemy = _player.transform.position - transform.position;
            shotDir = dirJoueurEnemy.normalized;
        }

       if (shotDone < nbTir)
        {
            if (timer > shotInterval && shooting)
            {
                //Debug.Log("bang!");
                //GameObject clone = Instantiate(shot, transform.position, transform.rotation) as GameObject;
               // clone.GetComponent<Rigidbody>().AddForce(shotDir * shotSpeed);

				GameObject clone = ObjectPooler.current.GetObject(shot);
				clone.SetActive(true);
				clone.transform.position = transform.position;
				clone.transform.rotation = transform.rotation;
				clone.GetComponent<Rigidbody>().velocity = (shotDir * shotSpeed);

				shotDone++;
                timer = 0;
            }
            timer += Time.deltaTime;
        }

        if (shooting && shotDone == nbTir)
        {
            shooting = false;
            nextStates = AttackStates.Move;
            actStates = AttackStates.Wait;
        }
    }
}
