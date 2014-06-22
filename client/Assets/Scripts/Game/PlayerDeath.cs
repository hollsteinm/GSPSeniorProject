using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour {
    public float effectLength = 5.0f;
    public Camera dcamera;
    public bool isClient;

    private float timePassed = 0.0f;

	// Use this for initialization
	void Start () {
        if (isClient)
        {
            dcamera = Camera.main;
            dcamera.GetComponent<ManeuverCamera>().target = transform;
        }

	}

    public void AssignFocus(int remoteid)
    {
        if (GameManager.gameManager.Players[remoteid] != null)
        {
            dcamera = Camera.main;
            dcamera.GetComponent<ManeuverCamera>().target = GameManager.gameManager.Players[remoteid].transform;
        }
    }

    private bool LifeOver() {
        return timePassed >= effectLength;
    }

    private void IncrementLife() {
        timePassed += Time.deltaTime;
    }

    private void OnFinish ( ) {
        if (isClient) {
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
        if (isClient)
        {
            dcamera = Camera.main;
        }
        if (LifeOver()) {
            OnFinish();
        } else {
            //Grow();
        }
        IncrementLife();

	}
}
