using UnityEngine;
using System.Collections;

//use this script to fly through gui levels to have the background move and the menu
//seem more animated.

public class CameraFlyThrough : MonoBehaviour {
    public float travelSpeed;
    public float threshHold;
    public Transform[] paths;

    private int currentIndex = 0;
    private int numPaths;

	// Use this for initialization
	void Start () {
        numPaths = paths.Length;
	}
	
	// Update is called once per frame
	void Update () {
        iTween.MoveTo(gameObject, paths[currentIndex].position, travelSpeed);
        iTween.LookTo(gameObject, paths[currentIndex].position, travelSpeed);
	
	}

    void FixedUpdate() {
        if (Vector3.Distance(paths[currentIndex].position, transform.position) <= threshHold) {
            if (currentIndex < numPaths - 1) {
                currentIndex++;
            } else {
                currentIndex = 0;
            }
        }
    }
}
