using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum GameType {
    SINGLEPLAYER = 0,
    MULTIPLAYER
};


public class GameManager : MonoBehaviour {

    private static GameManager _gameManager = null;
    private static object mutex = new object();
    private static bool applicationIsQuitting = false;

    private GameManager ( ) {
    }

    void Start() {
        Physics.gravity = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public static GameManager gameManager {
        get {
            if ( applicationIsQuitting ) {
                return null;
            }
            lock ( mutex ) {
                if ( _gameManager == null ) {
                    GameObject singleton = new GameObject ();
                    _gameManager = singleton.AddComponent<GameManager> ();
                    singleton.name = "GameManager";
                    DontDestroyOnLoad ( singleton );
                }
                return _gameManager;
            }
        }
    }


    private IClientController client = null;
    private GameType type;

    void FixedUpdate ( ) {
        if (client != null) {
            client.Update();
        }
        if (queuedplayers.Count > 0) {
            AddQueuedPlayers();
        }
    }

    void OnLevelWasLoaded(int id) {
        if (id == 4 || id == 2) { //multiplayer or singleplayer
            AddQueuedPlayers();
        }
    }

    private void AddQueuedPlayers() {
        Debug.Log("Adding Queued Players");
        Dictionary<int, string>.KeyCollection keys = queuedplayers.Keys;
        int[] keylist = new int[keys.Count];
        keys.CopyTo(keylist, 0);        

        for(int i = 0; i < keys.Count; ++i){
            int id = keylist[i];

            GameObject other = (GameObject)Instantiate(Resources.Load("RemotePlayer"));
            other.GetComponent<RemotePlayerScript>().Id = id;
            other.GetComponent<RemotePlayerScript>().Username = queuedplayers[id];

            players.Add(id, other);
            queuedplayers.Remove(id);
            Debug.Log("Player added <Id : Username> <" + other.GetComponent<RemotePlayerScript>().Id + " : "
                + other.GetComponent<RemotePlayerScript>().Username + ">");
        }
    }

    public void OnDestroy ( ) {
        applicationIsQuitting = true;
    }

    void OnApplicationQuit ( ) {
        client.Disconnect ();
    }

    public GameType gameType {
        get {
            return type;
        }
        set {
            type = value;
            NotifyGameTypeChange ();
        }
    }

    private void NotifyGameTypeChange ( ) {
        switch ( type ) {
            case GameType.MULTIPLAYER:
                client = SFSClient.Singleton;
                break;
            case GameType.SINGLEPLAYER:
                client = DummyClient.Singleton;
                break;
            default:
                Debug.LogError ( "Unkown gametype value set" );
                break;
        }
    }

    public IClientController ClientController {
        get {
            return client;
        }
    }

    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject> ();
    private Dictionary<int, string> queuedplayers = new Dictionary<int, string>();
    private GameObject clientPlayer = null;

    public Dictionary<int, GameObject> Players {
        get {
            return players;
        }
    }

    public void AddRemotePlayer ( int id, string name ) {
        if (Application.loadedLevelName == "multiplayer" || Application.loadedLevelName == "singleplayer") {
            if (!queuedplayers.ContainsKey(id)) {
                queuedplayers.Add(id, name);
                Debug.Log("Queueing Player: " + id.ToString() + "/" + name);
            }
        }
    }

    public void RemoveRemotePlayer(int id) {
        if (players.ContainsKey(id)) {
            GameObject obj = players[id];
            Destroy(obj);
            players.Remove(id);
        }
    }

    private int fkplayers = 10000;
    public void Update() {
        if (Input.GetKeyDown(KeyCode.R) && Application.isEditor) {
            AddRemotePlayer(fkplayers, "FakePlayer#"+fkplayers);
            fkplayers++;
        }
    }


}
