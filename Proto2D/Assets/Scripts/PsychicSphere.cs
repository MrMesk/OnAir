using UnityEngine;
using System.Collections;

public class PsychicSphere : MonoBehaviour
{
    Ray ray;
    public float speed = 5f;
    public float hitRate = 0.2f;
    public int damage = 2;
    float timer;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 destination = Vector3.MoveTowards(transform.position, hit.point, speed * Time.deltaTime);
            destination.y = 0f;
            transform.position = destination;

            
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy") other.gameObject.GetComponent<EnemyController>().StartCoroutine(other.gameObject.GetComponent<EnemyController>().damage(damage));
    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            timer += Time.deltaTime;
            if (timer >= hitRate)
            {
                other.gameObject.GetComponent<EnemyController>().StartCoroutine(other.gameObject.GetComponent<EnemyController>().damage(damage));
                timer = 0f;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy") timer = 0f;
    }
}
