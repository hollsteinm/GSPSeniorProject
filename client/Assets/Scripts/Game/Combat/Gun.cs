using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {
    public GameObject ammunitionPrefab;
    public AudioSource shootingSound;
    public Transform muzzlePoint;

    private float cooldown = 0.5f;
    private float currentCooldown;
    
    private int clipSize = 100;
    private int currentAmmunition = 100;
    private int totalAmmunition = 100;

    //reserved for future use
    private string networkProjectileType;

    public float Cooldown {
        get {
            return cooldown;
        }
        set {
            cooldown = value;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1")) {
            OnFire();
        }
        if (Input.GetButtonDown("Fire2")) {
            OnReload();
        }
        CoolOff();
	}

    private void CoolOff() {
        currentCooldown -= Time.deltaTime ;
    }

    private void OnFire() {
        if (CanFire()) {
            shootingSound.Play();
            currentCooldown = cooldown;
            Instantiate(ammunitionPrefab, muzzlePoint.position, muzzlePoint.rotation);
            currentAmmunition--;
        }
    }

    private void OnReload() {
        if (CanReload()) {
            int diff = clipSize - currentAmmunition;
            totalAmmunition -= diff;
            currentAmmunition += diff;
        }
    }

    private bool CanFire() {
        return currentCooldown <= 0.0f && currentAmmunition > 0;
    }

    private bool CanReload() {
        return currentAmmunition < clipSize && totalAmmunition > 0;
    }
}
