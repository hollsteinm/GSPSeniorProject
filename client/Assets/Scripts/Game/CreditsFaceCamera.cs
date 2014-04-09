using UnityEngine;
using System.Collections;

public class CreditsFaceCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate() {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(Vector3.up, 180.0f);
    }
}
