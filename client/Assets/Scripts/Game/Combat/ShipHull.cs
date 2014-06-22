using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipHull : MonoBehaviour {
    public float hullHealth = 100.0f;
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
        get {
			return maxHealth;
		}
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
        GameObject o = Instantiate(deathPrefab, transform.position, transform.rotation) as GameObject;
        o.GetComponent<PlayerDeath>().isClient = true;
        o.GetComponent<PlayerDeath>().AssignFocus(gameObject.GetComponent<ClientPlayer>().hitByPlayerLast);
        GameManager.gameManager.ClientController.Send(DataType.DEATH, new Dictionary<string, int>());
        deathSound.Play();
        Destroy(transform.root.gameObject);
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
        GameObject shieldPrefab = transform.FindChild("ShieldEffectPrefab(Clone)").gameObject;
        shieldPrefab.SetActive(true);
        ShieldEffect se = shieldPrefab.GetComponent<ShieldEffect>();

        se.effectTime = 30.0f;

		shielded = true;
		shieldTimer = 30;
	}
}