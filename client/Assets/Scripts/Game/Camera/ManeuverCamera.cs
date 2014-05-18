using UnityEngine;
using System.Collections;

public class ManeuverCamera : MonoBehaviour{
	
	public JetMovement playerMovement;
	
	private AerialManeuvers.TransformStruct camStart, previousFrame, currentFrame;
	
	// The target we are following
	public Transform target;
	// The distance in the x-z plane to the target
	public float distance = 40.0f;
	// the height we want the camera to be above the target
	public float height = 10.0f;
	// the rate at which we want the camera to follow
	public float dampRate = 0.1f;
	//The rate the distance should be for regular maneuvers
	public float distanceChange = 3.0f;
	//The rate the dampening should change for regular maneuvers
	public float dampChange = 0.5f;
	//Bool to control camera
	private bool followingManeuver;
	
	// Use this for initialization
	void Start () {
		followingManeuver = false;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		// Early out if we don't have a target
		if (!target)
			return;
		
		if (playerMovement.GetUsingManeuver())
		{
			previousFrame = currentFrame;
			currentFrame = target;
			
			if (!followingManeuver)
			{
				camStart = transform;
				followingManeuver = true;
			}
			if (playerMovement.GetManeuverType() == AerialManeuvers.ManeuverType.LOOP ||
				playerMovement.GetManeuverType() == AerialManeuvers.ManeuverType.L_DODGE ||
				playerMovement.GetManeuverType() == AerialManeuvers.ManeuverType.R_DODGE ||
				playerMovement.GetManeuverType() == AerialManeuvers.ManeuverType.U_TURN1)
			{
				UpdateRegularManeuver();
			}
			else
				UpdateUTurn();
		}
		else
		{			
			followingManeuver = false;
			currentFrame = target;
			
			// Calculate the position the camera should be in
			Vector3 wantedHeight =  target.up * height;
			Vector3 wantedDistance = target.forward * -distance;
			
			// Set the position of the camera on the x-z plane to:
			// distance meters behind the target
			Vector3 wantedPosition = target.position + wantedHeight + wantedDistance;
			Vector3 cameraMoveVec = wantedPosition - transform.position;
			
			transform.position += cameraMoveVec * dampRate;
			
			// Always look at the target
			transform.LookAt (target, target.up);
		}
	}
	
	void UpdateRegularManeuver()
	{		
		// Calculate the position the camera should be in
		Vector3 wantedHeight =  camStart.up * height;
		Vector3 wantedDistance = camStart.forward * -distance * distanceChange;
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		Vector3 wantedPosition = target.position + wantedHeight + wantedDistance;
		Vector3 cameraMoveVec = wantedPosition - transform.position;
		
		transform.position += cameraMoveVec * dampRate * dampChange;
			
		// Always look at the target
		transform.LookAt (target);
	}
	
	void UpdateUTurn()
	{
		// Calculate the position the camera should be in
		Vector3 wantedHeight =  camStart.up * height;
		Vector3 wantedDistance = target.forward * -distance;
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		Vector3 wantedPosition = target.position + wantedHeight + wantedDistance;
		Vector3 cameraMoveVec = wantedPosition - transform.position;
		
		transform.position += cameraMoveVec * dampRate * dampChange;
			
		// Always look at the target
		transform.LookAt (target);
	}
}
