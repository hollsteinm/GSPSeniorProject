using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum WeaponType { 
    BULLET, 
    SEEKER 
};

[System.Serializable]
public enum GunType {
    CANNON,
    SEEKERMISSILELAUNCHER
};

public class Gun : MonoBehaviour, IEventListener {
    public GameObject ammunitionPrefab;
    public WeaponType weaponType;
    public GunType gunType;
    public AudioSource shootingSound;
    public Transform muzzlePoint;
    public GUIStyle gunHUDStyle;

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
        GameManager.gameManager.ClientController.Register(this);
	
	}

    void OnGUI() {
        GUI.Label(new Rect(Screen.width - 196, Screen.height - 128, 128, 32), currentCooldown.ToString("F"), gunHUDStyle);
        GUI.Label(new Rect(Screen.width - 196, Screen.height - 96, 128, 32), currentAmmunition.ToString() + " / " + clipSize.ToString(), gunHUDStyle);
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
        if (currentCooldown < 0.0f) {
            currentCooldown = 0.0f;
        }
    }

    private void OnFire() {
        if (CanFire()) {
            shootingSound.Play();
            currentCooldown = cooldown;
            SendShootRequest();
            Instantiate(ammunitionPrefab, muzzlePoint.transform.position, muzzlePoint.transform.rotation);

            currentAmmunition--;
        }
    }

    private void SendShootRequest() {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("position.x", muzzlePoint.transform.position.x);
        data.Add("position.y", muzzlePoint.transform.position.y);
        data.Add("position.z", muzzlePoint.transform.position.z);
        data.Add("rotation.x", muzzlePoint.transform.rotation.x);
        data.Add("rotation.y", muzzlePoint.transform.rotation.y);
        data.Add("rotation.z", muzzlePoint.transform.rotation.z);
        data.Add("rotation.w", muzzlePoint.transform.rotation.w);

        string type = "";
        switch (weaponType) {
            case WeaponType.SEEKER:
                type = "Seeker";
                break;

            case WeaponType.BULLET:
                type = "Bullet";
                break;

            default:
                type = "Bullet";
                break;
        }
        data.Add("type", type);

        GameManager.gameManager.ClientController.Send(DataType.SHOOT, data);
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

    public void Notify(string eventType, object o) {
        switch (eventType) {
            case "spawn":
                Dictionary<string, float> data = o as Dictionary<string, float>;
                cooldown = data["cooldown"];
                break;

            default:
                break;
        }     
    }
}
