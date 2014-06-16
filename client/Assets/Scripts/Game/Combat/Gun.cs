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
	
	private bool rapid = false;
	private float rapidTimer;
	
	private Texture2D rapidTex;
	
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

    public int MaxAmmo = 100;

    public int Clipsize
    {
        set
        {
            totalAmmunition = value;
        }
    }

	// Use this for initialization
	void Start () {
        gunType = GameManager.gameManager.CurrentWeaponChoice;
        switch (gunType) {
            case GunType.CANNON:
                ammunitionPrefab = (GameObject)Resources.Load("Bullet");
                weaponType = WeaponType.BULLET;
                break;

            case GunType.SEEKERMISSILELAUNCHER:
                ammunitionPrefab = (GameObject)Resources.Load("SeekerMissile");
                weaponType = WeaponType.SEEKER;
                break;

            default:
                break;
        }

        GameManager.gameManager.ClientController.Register(this);
		
		rapidTex = Resources.Load("Textures/Rapidfire") as Texture2D;
	
	}

    void OnGUI() {
        GUI.Label(new Rect(Screen.width - 196, Screen.height - 128, 128, 32), currentCooldown.ToString("F"), gunHUDStyle);
        GUI.Label(new Rect(Screen.width - 196, Screen.height - 96, 128, 32), currentAmmunition.ToString() + " / " + clipSize.ToString(), gunHUDStyle);
		
		if (rapid)
		{
			if(!rapidTex)
			{
				Debug.LogError("Assign a Texture in the inspector.");
				return;
			}
			GUI.DrawTexture(new Rect(35,10,55,30), rapidTex, ScaleMode.ScaleToFit, true, 0);
		}
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
		
		if (rapid)
		{
			rapidTimer -= Time.deltaTime;
			if (rapidTimer < 0)
				rapid = false;
		}
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
			if (rapid)
				currentCooldown = cooldown/2.0f;
			else
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
                Dictionary<string, object> data = o as Dictionary<string, object>;
                cooldown = (float)data["cooldown"];
                break;

            default:
                break;
        }     
    }
	
	public void ActivateRapid()
	{
		rapid = true;
		rapidTimer = 30;
	}
}
