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
        SweepTest();
        Move();
    }

    private void Move() {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }

    private void SweepTest() {
        RaycastHit hitInfo = new RaycastHit();
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.red);
        if (Physics.Raycast(new Ray(transform.position, transform.TransformDirection(Vector3.forward)), out hitInfo, speed)) {
            try {
                other = hitInfo.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject;

                colpoint = other.transform.position;

                if (other.GetComponent<RemotePlayerScript>() != null) {
                    OnRemoteHit();
                } else if (other.GetComponent<ClientPlayer>() != null) {
                    OnClientHit();
                }

            } catch (System.Exception e) {
                //here I actually intend to ignore the null reference because that means i hit something that isn't a player.
            }
            OnCollide();
        }
    }

    Vector3 colpoint = new Vector3();
    void OnTriggerEnter(Collider colother) {
        other = colother.gameObject.transform.parent.gameObject.transform.parent.gameObject;
        colpoint = transform.parent.position;

        if (other.GetComponent<ClientPlayer>() != null) {
            OnClientHit();
        } else if (other.GetComponent<RemotePlayerScript>() != null) {
            OnRemoteHit();
        }

        OnCollide();
    }

    void OnCollisionEnter(Collision colother) {
        other = colother.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject;
        col = colother;

        if (other.GetComponent<ClientPlayer>() != null) {
            OnClientHit();
        } else if (other.GetComponent<RemotePlayerScript>() != null) {
            OnRemoteHit();
        }

        OnCollide();
    }

    private void OnCollide() {
        Instantiate(collisionEffectPrefab, transform.position, transform.rotation);
        collisionSound.Play();
        Destroy(gameObject);
    }

    private void OnClientHit() {
        ClientPlayer cp = other.GetComponent<ClientPlayer>();
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("damage", damage);
        data.Add("player.hit.id", -1);
        
        Vector3 contactPoint;
        if (col != null) {
            contactPoint = col.contacts[0].point;
        } else {
            contactPoint = colpoint;
        }
        
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

        Vector3 contactPoint;
        if (col != null) {
            contactPoint = col.contacts[0].point;
        } else {
            contactPoint = colpoint;
        }

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