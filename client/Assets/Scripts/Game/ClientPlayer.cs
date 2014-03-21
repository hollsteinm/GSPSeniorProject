using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientPlayer : MonoBehaviour, IEventListener {
    IClientController server; 

	// Use this for initialization
	void Start () {
	    server = GameManager.gameManager.ClientController;
        server.Register ( this );
        GameManager.gameManager.ClientPlayer = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        SendData ();
	
	}

    public void Notify ( string eventType, object o ) {
        if ( o == null || eventType == null ) {
            return;
        }

        switch ( eventType ) {
            default:
                break;
        }
    }

    private float delay = 0.1f;
    private float timepassed = 0.0f;
    private void SendData ( ) {
        timepassed += Time.deltaTime;
        if (timepassed >= delay) {
            server.Send(DataType.TRANSFORM, transform);
            timepassed = 0.0f;
        }
    }
}
