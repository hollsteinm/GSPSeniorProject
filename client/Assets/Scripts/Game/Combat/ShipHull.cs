using UnityEngine;
using System.Collections;

public class ShipHull : MonoBehaviour {
    private float hullHealth = 100.0f;
    private float maxHealth = 100.0f;
	
	private bool shielded = false;
	private float shieldTimer;
	
	private Texture2D shieldTex;

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
        shieldTex = Resources.Load("Textures/Shield") as Texture2D;
	}
	
	void OnGUI() {
        if (shielded)
		{
			if(!shieldTex)
			{
				Debug.LogError("Assign a Texture in the inspector.");
				return;
			}
			GUI.DrawTexture(new Rect(60,10,80,30), shieldTex, ScaleMode.ScaleToFit, true, 0);
		}
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
	
		if (shielded)
		{
			shieldTimer -= Time.deltaTime;
			if (shieldTimer < 0)
				shielded = false;
		}
	}

    private bool IsDead() {
        return hullHealth <= 0.0f;
    }

    private void OnDeath() {
        deathPrefab.GetComponent<PlayerDeath>().isClient = true;
        Instantiate(deathPrefab);
        GameManager.gameManager.ClientController.Send(DataType.DEATH, new object());
        Destroy(gameObject);
        //Application.LoadLevel("lobby");
    }

    public void OnHit(float damage, Vector3 contactPoint) {
        Instantiate(Resources.Load("HitPrefab"), contactPoint, transform.rotation);
		if (shielded)
			damage = 0;
        hullHealth -= damage;
        onHit.Play();
    }
	
	public void ActivateShield()
	{
		shielded = true;
		shieldTimer = 30;
	}
}