using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientPlayer : MonoBehaviour, IEventListener {
    IClientController server; 

	// Use this for initialization
	void Start () {
	    server = GameManager.gameManager.ClientController;
        server.Register ( this );
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

    private void SendData ( ) {
        server.Send ( DataType.TRANSFORM, transform );
    }
}
