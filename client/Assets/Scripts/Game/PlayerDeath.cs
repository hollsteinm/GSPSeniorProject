using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour {
    public float effectLength;
    public Camera dcamera;
    public bool isClient;

    private float timePassed;

	// Use this for initialization
	void Start () {
        dcamera = Camera.main;

	}

    private bool LifeOver() {
        return timePassed >= effectLength;
    }

    private void IncrementLife() {
        timePassed++;
    }

    private void OnFinish ( ) {
        if (isClient) {
            dcamera.GetComponent<ManeuverCamera>().target = transform;
            GameManager.gameManager.ClientController.Send(DataType.JOINGAME, "lobby");
        } 
    }

    private void Grow() {
        transform.localScale += new Vector3 ( transform.localScale.x + 0.0025f * Time.deltaTime,
            transform.localScale.y + 0.0025f * Time.deltaTime,
            transform.localScale.z + 0.0025f * Time.deltaTime );
	
    }
	
	// Update is called once per frame
	void Update () {
        if (LifeOver()) {
            OnFinish();
        } else {
            Grow();
        }
        IncrementLife();

	}
}
