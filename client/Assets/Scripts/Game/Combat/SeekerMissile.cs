using UnityEngine;
using System.Collections;

public class SeekerMissile : Projectile {
    private Transform target;

    protected void OnStart() {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Reticle>().TargetTransform;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        SweepTest();
        Vector3 dir = target.position - transform.position;
        iTween.RotateAdd(this.gameObject, dir, 0.25f);
        this.gameObject.transform.position += transform.forward * Speed * Time.fixedDeltaTime;
	
	}
}
