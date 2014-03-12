using UnityEngine;
using System.Collections;

public class LoginGUI : MonoBehaviour, IEventListener {
    private IClientController server = null;

	// Use this for initialization
	void Start () {
        server = GameManager.gameManager.ClientController;
        server.Register ( this );
        server.Connect ( "localhost", 9933 );	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Notify ( string eventType, object o ) {
        switch(eventType){
            case "server":
                Debug.Log ( "Server event received." );
                break;
            default:
                break;
        }
    }

    void OnGUI ( ) {
        if ( GUI.Button ( new Rect ( 0, 32, Screen.width, 64 ), "Login" ) ) {
            server.Login ( "", "" );
        }
    }
}
