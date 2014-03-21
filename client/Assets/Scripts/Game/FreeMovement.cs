using UnityEngine;
using System.Collections;

public class FreeMovement : MonoBehaviour {
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    public float lookSpeed = 10.0f;
    private float moveSpeed = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        rotationX += Input.GetAxis("Mouse X") * lookSpeed;
        rotationY += Input.GetAxis("Mouse Y") * lookSpeed;
        rotationY = Mathf.Clamp(rotationY, -360, 360);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        moveSpeed += Input.GetAxis("Vertical");

        transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;
        transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal");
	}
}
