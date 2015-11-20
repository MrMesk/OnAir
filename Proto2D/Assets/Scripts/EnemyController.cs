﻿using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    /* Variable pour les distances */
    public float distanceVision;
    public float distanceAttaque;
    public float fourchette;

    // Variable pour l'agent de l'ennemi et le GameObject joueur.
    private NavMeshAgent _agent;
    private GameObject _player;

    // Un putain de timer magique parce qu'Unity est tout pourri pour suivre des ordres dans le temps
    private float timer;

    // Timer pour le temps avant d'arrêter de suivre le joueur
    public float timeFollowingPlayer;
    private float timerFollow;

    /* Variable pour le Roam */
    public Vector3 controlPoint;
    public float RoamRadius;
    private Vector3 dest;

    /* Variable pour l'attaque */
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
        Dodge,
        Move
    }
    private AttackStates actStates;
    private AttackStates nextStates;


    // Use this for initialization
    void Start () {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
        transform.position = controlPoint;

        states = EnemyStates.Roam;
        actStates = AttackStates.Wait;

        shooting = false;
        moving = false;
	}
	

	// Update is called once per frame
	void Update () {
	    switch(states)
        {
            case EnemyStates.Roam:
                Debug.Log("L'IA est en mode roaming.");
                Roam();
                break;


            case EnemyStates.Attack:
                Attack();
                break;
        }
	}


    // Fonction qui gere le roaming de l'ennemi
    void Roam()
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
                Debug.Log("wait");
                _agent.SetDestination(transform.position);
                transform.LookAt(_player.transform.position);
                timer += Time.deltaTime;
                if (timer > timeToAction)
                {
                    timer = 0;
                    actStates = nextStates;
                }
                break;

            case AttackStates.GetCloser:    /* L'ennemi se rapproche */
                Debug.Log("get closer");
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
                Debug.Log("shoot");
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
                Debug.Log("Move");
                _agent.stoppingDistance = 0;
                if (!moving)
                {
                    dest = transform.position + (Random.insideUnitSphere * RoamRadius);
                    NavMeshHit hitRoam;
                    if (NavMesh.SamplePosition(dest, out hitRoam, 1.5f, NavMesh.AllAreas))
                    {
                        _agent.SetDestination(hitRoam.position);
                        moving = true;
                    }
                }

                if (Vector3.Distance(_agent.transform.position, _agent.destination) < 1.5f && moving)
                {
                    moving = false;
                    actStates = AttackStates.GetCloser;
                }
                break;

            case AttackStates.Dodge:
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


    void Tir (int nbTir)
    {
        dirJoueurEnemy = _player.transform.position - transform.position;
        shotDir = dirJoueurEnemy.normalized;

        if (!shooting)
        {
            shotDone = 0;
            shooting = true;
        }

       if (shotDone < nbTir)
        {
            if (timer > shotInterval && shooting)
            {
                Debug.Log("bang!");
                GameObject clone = Instantiate(shot, transform.position, transform.rotation) as GameObject;
                clone.GetComponent<Rigidbody>().AddForce(shotDir * shotSpeed);
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