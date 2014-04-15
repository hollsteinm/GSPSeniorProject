using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioClip))]
public class Projectile : MonoBehaviour {
    public GameObject collisionEffectPrefab;
    public AudioSource collisionSound;

    private GameObject other;
    private Collision col;

    private float range = 1000.0f;
    private float damage = 50.0f;
    private float speed = 100.0f;
    private Vector3 spawnLocation;

    private int networkInstanceID;

	// Use this for initialization
    bool doonce = true;
	void Start () {
        //give control to the transformations
        gameObject.rigidbody.isKinematic = true;
        gameObject.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        spawnLocation = gameObject.transform.position;
        //GameManager.gameManager.ClientController.Register(this);
        if (doonce) {
            Dictionary<string, float> data = new Dictionary<string, float>();
            data.Add("position.x", transform.position.x);
            data.Add("position.y", transform.position.y);
            data.Add("position.z", transform.position.z);
            data.Add("rotation.x", transform.rotation.x);
            data.Add("rotation.y", transform.rotation.y);
            data.Add("rotation.z", transform.rotation.z);
            data.Add("rotation.w", transform.rotation.w);
            data.Add("damage", damage);
            data.Add("speed", speed);
            networkInstanceID = GetHashCode();//TODO: make sure to have server calculate this.
            data.Add("networkId", networkInstanceID);
            GameManager.gameManager.ClientController.Send(DataType.SHOOT, data);
            doonce = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(transform.position, spawnLocation) > range) {
            Destroy(gameObject);
        }	
	}

    void FixedUpdate() {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
        //SendData();
    }

    void OnCollisionEnter(Collision collision) {
        other = collision.collider.gameObject;
        col = collision;

        if (other.GetComponent<ClientPlayer>() != null) {
            OnClientHit();
        } else if (other.GetComponent<RemotePlayerScript>() != null) {
            OnRemoteHit();
        }

        Instantiate(collisionEffectPrefab);
        collisionSound.Play();
        Destroy(gameObject);
    }

    private void OnClientHit() {
        ClientPlayer cp = other.GetComponent<ClientPlayer>();
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("damage", damage);
        data.Add("player.hit.id", -1);

        Vector3 contactPoint = col.contacts[0].point;
        data.Add("contact.point.x", contactPoint.x);
        data.Add("contact.point.y", contactPoint.y);
        data.Add("contact.point.z", contactPoint.z);
        GameManager.gameManager.ClientController.Send(DataType.FIRE, data);
    }

    private void OnRemoteHit() {
        RemotePlayerScript rp = other.GetComponent<RemotePlayerScript>();
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("damage", damage);
        data.Add("player.hit.id", rp.Id);

        Vector3 contactPoint = col.contacts[0].point;
        data.Add("contact.point.x", contactPoint.x);
        data.Add("contact.point.y", contactPoint.y);
        data.Add("contact.point.z", contactPoint.z);
        GameManager.gameManager.ClientController.Send(DataType.FIRE, data);
    }
    /*
    private float delay = 0.1f;
    private float timepassed = 0.0f;
    private void SendData ( ) {
        timepassed += Time.deltaTime;
        if (timepassed >= delay) {
            GameManager.gameManager.ClientController.Send(DataType.TRANSFORM, transform);
            timepassed = 0.0f;
        }
    }

    public void Notify(string eventType, object o) {
        switch (eventType) {
            case "transform.projectile":
                Dictionary<string, object> data = o as Dictionary<string, object>;
               
                if((int)data["instanceID"] == networkInstanceID){
                    transform.position = new Vector3(
                        (float)data["position.x"],
                        (float)data["position.y"],
                        (float)data["position.z"]);

                    transform.rotation = new Quaternion(
                        (float)data["rotation.x"],
                        (float)data["rotation.y"],
                        (float)data["rotation.y"],
                        (float)data["rotation.w"]);
                }
                break;

            default:
                break;
        }
    }*/
}