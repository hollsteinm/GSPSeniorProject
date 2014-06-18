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
    public GUIContent configContent;
    public Rect configRect;
    public Rect playRect;
    public Rect highScoreRect;
    public Rect logoutRect;

    public float gameListHeight;
    public float gameListWidth;
    public float gameListX;
    public float gameListY;

    public float configListWidth;
    public float configListHeight;

    private static object mutex = new object();

	// Use this for initialization
	void Start () {
        createdGames = new List<string>();
        createdGames.Add("");
	}

    void OnLevelWasLoaded(int id) {
        if (id == 3) {
            server = GameManager.gameManager.ClientController;
            server.Register(this);
            //server.Send(DataType.JOINGAME, "lobby");
            createdGames = new List<string>();
            createdGames.Add("");
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

    public AudioSource click;
    void OnGUI ( ) {
        if ( GUI.Button ( playRect, playContent, lobbyStyle ) ) {
            click.Play();
            server.Send ( DataType.MAKEGAME, null );
        }
        if (GUI.Button(logoutRect, logoutContent, lobbyStyle)) {
            click.Play();
            server.Logout();
            Application.LoadLevel("launch");
        }

        DrawGameList ();

        if (GUI.Button(configRect, configContent, lobbyStyle)) {
            click.Play();
            Application.LoadLevel("shipconfig");
        }

        if (GUI.Button(highScoreRect, highScoreContent, lobbyStyle)) {
            click.Play();
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
                    if(createdGames.Contains((string)o)){
                        createdGames.Remove((string)o);
                    }
                }
                break;

            default:
                break;
        }
    }

    private Vector2 scrollPosition;
    int selected = 0;
    private void DrawGameList ( ) {
        
        GUILayout.BeginArea(new Rect(Screen.width - gameListWidth, 0, gameListWidth, Screen.height));
        GUILayout.Box("Queuing Games", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUILayout.BeginVertical();
        GUILayout.BeginArea(new Rect(32, 20, gameListWidth - 64, Screen.height - 32));

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        string[] games = createdGames.ToArray();
        lobbyStyle.margin = new RectOffset(4, 4, 4, 4);
        selected = GUILayout.SelectionGrid(selected, games, 1, lobbyStyle);
        
        if (selected >= 0 && !createdGames[selected].Equals("")) {
            click.Play();
            server.Send(DataType.JOINGAME, createdGames[selected]);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
        GUILayout.EndVertical();
        GUILayout.EndArea();       
    }
}
