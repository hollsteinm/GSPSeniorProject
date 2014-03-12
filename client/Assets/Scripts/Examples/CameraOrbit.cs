using UnityEngine;
using System.Collections;

public class CameraOrbit : MonoBehaviour {
	private float x, y;
	public Transform target;
	private float camDistance;
    private Vector3 camOffset;

	private float xSpeed = 250.0f;
	private float ySpeed = 120.0f;
	
	// Use this for initialization
	void Start () {
    	Vector3 angles = transform.eulerAngles;
    	x = angles.y;
    	y = angles.x;
		
        if (target) {
            camOffset = transform.position - target.position;
            camDistance = camOffset.magnitude;
        }

        SetCameraPosition();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1") && target)
        {

            x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;

            // y = ClampAngle(y, yMinLimit, yMaxLimit);

            SetCameraPosition();
        }		
	}
	

    void SetCameraPosition() {
        Quaternion camRotation = Quaternion.Euler(y, x, 0);
        Vector3 distVector = new Vector3(0.0f, 0.0f, -camDistance);
        Vector3 camPosition = (camRotation * (distVector + (Vector3.right * camOffset.x))) + target.position;

        transform.rotation = camRotation;
        transform.position = camPosition;        
    }
}
