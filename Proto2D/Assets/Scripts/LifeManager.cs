using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class LifeManager : MonoBehaviour
{
	public GameObject particleDeath;
	public int lifePoints;
	public AudioClip ouch;
	public AudioClip healSound;

	AudioSource audioPlayer;
	int remainingLife;
	Color normalColor;


	public Color hitColor = Color.red;
	public GameObject skin;
	// Use this for initialization
	void Start ()
	{
		audioPlayer = GetComponent<AudioSource>();
		remainingLife = lifePoints;
		normalColor = skin.GetComponent<Renderer>().material.color;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (remainingLife <= 0)
		{
			Instantiate(particleDeath, transform.position, transform.rotation);
			Destroy(this.gameObject);
		}
	}
	public IEnumerator damage(int dmg)
	{
		audioPlayer.PlayOneShot(ouch);
		//Debug.Log("Enemy is taking " + dmg + " damage");
		skin.GetComponent<Renderer>().material.color = hitColor;
		yield return new WaitForSeconds(0.2f);
		skin.GetComponent<Renderer>().material.color = normalColor;
		
		remainingLife -= dmg;
		
	}
	public IEnumerator heal(int amount)
	{
		audioPlayer.PlayOneShot(healSound);
		skin.GetComponent<Renderer>().material.color = Color.green;
		yield return new WaitForSeconds(0.2f);
		skin.GetComponent<Renderer>().material.color = normalColor;
		remainingLife += amount;
	}
}
