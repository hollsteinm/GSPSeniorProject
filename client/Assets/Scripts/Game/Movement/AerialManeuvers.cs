using UnityEngine;
using System.Collections;

public class AerialManeuvers {
	
	public struct TransformStruct
	{
		public Vector3 position, rotation, up, right, forward;
		
		public static implicit operator TransformStruct (Transform trans)
		{
			TransformStruct newTrans;
			newTrans.position = trans.position;
			newTrans.rotation = trans.eulerAngles;
			newTrans.up = trans.up;
			newTrans.right = trans.right;
			newTrans.forward = trans.forward;
			return newTrans;
		}
	}
	
	//Holds an enumeration for the different types of maneuver
	public enum ManeuverType {NONE,LOOP,U_TURN1,U_TURN2,L_DODGE,R_DODGE};
	//Holds the starting and ending position/rotation
	private TransformStruct start,end;
	//Loops end where they begin. This will help make sure the loop happens.
	private int loopCount;
	//Range of comparison
	private float range = 0.05f;
	
	// Use this for initialization
	public AerialManeuvers()
	{
	}
	
	//Choose the Maneuver the ship will take
	public ManeuverType ChooseManeuver(Transform player)
	{
		if (Input.GetButton("LeftMan"))
		{
			//Save coordinates
			start = player;
			end = player;
			loopCount = 0;
			//Return Maneuver Type
			return ManeuverType.L_DODGE;
		}
		else if (Input.GetButton("RightMan"))
		{
			//Save coordinates
			start = player;
			end = player;
			loopCount = 0;
			//Return Maneuver Type
			return ManeuverType.R_DODGE;
		}
		else if (Input.GetButton("DownMan"))
		{
			//Save coordinates
			start = player;
			player.Rotate(Vector3.left * 180);
			end = player;
			player.Rotate(Vector3.left * 180);
			//Return Maneuver Type
			return ManeuverType.U_TURN1;
		}
		else if (Input.GetButton("UpMan"))
		{
			//Save coordinates
			start = player;
			end = player;
			loopCount = 0;
			//Return Maneuver Type
			return ManeuverType.LOOP;
		}
		else
			return ManeuverType.NONE;
	}
	
	//Perform an update depending on the maneuver
	//and return whether or not the maneuver will continue
	public bool UpdateManeuver(ManeuverType current, Transform player)
	{
		bool continueManeuver = false;
		
		if (current == ManeuverType.LOOP)
			continueManeuver = UpdateLoop(player);
		else if (current == ManeuverType.U_TURN1)
			continueManeuver = UpdateUTurn1(player);
		else if (current == ManeuverType.U_TURN2)
			continueManeuver = UpdateUTurn2(player);
		else if (current == ManeuverType.L_DODGE)
			continueManeuver = UpdateLDodge(player);
		else if (current == ManeuverType.R_DODGE)
			continueManeuver = UpdateRDodge(player);
		
		return continueManeuver;
	}
	
	public bool UpdateLoop(Transform player)
	{
		Vector3 currentRotation = player.eulerAngles;
		if (CompareVec (player.eulerAngles, end.rotation))
		{
			if (loopCount > 0)
			{
				player.eulerAngles = end.rotation;
				return false;
			}
			else
				loopCount++;
		}
		Vector3 rotateUp = Vector3.left * 1.5f;
		player.Rotate(rotateUp);
		return true;
	}
	
	public bool UpdateUTurn1(Transform player)
	{
		Vector3 currentRotation = player.eulerAngles;
		if (CompareVec (player.eulerAngles, end.rotation))
		{
			player.eulerAngles = end.rotation;
			
			//Set up coordinates for the second part of the U-Turn
			start = player;
			player.Rotate(Vector3.forward * 180);
			end = player;
			player.Rotate(Vector3.forward * 180);
			
			return false;
		}
		Vector3 rotateUp = Vector3.left * 1.5f;
		player.Rotate(rotateUp);
		return true;
	}
	public bool UpdateUTurn2(Transform player)
	{
		Vector3 currentRotation = player.eulerAngles;
		if (CompareVec (player.eulerAngles, end.rotation))
		{
			player.eulerAngles = end.rotation;
			return false;
		}
		Vector3 rotateSide = Vector3.forward * 3.0f;
		player.Rotate(rotateSide);
		return true;
	}
	
	public bool UpdateLDodge(Transform player)
	{
		Vector3 currentRotation = player.eulerAngles;
		if (CompareVec (player.eulerAngles, end.rotation))
		{
			if (loopCount > 0)
			{
				player.eulerAngles = end.rotation;
				return false;
			}
			else
				loopCount++;
		}
		Vector3 rotateSide = Vector3.forward * 10.0f;
		player.Rotate(rotateSide);
		player.position -= start.right * 5;
		return true;
	}
	
	public bool UpdateRDodge(Transform player)
	{
		Vector3 currentRotation = player.eulerAngles;
		if (CompareVec (player.eulerAngles, end.rotation))
		{
			if (loopCount > 0)
			{
				player.eulerAngles = end.rotation;
				return false;
			}
			else
				loopCount++;
		}
		Vector3 rotateSide = -Vector3.forward * 10.0f;
		player.Rotate(rotateSide);
		player.position += start.right * 5;
		return true;
	}
		
	public bool CompareVec(Vector3 start, Vector3 end)
	{
		if (start.x < end.x + range
		 && start.x > end.x - range)
		{
			if (start.y < end.y + range
			 && start.y > end.y - range)
			{
				if (start.z < end.z + range
				 && start.z > end.z - range)
				{
					return true;	
				}
			}	
		}
		return false;
	}
}