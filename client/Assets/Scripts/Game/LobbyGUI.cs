using UnityEngine;
using System.Collections;

public class LobbyGUI : MonoBehaviour, IEventListener {
    private IClientController server = null;

	// Use this for initialization
	void Start () {
        server = GameManager.gameManager.ClientController;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI ( ) {
        if ( GUI.Button ( new Rect ( 0, 32, Screen.width, 64 ), "Play" ) ) {
            server.Send ( DataType.ROOMREQUEST, null );
        }
    }

    public void  Notify(string eventType, object o)
    {
        switch ( eventType ) {
            case "server":
                break;
            case "playerjoin":
                //bring the creator (me) to the game and notify the other player
                break;
            case "join":
                //bring me to this game and notify the creator
                break;

            default:
                break;
        }
    }
}
