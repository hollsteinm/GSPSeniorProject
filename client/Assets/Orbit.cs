using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        iTween.RotateBy(gameObject, new Vector3(1.0f, 0.5f, 0.25f), 7200.0f);
	
	}
}
