using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyGUI : MonoBehaviour, IEventListener {
    private IClientController server = null;
    private List<string> createdGames = new List<string> ();

	// Use this for initialization
	void Start () {
        server = GameManager.gameManager.ClientController;
        server.Send ( DataType.JOINGAME, "lobby" );
        server.Register ( this );
	}
	
	// Update is called once per frame
	void Update () {

	
	}

    void OnGUI ( ) {
        if ( GUI.Button ( new Rect ( 0, 32, Screen.width, 64 ), "Play" ) ) {
            server.Send ( DataType.MAKEGAME, null );
        }
        DrawGameList ();
    }

    public void  Notify(string eventType, object o)
    {
        switch ( eventType ) {
            case "roomadd":
                createdGames.Add ( ( string ) o );
                break;

            default:
                break;
        }
    }

    private void DrawGameList ( ) {
        int top = 96;
        foreach ( string s in createdGames ) {
            if ( GUI.Button ( new Rect ( 0, top, Screen.width, 32 ), s ) ) {
                server.Send ( DataType.JOINGAME, s );
            }
            top += 32;
        }
    }
}
