using UnityEngine;
using System.Collections;

public class AnimatePlayer : MonoBehaviour
{
	private Animator animator;
	// Use this for initialization
	void Start()
	{
		animator = this.GetComponent<Animator>();
	}
	// Update is called once per frame
	void Update()
	{
		float v = Input.GetAxis("Vertical");
		float h = Input.GetAxis("Horizontal");
		ManageMovement(h, v);
	}
	void ManageMovement(float horizontal, float vertical)
	{

		if (horizontal != 0f || vertical != 0f)
		{
			animator.SetBool("moving", true); animateWalk(horizontal, vertical);
		}
		else
		{
			animator.SetBool("moving", false);
		}
	}
	void animateWalk(float h, float v)
	{
		if (animator)
		{
			if ((v > 0) && (v > h))
			{
				animator.SetInteger("direction", 1);
			}
			if ((h > 0) && (v < h))
			{
				animator.SetInteger("direction", 2);
			}
			if ((v < 0) && (v < h))
			{
				animator.SetInteger("direction", 3);
			}
			if ((h < 0) && (v > h))
			{
				animator.SetInteger("direction", 4);
			}
		}
	}
}
