using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour {
	
	public enum PowerupType{
		SHIELD,
		RAPID_FIRE,
		INFINITE_ENERGY
	}
	
	public PowerupType type;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0,1,0);
	}
	
	void OnTriggerEnter(Collider other)
	{
		GameObject collided = other.transform.root.gameObject;
		JetMovement move = collided.GetComponent<JetMovement>();
		
		if (move)
		{
			Transport(move);
		
			if (type == PowerupType.SHIELD)
			{
				ShipHull playerHull = collided.GetComponent<ShipHull>();
				playerHull.ActivateShield();
			}
			else if (type == PowerupType.RAPID_FIRE)
			{
				Gun playerGun = collided.GetComponent<Gun>();
				playerGun.ActivateRapid();
			}
			else if (type == PowerupType.INFINITE_ENERGY)
			{
				move.ActivateInfiniteEnergy();
			}
		}
	}
	
	void Transport(JetMovement move)
	{
		int distance = Random.Range (0, move.boundary);
		float xDir = Random.Range (0.0f,360.0f);
		float yDir = Random.Range (0.0f,360.0f);
		float zDir = Random.Range (0.0f,360.0f);
		
		Vector3 direction = new Vector3 (xDir,yDir,zDir);
		direction.Normalize();
		direction *= distance;
		
		transform.position = direction;
	}
}
