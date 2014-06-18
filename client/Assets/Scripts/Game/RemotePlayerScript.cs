using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RemotePlayerScript : MonoBehaviour, IEventListener {
    private string username = "";
    GUIText guiTextref;
    private int remoteId = -1;
    private float localHealth = 300.0f;
    private float maxHealth = 300.0f;
    IClientController server;

    public GameObject fireDamage;
    public GameObject smokeDamage;

    public float MaxHealth
    {
        set
        {
            maxHealth = value;
            localHealth = value;
        }
    }

	// Use this for initialization
	void Start () {
        guiTextref = gameObject.GetComponentInChildren<GUIText>();
        guiTextref.text = username;
        guiTextref.color = new Color(0.0f, 1.0f, 0.0f);
        server = GameManager.gameManager.ClientController;
        server.Register(this);
        maxHealth = localHealth;
	}

	// Update is called once per frame
	void Update () {
        if (Application.isEditor) {
            if (localHealth <= 0.0f) {
                Notify("player.remote.death", remoteId);
            }
        }

        if ((localHealth / maxHealth) <= 0.3f) {
            fireDamage.SetActive(true);
        }

        if ((localHealth / maxHealth) <= 0.15f) {
            smokeDamage.SetActive(true);
        }
        noCollideCoolant -= Time.deltaTime;
        if (noCollideCoolant <= 0.0f) {
            noCollideCoolant = 0.0f;
            collide = true;
        }
	}

    float noCollideCoolant = 5.0f; //5 seconds to seperate players.
    bool collide = false;
    void OnCollisionEnter(Collision col) {
        if (collide) {
            localHealth = 0.0f;
        }
    }

    public float Health {
        get {
            return localHealth;
        }
    }

    void OnDestroy() {
        //TODO: instantiate explosion prefab
        //Instantiate(Resources.Load("DeathPrefabRemote"));
    }

    public string Username {
        get {
            return username;
        }
        set {
            username = value;
            try {
                guiTextref.text = username;
            } catch (System.Exception e) {
                //eat it
            }
        }
    }

    public int Id {
        get {
            return remoteId;
        }
        set {
            remoteId = value;
            GetComponent<NetworkTransformer>().NetworkId = remoteId;
        }
    }

    public void Notify(string eventType, object o) {
        switch (eventType) {
            case "player.remote.hit":
                handlePlayerRemoteHit ( o );
                break;

            case "player.remote.death":
                handlePlayerRemoteDeath ( o );
                break;

            case "powerup":
                handlePlayerRemotePowerUp(o);
                break;

            default:
                break;
        }
    }

    private void handlePlayerRemotePowerUp(object o){
        Dictionary<string, object> data = o as Dictionary<string, object>;
        string type = (string)data["powerup"];
        int id = (int)data["playerid"];

        if (id == remoteId) {
            switch (type) {
                case "shield":
                    GameObject shield = transform.FindChild("ShieldEffectPrefab").gameObject;
                    shield.SetActive(true);
                    ShieldEffect se = shield.GetComponent<ShieldEffect>();
                    se.effectTime = 30;                    

                    break;

                default:
                    break;
            }
        }

    }

    private void handlePlayerRemoteHit(object o){
        Dictionary<string, object> data = o as Dictionary<string, object>;
        if ( ( int ) data[ "player.hit.id" ] == remoteId ) {
            localHealth -= ( float ) data[ "damage" ];
            Vector3 contactPoint = new Vector3 (
                ( float ) data[ "contact.point.x" ],
                ( float ) data[ "contact.point.y" ],
                ( float ) data[ "contact.point.z" ] );
            Instantiate ( Resources.Load ( "HitPrefab" ), contactPoint, transform.rotation );
            transform.FindChild("ShieldEffectPrefab").gameObject.SetActive(true);

            Debug.Log ( "Damage taken <Damage : Health> <" + ( ( float ) data[ "damage" ] ).ToString () + " : "
                + localHealth.ToString () + "> to remote player." );
        }
    }

    public GameObject deathPrefab;
    private void handlePlayerRemoteDeath ( object o ) {
        if ( ( int ) o == remoteId ) {
            GameManager.gameManager.RemoveRemotePlayer ( remoteId );
            Instantiate(deathPrefab, transform.position, transform.rotation);
        }
    }
}
