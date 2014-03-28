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
    }

    void OnLevelWasLoaded(int id) {
        if (id == 4) { //multiplayer
            foreach (int i in queuedplayers.Keys) {
                AddRemotePlayer(id, queuedplayers[id]);
                queuedplayers.Remove(i);
            }
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
        if (Application.loadedLevelName == "multiplayer") {
            GameObject newPlayer = (GameObject)Instantiate(Resources.Load("RemotePlayer"));
            players.Add(id, newPlayer);
            GameObject other = players[id];
            other.GetComponent<RemotePlayerScript>().Name = name;
            other.GetComponent<RemotePlayerScript>().Id = id;
            Debug.Log("New Player added: " + id.ToString() + "/" + name);
        } else {
            queuedplayers.Add(id, name);
            Debug.Log("Queueing Player: " + id.ToString() + "/" + name);
        }
    }

    public void RemoveRemotePlayer(int id) {
        if (players.ContainsKey(id)) {
            GameObject obj = players[id];
            Destroy(obj);
            players.Remove(id);
        }
    }

    public GameObject ClientPlayer {
        get {
            return clientPlayer;
        }
        set {
            clientPlayer = value;
        }
    }
}
