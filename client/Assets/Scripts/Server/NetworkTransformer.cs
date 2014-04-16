using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum NetworkObjectType {
    PLAYER = 0,
    PROJECTILE
}

public enum CommunicationType {
    SEND_ONLY,
    RECEIVE_ONLY,
    SEND_AND_RECEIVE
}


public class NetworkTransformer : MonoBehaviour, IEventListener {

    public NetworkObjectType type;
    public CommunicationType commType;

    private int networkId = -1;
    private string stype;
    private IClientController server;


	// Use this for initialization
	void Start () {
        server = GameManager.gameManager.ClientController;
        server.Register(this);
        switch (type) {
            case NetworkObjectType.PLAYER:
                stype = "player";
                break;

            case NetworkObjectType.PROJECTILE:
                stype = "projectile";
                break;

            default:
                break;
        }	
	}

    public int NetworkId {
        get {
            return networkId;
        }
        set {
            networkId = value;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (commType == CommunicationType.SEND_ONLY || commType == CommunicationType.SEND_AND_RECEIVE) {
            SendData();
        }
	
	}

    private float delay = 0.1f;
    private float timepassed = 0.0f;
    private void SendData() {
        timepassed += Time.deltaTime;
        if (timepassed >= delay) {
            switch (type) {
                case NetworkObjectType.PLAYER:
                    SendPlayerData();
                    break;

                case NetworkObjectType.PROJECTILE:
                    SendProjectileData();
                    break;

                default:
                    break;
            }
            
            timepassed = 0.0f;
        }
    }

    private void SendPlayerData() {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("transform", transform);
        data.Add("type", stype);
        server.Send(DataType.TRANSFORM, data);
    }

    private void SendProjectileData() {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("transform", transform);
        data.Add("type", stype);
        data.Add("networkId", networkId);
        server.Send(DataType.TRANSFORM, data);
    }

    public void Notify(string eventType, object o) {
        if (commType == CommunicationType.RECEIVE_ONLY || commType == CommunicationType.SEND_AND_RECEIVE) {
            switch (eventType) {

                case "transform":
                    handleTransform(o);
                    break;

                default:
                    break;
            }
        }
    }

    private void handleTransform(object o) {
        Dictionary<string, object> data = o as Dictionary<string, object>;
        int id = (int)data["id"];

        if (id != networkId) {
            return;
        }

        float px = (float)data["position.x"];
        float py = (float)data["position.y"];
        float pz = (float)data["position.z"];
        float rx = (float)data["rotation.x"];
        float ry = (float)data["rotation.y"];
        float rz = (float)data["rotation.z"];
        float rw = (float)data["rotation.w"];

        transform.position = new Vector3(px, py, pz);
        transform.rotation = new Quaternion(rx, ry, rz, rw);
    }
}
