using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
<<<<<<< HEAD
  
	public float dodgeAngle = 15f;

=======
>>>>>>> origin/master
    /* Variable pour les distances */
    [Header("Distances")]
    public float distanceVision;
    public float distanceAttaque;
    public float fourchette;

    // Variable pour l'agent de l'ennemi et le GameObject joueur.
    private NavMeshAgent _agent;
    private GameObject _player;

    // Un putain de timer magique parce qu'Unity est tout pourri pour suivre des ordres dans le temps
    private float timer;

    // Timer pour le temps avant d'arrêter de suivre le joueur
    [Header("Temps de poursuite")]
    public float timeFollowingPlayer;
    private float timerFollow;

    /* Variable pour le Roam */
    [Header("Variables de Roam")]
    public Vector3 controlPoint;
    public float RoamRadius;
    private Vector3 dest;
    public float timeBetweenRoam;
    private float timerRoam;

    /* Variable pour l'attaque */
    [Header("Variables d'attaque")]
    public float timeToAction;
    public GameObject shot;
    public float shotSpeed;
    public float shotInterval;
    private Vector3 dirJoueurEnemy;
    private Vector3 shotDir;
    private bool canShoot;
    public int nbShot;
    private int shotDone;
    private bool shooting;
    private bool moving;
    public float distMove;

	public float dodgeRate;
	public float dodgeDist;
	public float dodgeForce;
    public float dodgeAngle = 15f;

    public float dodgeCD;
	private float timerDodge;

    /* State Machine de l'ennemi */
    public enum EnemyStates
    {
        Roam,
        Attack
    }
    private EnemyStates states;

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
	Vector3 pos;

    // Use this for initialization
    void Start ()
    {
<<<<<<< HEAD
        

=======
>>>>>>> origin/master
		_agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        controlPoint = transform.position;

        states = EnemyStates.Roam;
        actStates = AttackStates.Wait;

        shooting = false;
        moving = false;
	}
	

	// Update is called once per frame
	void Update ()
	{
	    switch(states)
        {
            case EnemyStates.Roam:
                //Debug.Log("L'IA est en mode roaming.");
                Roam();
                break;


            case EnemyStates.Attack:
                Attack();
                break;
        }
<<<<<<< HEAD
		
=======
>>>>>>> origin/master
		pos = transform.position;
		pos.y = 0f;
		transform.position = pos;
	
	}


    // Fonction qui gere le roaming de l'ennemi
    void Roam()
    {
        _agent.stoppingDistance = 0;

        if (timerRoam > timeBetweenRoam)
        {
            if ((Vector3.Distance(_agent.transform.position, _agent.destination) < 1.5f))
            {
                dest = controlPoint + (Random.insideUnitSphere * RoamRadius);
                NavMeshHit hitRoam;
                if (NavMesh.SamplePosition(dest, out hitRoam, 1.5f, NavMesh.AllAreas))
                {
                    _agent.SetDestination(hitRoam.position);
                    timerRoam = 0;
                }
            }
        }

        timerRoam += Time.deltaTime;

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
                        states = EnemyStates.Attack;
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
        switch(actStates)
        {
            case AttackStates.Wait:     /* L'ennemi est en etat d'attente */
                //Debug.Log("wait");
                _agent.SetDestination(transform.position);
                //transform.LookAt(_player.transform.position);
                timer += Time.deltaTime;
                if (timer > timeToAction)
                {
                    timer = 0;
                    actStates = nextStates;
                }
                break;

            case AttackStates.GetCloser:    /* L'ennemi se rapproche */
                //Debug.Log("get closer");
                NavMeshHit hitAttack;
                if (NavMesh.SamplePosition(_player.transform.position, out hitAttack, 1.5f, NavMesh.AllAreas))
                {
                    _agent.SetDestination(hitAttack.position);
                    
                }

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
                if(VisionOk())
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
				//Debug.Log(_agent.destination);
				//Debug.Log(_agent.remainingDistance);
				//Debug.Log(moving);
                _agent.stoppingDistance = 0;
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

                   
                    _agent.SetDestination(transform.position + (dir.normalized * distMove));
                    moving = true;

                }

                if (Vector3.Distance(transform.position, _agent.destination) < 1.2f && moving)
                {
                    moving = false;
                    actStates = AttackStates.GetCloser;
                }
                break;
				
        }


        if (Vector3.Distance(transform.position, _player.transform.position) > distanceVision)
        {
            timerFollow += Time.deltaTime;
            if (timerFollow > timeFollowingPlayer)
            {
                states = EnemyStates.Roam;
                actStates = AttackStates.Wait;
            }
        }
        else
        {
            timerFollow = 0;
        }

        Dodge();
	}


    /* Fonction pour savoir si l'ennemi peut tirer sur le joueur */
    bool VisionOk ()
    {
        dirJoueurEnemy = _player.transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dirJoueurEnemy, out hit))
        {
            if (hit.transform.gameObject.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }


    void Dodge ()
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

                    Vector3 dir = coll.transform.position - transform.position;
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
                        Debug.Log("plop");
                        dir = -dir;
                    }

                    float changeDodge = Random.Range(0.0f, 1.0f);
                    if (changeDodge < dodgeRate && timerDodge > dodgeCD)
                    {
                        //Debug.Log("Avoid");
                        transform.GetComponent<Rigidbody>().velocity = dir * dodgeForce;
                        timerDodge = 0;
                        timerFollow = 0;
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

<<<<<<< HEAD
    
=======
>>>>>>> origin/master

	void OnDrawGizmos ()
	{
		//Gizmos.color = Color.red;
		//Gizmos.DrawSphere(_agent.destination, 1.0f);

	}
}
