using UnityEngine;
using System.Collections;

public class Revolve : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        iTween.RotateBy(gameObject, new Vector3(1.0f, 0.0f, 0.02f), 3600.0f);
	
	}
}
