using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientPlayer : MonoBehaviour, IEventListener {
    IClientController server;
    public float hullHealth = 100.0f;
    public GameObject deathprefab;

	// Use this for initialization
	void Start () {
	    server = GameManager.gameManager.ClientController;
        server.Register ( this );
        server.Send(DataType.SPAWNED, null);
	}

    void OnDestroy() {
        if (deathprefab != null ) {
            Instantiate(deathprefab, transform.position, transform.rotation);
        }
    }
	
	// Update is called once per frame
	void Update () {
        SendData ();
	}

    public void Notify ( string eventType, object o ) {
        if ( o == null || eventType == null ) {
            return;
        }

        switch ( eventType ) {
            case "spawn":
                //early out for debugging due to 'jittering' issue on spawn - conflicting information, need to have a gameStart() method
                if (o.GetType() != typeof(Dictionary<string, float>)) {
                    return;
                }

                Dictionary<string, float> data = o as Dictionary<string, float>;
                float px = data["position.x"];
                float py = data["position.y"];
                float pz = data["position.z"];
                float rx = data["rotation.x"];
                float ry = data["rotation.y"];
                float rz = data["rotation.z"];
                float rw = data["rotation.w"];
                
                transform.position = new Vector3(px, py, pz);
                transform.rotation = new Quaternion(rx, ry, rz, rw);

                Weapon[] weapons = GetComponents<Weapon>();
                int numWeapons = (int)data["numWeapons"];

                for (int i = 0; i < numWeapons; ++i) {
                    weapons[i].cooldown = data["cooldown"+i.ToString()];
                    weapons[i].damage = data["damage" + i.ToString()];
                }

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
                
                hullHealth -= dmg;
                Instantiate(Resources.Load("HitPrefab"), contactPoint, transform.rotation);

                Debug.Log("Damage Taken by Client. Damage: " + dmg + " Remaining Health: " + hullHealth);

                break;
            default:
                break;
        }
    }

    private float delay = 0.1f;
    private float timepassed = 0.0f;
    private void SendData ( ) {
        timepassed += Time.deltaTime;
        if (timepassed >= delay) {
            server.Send(DataType.TRANSFORM, transform);
            timepassed = 0.0f;
        }
        if (hullHealth <= 0.0f) {
            server.Send(DataType.DEATH, null);
            Destroy ( gameObject );
        }
    }
}
