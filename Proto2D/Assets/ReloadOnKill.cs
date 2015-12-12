using UnityEngine;
using System.Collections;

public class ReloadOnKill : MonoBehaviour
{

	ParticleSystem particles;
	// Use this for initialization
	void Start()
	{
		particles = GetComponent<ParticleSystem>();
	}

	// Update is called once per frame
	void Update ()
	{
		if (!particles.isPlaying)
		{
			GameObject.Find("GameManager").GetComponent<DungeonManager>().nbRooms += 3;
            Destroy(gameObject);
			Application.LoadLevel(1);
		}
	}
}
