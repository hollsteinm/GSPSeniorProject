using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[RequireComponent(typeof(LineRenderer))]
public class Weapon : MonoBehaviour {
    IClientController server;
    LineRenderer laser;
    public float range;
    public float damage;
    public float cooldown;
    public float effectCooldown;
    private float currCooldown = 0.0f;
    private float currEffectCooldown = 0.0f;

	// Use this for initialization
	void Start () {
        server = GameManager.gameManager.ClientController;
        laser = GetComponent<LineRenderer>();
        laser.enabled = false;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (laser.enabled && currEffectCooldown <= 0.0f) {
            laser.enabled = false;
        } else if (currEffectCooldown > 0.0f) {
            currEffectCooldown -= Time.deltaTime;
        }

        if (currCooldown <= 0.0f) {
            if (Input.GetButtonDown("Fire1")) {
                OnFire();
            }
        }
        currCooldown -= Time.deltaTime;
	}

    protected void OnFire() {
        laser.enabled = true;
        laser.SetPosition(0, transform.position);
        laser.SetPosition(1, transform.position + transform.forward * range);
        currEffectCooldown = effectCooldown;

        RaycastHit hitInfo = new RaycastHit();
        if(Physics.Raycast(new Ray(transform.position, transform.TransformDirection(Vector3.forward)), out hitInfo, range)){
            RemotePlayerScript rmplayer = hitInfo.collider.gameObject.GetComponent<RemotePlayerScript>();
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("damage", damage);
            data.Add("player.hit.id", rmplayer.Id);
            server.Send(DataType.FIRE, data);
        }
        currCooldown = cooldown;
    }
}
