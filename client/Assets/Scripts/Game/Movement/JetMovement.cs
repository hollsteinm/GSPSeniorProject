using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AerialManeuvers))]
public class JetMovement : MonoBehaviour {
	
	private float rotationX = 0.0f;
    private float rotationY = 0.0f;
	private float rotationZ = 0.0f;

    public float lookSpeed = 1.0f;

    private float forwardAcceleration = 0.0f;
    public float maxForwardAcceleration = 200.0f;
	public float forwardAccelerationReaction = 7.5f;

    private float forwardVelocity;
	public float defaultForwardVelocity = 200.0f;
	public float forwardVelocityDeviation = 100.0f;
	
	private float horizontalAcceleration = 0.0f;
	public float maxHorizontalAcceleration = 30.0f;
	public float horizontalAccelerationReaction = 15.0f;
	
	private float horizontalVelocity = 0.0f;
	public float horizontalVelocityDeviation = 20.0f;
	
	//Used for aerial maneuvers
	private bool usingManeuver = false;
	private AerialManeuvers.ManeuverType currentManeuver;
    private AerialManeuvers maneuver = new AerialManeuvers();
	
	// Use this for initialization
	void Start () {
		forwardVelocity = defaultForwardVelocity;
		rigidbody.freezeRotation = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButton(1))
		{
			if (!usingManeuver)
			{
				currentManeuver = maneuver.ChooseManeuver(transform);
				if (currentManeuver != AerialManeuvers.ManeuverType.NONE)
					usingManeuver = true;
			}
		}
		if (usingManeuver)
		{
			usingManeuver = maneuver.UpdateManeuver(currentManeuver,transform);
			if (!usingManeuver) 
				if (currentManeuver == AerialManeuvers.ManeuverType.U_TURN1)
				{
					usingManeuver = true;
					currentManeuver = AerialManeuvers.ManeuverType.U_TURN2;
				}
			transform.position += transform.forward * defaultForwardVelocity * Time.deltaTime;
		}
		else
		{
			//SHIP ROTATION
			
			float rotX = Input.GetAxis("Mouse X") * lookSpeed;
	        float rotY = Input.GetAxis("Mouse Y") * lookSpeed;
			
			transform.Rotate (Vector3.up * rotX);
			transform.Rotate (Vector3.left * rotY);
			
			//FORWARD THRUSTERS
			
			//If we are not boosting or braking, we should return to the
			//standard velocity by flipping the acceleration
			float thrust = Input.GetAxis("Vertical") * forwardAccelerationReaction;
			if (thrust == 0 && forwardAcceleration != 0)
			{
				if (forwardVelocity > defaultForwardVelocity)
					forwardAcceleration = -maxForwardAcceleration;
				else if (forwardVelocity < defaultForwardVelocity)
					forwardAcceleration = maxForwardAcceleration;
				else
					forwardAcceleration = 0;
			}
			//Otherwise we should increase the acceleration by the thrust amount
	        forwardAcceleration += thrust;
	        forwardAcceleration = Mathf.Clamp(forwardAcceleration, -maxForwardAcceleration, maxForwardAcceleration);
			
			//If we are not boosting or braking, we should check to see if the
			//velocity is going to reach the default velocity
			float deltaVelocity = forwardAcceleration * Time.deltaTime;
			if(thrust == 0)
			{
				if((forwardVelocity > defaultForwardVelocity && forwardVelocity + deltaVelocity < defaultForwardVelocity) ||
				   (forwardVelocity < defaultForwardVelocity && forwardVelocity + deltaVelocity > defaultForwardVelocity))
				{
					//If so, just change the values accordingly
					forwardVelocity = defaultForwardVelocity;
					deltaVelocity = 0;
				}				
			}
	        forwardVelocity += deltaVelocity;
			
			//Make sure the velocity does not exceed the max or min values
			forwardVelocity = Mathf.Clamp(forwardVelocity,
								   defaultForwardVelocity - forwardVelocityDeviation,
				                   defaultForwardVelocity + forwardVelocityDeviation);
			
			//SIDE THRUSTERS
			
			//If we are not using the side thrusters, we should return to zero
			//horizontal velocity by flipping the acceleration
			thrust = Input.GetAxis("Horizontal") * horizontalAccelerationReaction;
			if (thrust == 0 && horizontalAcceleration != 0)
			{
				if (horizontalVelocity > 0)
					horizontalAcceleration = -maxHorizontalAcceleration;
				else if (horizontalVelocity < 0)
					horizontalAcceleration = maxHorizontalAcceleration;
				else
					horizontalAcceleration = 0;
			}
			//Otherwise we should increase the acceleration by the thrust amount
	        horizontalAcceleration += thrust;
	        horizontalAcceleration = Mathf.Clamp(horizontalAcceleration, -maxHorizontalAcceleration, maxHorizontalAcceleration);
			
			//If we are not using the side thrusters, we should check to see if the
			//velocity is going to reach zero
			deltaVelocity = horizontalAcceleration * Time.deltaTime;
			if(thrust == 0)
			{
				if((horizontalVelocity > 0 && horizontalVelocity + deltaVelocity < 0) ||
				   (horizontalVelocity < 0 && horizontalVelocity + deltaVelocity > 0))
				{
					//If so, just change the values accordingly
					horizontalVelocity = 0;
					deltaVelocity = 0;
				}				
			}
	        horizontalVelocity += deltaVelocity;
			
			//Make sure the velocity does not exceed the max or min values
			horizontalVelocity = Mathf.Clamp(horizontalVelocity, -horizontalVelocityDeviation, horizontalVelocityDeviation);
	
	        transform.position += transform.forward * forwardVelocity * Time.deltaTime;
	        transform.position += transform.right * horizontalVelocity * Time.deltaTime;
		}
	}
}