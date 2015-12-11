using UnityEngine;
using System.Collections;

public class PsychicSphere : MonoBehaviour
{
    Ray ray;
    public float speed = 5f;
    public float hitRate = 0.2f;
	public float maxRange = 40f;
    public int damage = 2;
	GameObject player;
    float timer;

	// Use this for initialization
	void Start ()
    {
		player = GameObject.Find("Player");
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
		if(Vector3.Distance(transform.position, player.transform.position) > maxRange)
		{
			transform.position = player.transform.position;
		}
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy") other.gameObject.GetComponent<LifeManager>().StartCoroutine(other.gameObject.GetComponent<LifeManager>().damage(damage));
    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            timer += Time.deltaTime;
            if (timer >= hitRate)
            {
                other.gameObject.GetComponent<LifeManager>().StartCoroutine(other.gameObject.GetComponent<LifeManager>().damage(damage));
                timer = 0f;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy") timer = 0f;
    }
}
