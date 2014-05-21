using UnityEngine;
using System.Collections;

public class SeekerMissile : Projectile {
    private Transform target;

    protected void OnStart() {
        Range = 5000.0f;
        Speed = 100.0f;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Reticle>().TargetTransform;
        if (target == null) {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Reticle>().farSight.transform;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        SweepTest();
        if (target != null) {
            iTween.RotateTo(this.gameObject, target.eulerAngles, 0.5f);
            iTween.MoveTo(this.gameObject, target.position, 2.0f);
        } else {
            Move();
        }
	}
}
