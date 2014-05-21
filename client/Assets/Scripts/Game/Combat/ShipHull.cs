using UnityEngine;
using System.Collections;

public class ShipHull : MonoBehaviour {
    private float hullHealth = 100.0f;
    private float maxHealth = 100.0f;

    public Gun weapon;
    public GameObject deathPrefab;
    public AudioSource deathSound;
    public AudioSource onHit;

    public GameObject fireDamage;
    public GameObject smokeDamage;

    public float Health {
        get {
            return hullHealth;
        }
        set {
            hullHealth = value;
        }
    }

    public float MaxHealth {
        set {
            maxHealth = value;
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

        if ((hullHealth / maxHealth) <= 0.3f) {
            fireDamage.SetActive(true);
        }

        if ((hullHealth / maxHealth) <= 0.15f) {
            smokeDamage.SetActive(true);
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