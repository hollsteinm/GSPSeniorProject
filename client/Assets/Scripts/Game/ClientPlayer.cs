using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientPlayer : MonoBehaviour, IEventListener {
    IClientController server;
    public ShipHull shipHull;

	// Use this for initialization
	void Start () {
	    server = GameManager.gameManager.ClientController;
        server.Register ( this );
        server.Send(DataType.SPAWNED, null);
	}
	
	// Update is called once per frame
	void Update () {
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

                shipHull.OnHit(dmg, contactPoint);

                Debug.Log("Damage Taken by Client. Damage: " + dmg + " Remaining Health: " + shipHull.Health);

                break;
            default:
                break;
        }
    }
}
