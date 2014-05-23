using UnityEngine;
using System.Collections;

public class ShieldEffect : MonoBehaviour {

    public float effectTime = 2.0f;
    private float timePassed = 0.0f;
    void Awake() {
        timePassed = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (timePassed < effectTime) {
            timePassed += Time.deltaTime;
        } else {
            gameObject.SetActive(false);
        }
	}
}
