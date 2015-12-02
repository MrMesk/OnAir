using UnityEngine;
using System.Collections;

public class ParticleRIP : MonoBehaviour
{
	ParticleSystem particles;
	// Use this for initialization
	void Start ()
	{
		particles = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!particles.isPlaying)
		{
			Destroy(gameObject);
		}
	}
}
