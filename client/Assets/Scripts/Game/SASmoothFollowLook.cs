using UnityEngine;
using System.Collections;

//Script for a smooth follow and lookat that is more appropiate for our game
public class SASmoothFollowLook : MonoBehaviour {
    public Transform target;
    public float distance;
    public float height;
    public float damping;
    public bool smoothRotation;
    public float rotationDamping;

	// Use this for initialization
	void Start () {
        if ( target == null ) {
            Debug.LogError ( "SASmoothFollowLook Script on " + gameObject.name + " has no associated target reference." );
        }	    
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 position = target.TransformPoint ( 0, height, -distance );
        transform.position = Vector3.Lerp ( transform.position, position, Time.deltaTime * damping);

        if ( smoothRotation ) {
            Quaternion rotation = Quaternion.LookRotation ( target.position - transform.position, target.up );
            transform.rotation = Quaternion.Slerp ( transform.rotation, rotation, Time.deltaTime * rotationDamping );
        } else {
            transform.LookAt ( target, target.up );
        }
	}
}
