using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioClip))]
public class Projectile : MonoBehaviour, IEventListener {
    public GameObject collisionEffectPrefab;
    public AudioSource collisionSound;

    private GameObject other;
    private Collision col;

    private float range = 1000.0f;
    private float damage = 50.0f;
    private float speed = 1000.0f;
    private Vector3 spawnLocation;

    public float Speed {
        set {
            speed = value;
        }
        get {
            return speed;
        }
    }

    public float Damage {
        set {
            damage = value;
        }
        get {
            return damage;
        }
    }

    public float Range {
        set {
            range = value;
        }
        get {
            return range;
        }
    }

	// Use this for initialization
	void Start () {
        //give control to the transformations
        gameObject.rigidbody.isKinematic = true;
        gameObject.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        spawnLocation = gameObject.transform.position;
        GameManager.gameManager.ClientController.Register(this);
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(transform.position, spawnLocation) > range) {
            Destroy(gameObject);
        }	
	}

    void OnDestroy() {
        GameManager.gameManager.ClientController.Unregister(this);
    }

    void FixedUpdate() {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
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

    public void Notify(string eventType, object o) {
        switch (eventType) {
            case "projectile.assign":
                if (GetComponent<NetworkTransformer>().NetworkId != -1) {
                    return;
                }

                Dictionary<string, object> data = o as Dictionary<string, object>;
                range = (float)data["range"];
                damage = (float)data["damage"];
                speed = (float)data["speed"];
                GetComponent<NetworkTransformer>().NetworkId = (int)data["networkId"];
                
                break;

            default:
                break;
        }
    }
}