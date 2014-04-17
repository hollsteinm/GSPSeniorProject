using UnityEngine;
using System.Collections;

public class FreeMovement : MonoBehaviour {
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    public float lookSpeed = 0.0f;

    private float acceleration = 0.0f;
    public float maxAcceleration = 0.0f;

    private float velocity;

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

        acceleration += Input.GetAxis("Vertical");
        acceleration = Mathf.Clamp(acceleration, -maxAcceleration, maxAcceleration);
        velocity += acceleration * Time.deltaTime;

        transform.position += transform.forward * velocity * Time.deltaTime;
        transform.position += transform.right * velocity * Time.deltaTime * Input.GetAxis("Horizontal");
	}
}
