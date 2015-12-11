using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class GoToBoss : MonoBehaviour
{
	GameObject toBossRoom;
	GameObject cameraMain;
	public Texture2D blackScreen; // add a black texture here
	public AudioClip warp;
	AudioSource audioPlayer;
	public float fadeTime; // how long you want it to fade?

	private bool fadeIn; // false for fade out
	private Color color = Color.black;
	private float timer;

	public void FadeIn()
	{
		timer = fadeTime;
		fadeIn = true;
	}

	public void FadeOut()
	{
		timer = fadeTime;
		fadeIn = false;
	}

	protected void Start()
	{
		toBossRoom = GameObject.Find("ToBossRoom");
		audioPlayer = GetComponent<AudioSource>();
		FadeIn();
	}

	protected void OnGUI()
	{
		if (fadeIn)
		{
			color.a = timer / fadeTime;
		}
		else
		{
			color.a = 1 - (timer / fadeTime);
		}

		GUI.color = color;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackScreen);
	}

	protected void Update()
	{
		timer -= Time.deltaTime;

		if (timer <= 0)
		{
			timer = 0;
		}
	}

	public IEnumerator FadeOnTp(GameObject player, GameObject camera)
	{
		FadeOut();
		audioPlayer.PlayOneShot(warp);
		yield return new WaitForSeconds(fadeTime);

		player.transform.position = toBossRoom.transform.position;
		Vector3 camPos;
		camPos = toBossRoom.transform.position;
		camPos.y = 10;
		camera.transform.position = camPos;

		yield return new WaitForSeconds(fadeTime);

		FadeIn();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			StartCoroutine(FadeOnTp(other.gameObject, GameObject.Find("Main Camera")));
		}
	}
	void OnTriggerStay(Collider other)
	{
		if(other.tag == "Wall" || other.tag == "Hole" || other.tag == "Enemy")
		{
			Destroy(other.gameObject);
		}
	}
}
