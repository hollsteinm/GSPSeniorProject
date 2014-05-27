using UnityEngine;
using System.Collections;

public class SeekerMissile : Projectile {
    public Transform target;

    protected override void OnStart() {
        range = 10000.0f;
        speed = 1000.0f;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Reticle>().TargetTransform;
        if (target == null) {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Reticle>().farSight.transform;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        SweepTest();
        if (target != null) {
            transform.LookAt(target);
            Move();
        } 
	}
}
