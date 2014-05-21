using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour {

    public Transform farSight;
    public Transform nearSight;
    public Transform muzzlePoint;

    private Transform targetTransform;

    public GameObject distanceMessage;

    public float range;

    public Transform TargetTransform {
        get {
            return targetTransform;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.parent.forward, out hitInfo, range)) {
            targetTransform.position = hitInfo.point;
            farSight.position = hitInfo.point;
            nearSight.position = hitInfo.point - hitInfo.normal * 0.5f;
            distanceMessage.GetComponent<TextMesh>().text = Vector3.Distance(muzzlePoint.transform.position, hitInfo.point).ToString("F");
        } else {
            farSight.position = muzzlePoint.position + muzzlePoint.parent.forward * range;
            nearSight.position = muzzlePoint.position + muzzlePoint.parent.forward * (range * 0.5f);
            targetTransform = farSight.transform;
            distanceMessage.GetComponent<TextMesh>().text = "";
        }
	
	}
}
