using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour {
	
	public enum PowerupType{
		Shield
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
		
		int distance = Random.Range (0, move.boundary);
		float xDir = Random.Range (0.0f,360.0f);
		float yDir = Random.Range (0.0f,360.0f);
		float zDir = Random.Range (0.0f,360.0f);
		
		Vector3 direction = new Vector3 (xDir,yDir,zDir);
		direction.Normalize();
		direction *= distance;
		
		transform.position = direction;
		
		if (type == PowerupType.Shield)
		{
			ShipHull playerHull = collided.GetComponent<ShipHull>();
			playerHull.ActivateShield();
		}
	}
}
