using UnityEngine;
using System.Collections;

public class AerialManeuvers : MonoBehaviour {
	
	public enum ManeuverType {NONE,LOOP,U_TURN,L_DODGE,R_DODGE};
	private Transform start,end;
	
	// Use this for initialization
	void Start () {
	}
	
	public ManeuverType ChooseManeuver(Transform player)
	{
		if (Input.GetAxis("Vertical") > 0)
		{
			//Save coordinates
			start = player;
			end = player;
			//Return Maneuver Type
			return ManeuverType.LOOP;
		}
		else if (Input.GetAxis("Vertical") < 0)
		{
			//Save coordinates
			start = player;
			end = player;
			end.Rotate(-player.forward);
			//Return Maneuver Type
			return ManeuverType.U_TURN;
		}
		else if (Input.GetAxis("Horizontal") > 0)
		{
			//Save coordinates
			start = player;
			end = player;
			end.Translate(-player.right * 25);
			//Return Maneuver Type
			return ManeuverType.L_DODGE;
		}
		else if (Input.GetAxis("Horizontal") < 0)
		{
			//Save coordinates
			start = player;
			end = player;
			end.Translate(player.right * 25);
			//Return Maneuver Type
			return ManeuverType.R_DODGE;
		}
		else
			return ManeuverType.NONE;
	}
	
	public bool UpdateManeuver(ManeuverType current, Transform player)
	{
		return false;
	}
}
