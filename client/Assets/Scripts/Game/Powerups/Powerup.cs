using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour {
	
	public enum PowerupType{
		SHIELD,
		RAPID_FIRE,
		INFINITE_ENERGY
	}
	
	public PowerupType type;
	
	public int numberOfInstances = 1;
	public Transform clone;
	public Transform player;
	public float timer = 30.0f;

	// Use this for initialization
	void Start () {
		if (numberOfInstances == 0)
			gameObject.SetActive(false);
		else if (numberOfInstances > 1)
		{
			for(int i = 1; i < numberOfInstances; i++)
			{
				Transform newObject = Instantiate(clone, clone.position, clone.rotation) as Transform;
				Powerup newPower = newObject.GetComponent<Powerup>();
				newPower.numberOfInstances = 1;
			}
		}
		JetMovement move = player.GetComponent<JetMovement>();
		Transport(move);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0,1,0);
		timer -= Time.deltaTime;
		if (timer <= 0)
		{
			JetMovement move = player.GetComponent<JetMovement>();
			Transport(move);
			timer = 30.0f;
		}
	}

    void SendPowerUpRequest(string typeAsString) {
        GameManager.gameManager.ClientController.Send(DataType.POWERUP, typeAsString);
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
            Debug.Log(type.ToString().ToLower());
            SendPowerUpRequest(type.ToString().ToLower());
            gameObject.audio.Play();
		}
	}
	
	void Transport(JetMovement move)
	{
		int distance = Random.Range (0, move.boundary);
		float xDir = Random.Range (-360.0f,360.0f);
		float yDir = Random.Range (-360.0f,360.0f);
		float zDir = Random.Range (-360.0f,360.0f);
		
		Vector3 direction = new Vector3 (xDir,yDir,zDir);
		direction.Normalize();
		direction *= distance;
		
		transform.position = direction;
	}
}
