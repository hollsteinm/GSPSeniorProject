using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientPlayer : MonoBehaviour, IEventListener {
    IClientController server;
    public float hullHealth = 100.0f;

	// Use this for initialization
	void Start () {
	    server = GameManager.gameManager.ClientController;
        server.Register ( this );
        GameManager.gameManager.ClientPlayer = this.gameObject;
        server.Send(DataType.SPAWNED, null);
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
            case "transform":
                //early out for debugging due to 'jittering' issue on spawn - conflicting information, need to have a gameStart() method
                return;
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

                break;

            case "player.hit":
                if (o.GetType() != typeof(float)) {
                    return;
                }

                float dmg = (float)o;
                hullHealth -= dmg;

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
        }
    }
}
