using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour
{
    public float maxZoom;
    public float minZoom;
    public float sensivity;

    public float dragSpeed = 2;
    private Vector3 dragOrigin;

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            if (Camera.main.orthographicSize < maxZoom)
            {
                Camera.main.orthographicSize += sensivity;
            }
            else Camera.main.orthographicSize = maxZoom;

        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            if (Camera.main.orthographicSize > minZoom)
            {
                Camera.main.orthographicSize -= sensivity;
            }
            else Camera.main.orthographicSize = minZoom;

        }

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed * -1, 0, pos.y * dragSpeed * -1);

        transform.Translate(move, Space.World);
    }
}
