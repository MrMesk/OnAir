using UnityEngine;
using System.Collections;

public class PlayerRIP : MonoBehaviour
{
	ParticleSystem particles;
	// Use this for initialization
	void Start()
	{
		particles = GetComponent<ParticleSystem>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!particles.isPlaying)
		{
			GameObject.Find("GameManager").GetComponent<DungeonManager>().nbRooms--;

			Destroy(GameObject.FindWithTag("MainUI"));
			Destroy(GameObject.FindWithTag("OptionsUI"));
			Destroy(gameObject);
			Application.LoadLevel(0);
		}
	}
}
