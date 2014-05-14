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

    private static object mutex = new object();

	// Use this for initialization
	void Start () {

	}

    void OnLevelWasLoaded(int id) {
        if (id == 3) {
            server = GameManager.gameManager.ClientController;
            server.Register(this);
            server.Send(DataType.JOINGAME, "lobby");
            createdGames = new List<string>();
            server.Send(DataType.GAMES_GET, null);
            AddQueuedGames();
        }
    }

    private void AddQueuedGames() {
        lock (mutex) {
            List<string> temp = new List<string>(queuedGamesList);
            foreach (string s in temp) {
                if (!createdGames.Contains(s)) {
                    createdGames.Add(s);
                }
                queuedGamesList.Remove(s);
                Debug.Log("Game added: " + s);
            }
            queuedGamesList.Clear();
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Application.loadedLevelName == "lobby") {
            if (queuedGamesList.Count > 0) {
                AddQueuedGames();
            }
        }	
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

    private List<string> queuedGamesList = new List<string>();
    public void  Notify(string eventType, object o)
    {
        switch ( eventType ) {
            case "roomadd":
                lock (mutex) {
                    if(!queuedGamesList.Contains((string)o)){
                        queuedGamesList.Add((string)o);
                    }
                }
                break;

            case "roomremove":
                lock (mutex) {
                    createdGames.Remove((string)o);
                }
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
