using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour
{
	public Texture2D newCursor;
	public CursorMode cursorAuto = CursorMode.Auto;
	// Use this for initialization
	void Start ()
	{
		Cursor.SetCursor(newCursor,Vector2.zero, cursorAuto); 
	}
}
