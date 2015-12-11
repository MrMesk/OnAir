using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    /* Variable pour les distances */
    [Header("Distances")]
    public float distanceVision;
    public float distanceAttaque;
    public float fourchette;

    // Variable pour le declacement
    [Header("Deplacement et vitesse")]
    private GameObject _player;
    private Vector3 destination;
    public float shoulderMultiplier = 0.5f;
    public float rayDistance = 5.0f;
    public float turnSpeed = 6.0f;
    public float moveSpeed = 6.0f;

    Vector3 pos;

    private Rigidbody myRigidbody;
    private Vector3 desiredVelocity;

    /* Variable pour le Roam */
    [Header("Roam")]
    public float RoamRadius;
    public float timeBetweenRoam;
    private float timerRoam;
    private Vector3 controlPoint;


    // Un putain de timer magique parce qu'Unity est tout pourri pour suivre des ordres dans le temps
    private float timer;

    // Timer pour le temps avant d'arrêter de suivre le joueur
    [Header("Temps de poursuite")]
    public float timeFollowingPlayer;
    private float timerFollow;



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
    private bool isPaused;

    public float dodgeRate;
    public float dodgeDist;
    public float dodgeForce;
    public float dodgeAngle = 15f;
    private bool isDodging;
    private Vector3 dir;


    public float dodgeCD;
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
    }




    // Update is called once per frame
    void Update()
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

        pos = transform.position;
        pos.y = 0f;
        transform.position = pos;

        if (!isPaused && !shooting)
        {
            Moving();
        }
        else
        {
            desiredVelocity = Vector3.zero;
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
    }




    void Moving()
    {
        Vector3 lookDirection = (destination - transform.position).normalized;

        RaycastHit hit;

        Vector3 leftRayPos = transform.position - (transform.right * shoulderMultiplier);
        Vector3 rightRayPos = transform.position + (transform.right * shoulderMultiplier);

        if (Physics.Raycast(leftRayPos, transform.forward, out hit, rayDistance))
        {
            if (hit.collider.tag == "Wall" || hit.collider.tag == "Hole")
            {
                Debug.DrawRay(leftRayPos, hit.point, Color.red);
                lookDirection += hit.normal * 20.0f;
            }

        }
        else if (Physics.Raycast(rightRayPos, transform.forward, out hit, rayDistance))
        {
            if (hit.collider.tag == "Wall" || hit.collider.tag == "Hole")
            {
                Debug.DrawRay(leftRayPos, hit.point, Color.red);
                lookDirection += hit.normal * 20.0f;
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
                destination = controlPoint + (point.normalized * RoamRadius);
                timerRoam = 0;
                isPaused = false;
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


                    destination = transform.position + (dir.normalized * distMove);
                    moving = true;

                }

                if (Vector3.Distance(transform.position, destination) < 0.7f && moving)
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
                isDodging = false;
                isPaused = false;
                shooting = false;
                states = GlobalStates.Roam;
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
    bool VisionOk()
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
}
