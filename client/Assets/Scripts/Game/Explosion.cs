using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
    public float effectLength;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, effectLength);
	
	}

    void OnDestroy ( ) {

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
