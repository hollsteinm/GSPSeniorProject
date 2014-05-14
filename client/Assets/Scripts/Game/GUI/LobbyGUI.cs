using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyGUI : MonoBehaviour, IEventListener {
    private IClientController server = null;
    private List<string> createdGames;

    public GUIStyle lobbyStyle;

    public GUIContent playContent;
    public GUIContent highScoreContent;
    public GUIContent logoutContent;
    public Rect playRect;
    public Rect highScoreRect;
    public Rect logoutRect;

    public float gameListHeight;
    public float gameListWdith;
    public float gameListX;
    public float gameListY;

	// Use this for initialization
	void Start () {
        server = GameManager.gameManager.ClientController;
        server.Register ( this );
        server.Send(DataType.JOINGAME, "lobby");
        createdGames = new List<string>();
	}
	
	// Update is called once per frame
	void Update () {

	
	}

    void OnGUI ( ) {
        if ( GUI.Button ( playRect, playContent, lobbyStyle ) ) {
            server.Send ( DataType.MAKEGAME, null );
        }
        if (GUI.Button(logoutRect, logoutContent, lobbyStyle)) {
            server.Logout();
            Application.LoadLevel("launch");
        }

        DrawGameList ();

        if (GUI.Button(highScoreRect, highScoreContent, lobbyStyle)) {
            Application.LoadLevel("highscores");
        }
    }

    public void  Notify(string eventType, object o)
    {
        switch ( eventType ) {
            case "roomadd":
                createdGames.Add ( ( string ) o );
                break;

            case "roomremove":
                createdGames.Remove((string)o);
                break;

            default:
                break;
        }
    }

    private void DrawGameList ( ) {
        float top = gameListY;
        foreach ( string s in createdGames ) {
            if ( GUI.Button ( new Rect ( gameListX, top, gameListWdith, gameListHeight ), s, lobbyStyle ) ) {
                server.Send ( DataType.JOINGAME, s );
            }
            top += gameListHeight;
        }
    }
}
