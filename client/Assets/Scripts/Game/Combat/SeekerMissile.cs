using UnityEngine;
using System.Collections;

public class SeekerMissile : Projectile {
    private Transform target;

    protected void OnStart() {
        Range = 5000.0f;
        Speed = 50.0f;
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
        } else {
            Move();
        }
	}
}
