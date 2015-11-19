using UnityEngine;
using System.Collections;

public class EnnemyController : MonoBehaviour {

    public float fieldOfViewAngle = 110f;
    public float distanceVision;

    public float distanceAttaque;
    public float fourchette;

    public float distanceMin;

    public float normalSpeed;
    public float fightingSpeed;

    public Vector3 controlPoint;
    public float RoamRadius;

    private Vector3 dest;

    private NavMeshAgent _agent;
    private GameObject _player;

    // shot parameter
    public GameObject shot;
    public float shotRate;
    public float shotCD;
    public float shotSpeed;
    private bool canShoot;
    private float shotTimer;
    private Vector3 shotDir;

    // dodge parameter
    public float dodgeRate;
    public Vector3 dodgeDir;
    private bool isDodging;

    public enum EnemyState
    {
        Roam,
        Attack
    }

    private EnemyState states;


    // Use this for initialization
    void Start () {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        transform.position = controlPoint;

        shotTimer = 0;
    }
	
	// Update is called once per frame
	void Update () {

        switch (states)
        {
            case EnemyState.Roam:
                Roam();
                break;


            case EnemyState.Attack:
                Attack();
                break;
        }

        PlayerInArea ();
    }

    /////  Gestion des timers
    void FixedUpdate ()
    {
        if (canShoot == false)
        {
            shotTimer += Time.deltaTime;
            if (shotTimer > shotCD)
            {
                canShoot = true;
                shotTimer = 0;
            }
        }
    }

    void PlayerInArea ()
    {
        Collider[] targets = Physics.OverlapSphere(this.transform.position, distanceVision);

        foreach (Collider target in targets)
        {
            if (target.name == "Player")
            {
                Vector3 direction = _player.transform.position - transform.position;
                float angle = Vector3.Angle(direction, transform.forward);

                if (angle < fieldOfViewAngle * 0.5f)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(transform.position, direction.normalized, out hit, distanceVision))
                    {
                        if (hit.collider.gameObject == _player)
                        {
                            states = EnemyState.Attack;
                        }
                    }
                }
            }
        }
    }

    void Roam ()
    {
        _agent.stoppingDistance = 0;
        if ((Vector3.Distance(_agent.transform.position, _agent.destination) < 1.5f))
        {
            dest = controlPoint + (Random.insideUnitSphere * RoamRadius);
            NavMeshHit hitRoam;
            if (NavMesh.SamplePosition(dest, out hitRoam, 1.5f, NavMesh.AllAreas))
            {
                _agent.SetDestination(hitRoam.position);
            }
        }
    }

    void Attack()
    {
        /////  Positionnement
        Collider[] targets = Physics.OverlapSphere(this.transform.position, distanceVision);
        foreach (Collider target in targets)
        {
            if (target.name == "Player")
            {
                NavMeshHit hitAttack;
                if (NavMesh.SamplePosition(_player.transform.position, out hitAttack, 1.5f, NavMesh.AllAreas))
                {
                    _agent.SetDestination(hitAttack.position);
                }
            }
        }
        _agent.stoppingDistance = distanceAttaque + Random.Range(-fourchette, fourchette);


        /////  Tir
        shotDir = _player.transform.position - transform.position;
        shotDir = shotDir.normalized;
        if (canShoot)
        {
            if (Random.Range(0,1.1f) < shotRate)
            {
                GameObject clone = Instantiate(shot, transform.position, transform.rotation) as GameObject;
                clone.GetComponent<Rigidbody>().AddForce(shotDir * shotSpeed);
                canShoot = false;
            }
        }


        /////   Esquive
        /*RaycastHit hit;
        if (Physics.Raycast(transform.position, shotDir, out hit))
        {
            if (Random.Range(0,1) < dodgeRate)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, shotDir, out hit))
                {

                }
            }
        }*/


        /////  Retour au roam
        if (Vector3.Distance(_player.transform.position, transform.position) > distanceVision)
        {
            states = EnemyState.Roam;
            _agent.SetDestination(controlPoint);
        }
    }
}
