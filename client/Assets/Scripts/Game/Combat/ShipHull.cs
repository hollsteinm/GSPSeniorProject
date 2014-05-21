using UnityEngine;
using System.Collections;

public class ShipHull : MonoBehaviour {
    private float hullHealth = 100.0f;

    public Gun[] weapons;
    public GameObject deathPrefab;
    public AudioSource deathSound;
    public AudioSource onHit;

    public float Health {
        get {
            return hullHealth;
        }
        set {
            hullHealth = value;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (IsDead()) {
            
            OnDeath();
        }
	
	}

    private bool IsDead() {
        return hullHealth <= 0.0f;
    }

    private void OnDeath() {
        Instantiate(deathPrefab);
        GameManager.gameManager.ClientController.Send(DataType.DEATH, new object());
        Destroy(gameObject);
        Application.LoadLevel("lobby");
    }

    public void OnHit(float damage, Vector3 contactPoint) {
        Instantiate(Resources.Load("HitPrefab"), contactPoint, transform.rotation);
        hullHealth -= damage;
        onHit.Play();
    }
}