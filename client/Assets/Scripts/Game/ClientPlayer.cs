using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientPlayer : MonoBehaviour, IEventListener {
    IClientController server;
    public ShipHull shipHull;

    private GameObject model;
    private GameObject shield;

	// Use this for initialization
	void Start () {
	    server = GameManager.gameManager.ClientController;
        server.Register ( this );

        string weaponType = "";
        switch(shipHull.weapon.gunType){
            case GunType.CANNON:
                weaponType = "Cannon";
                break;

            case GunType.SEEKERMISSILELAUNCHER:
                weaponType = "Seeker Missile Launcher";
                break;

            default:
                weaponType = "Cannon";
                break;
        }

        model = (GameObject)Instantiate(GameManager.gameManager.ShipModelPrefab);
        model.transform.parent = transform;

        shield = (GameObject)Instantiate(GameManager.gameManager.ShipShieldPrefab);
        shield.transform.parent = transform;
        
        
        server.Send(DataType.SPAWNED, weaponType);
	}
	
	// Update is called once per frame
	void Update () {
        noCollideCoolant -= Time.deltaTime;
        if (noCollideCoolant <= 0.0f) {
            noCollideCoolant = 0.0f;
            collide = true;
        }
	}

    void OnDestroy() {
        server.Unregister(this);
    }

    float noCollideCoolant = 5.0f; //5 seconds to seperate players.
    bool collide = false;
    void OnCollisionEnter(Collision col) {
        if (collide) {
            shipHull.onHit.Play();
            shipHull.Health = 0.0f;
        }
    }

    public void Notify ( string eventType, object o ) {
        if ( o == null || eventType == null ) {
            return;
        }

        switch ( eventType ) {
            case "spawn":
                //early out for debugging due to 'jittering' issue on spawn - conflicting information, need to have a gameStart() method
                if (o.GetType() != typeof(Dictionary<string, object>)) {
                    return;
                }
                
                Dictionary<string, float> data = o as Dictionary<string, float>;
                /*
                float px = data["position.x"];
                float py = data["position.y"];
                float pz = data["position.z"];
                float rx = data["rotation.x"];
                float ry = data["rotation.y"];
                float rz = data["rotation.z"];
                float rw = data["rotation.w"];
                
                transform.position = new Vector3(px, py, pz);
                transform.rotation = new Quaternion(rx, ry, rz, rw);
                */
                shipHull.Health = (float)data["health"];
                shipHull.MaxHealth = (float)data["health"];
                shipHull.weapon.Cooldown = (float)data["cooldown"];
                GetComponent<Reticle>().range = (float)data["range"];
                shipHull.weapon.MaxAmmo = (int)data["ammo"];
                shipHull.weapon.Clipsize = (int)data["clipsize"];
                GetComponent<JetMovement>().maxEnergy = (float)data["energy"];
                GetComponent<JetMovement>().maxVelocity = (float)data["velocity"];
                //TODO: once the database sends the right data, we will be able to set this for the reticle
                //GetComponent<Reticle>().range = data["range"];

                break;

            case "player.hit":
                if (o.GetType() != typeof(Dictionary<string, object>)) {
                    return;
                }

                Dictionary<string, object> hitdata = o as Dictionary<string, object>;
                float dmg = (float)hitdata["damage"];
                Vector3 contactPoint = new Vector3(
                    (float)hitdata["contact.point.x"],
                    (float)hitdata["contact.point.y"],
                    (float)hitdata["contact.point.z"]);

                shipHull.OnHit(dmg, contactPoint);
                transform.FindChild("ShieldEffectPrefab(Clone)").gameObject.SetActive(true);
                Debug.Log("Damage Taken by Client. Damage: " + dmg + " Remaining Health: " + shipHull.Health);

                break;
            default:
                break;
        }
    }
}
