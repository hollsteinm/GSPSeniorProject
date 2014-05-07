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

    private GameManager() {
    }

    void Start() {
        Physics.gravity = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public static GameManager gameManager {
        get {
            if (applicationIsQuitting) {
                return null;
            }
            lock (mutex) {
                if (_gameManager == null) {
                    GameObject singleton = new GameObject();
                    _gameManager = singleton.AddComponent<GameManager>();
                    singleton.name = "GameManager";
                    DontDestroyOnLoad(singleton);
                }
                return _gameManager;
            }
        }
    }


    private IClientController client = null;
    private GameType type;

    void FixedUpdate() {
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
        if (Application.loadedLevelName == "multiplayer" || Application.loadedLevelName == "singleplayer") {

            Debug.Log("Adding Queued Players");
            Dictionary<int, string> copy = new Dictionary<int, string>(queuedplayers);

            lock (mutex) {
                foreach (KeyValuePair<int, string> entry in copy) {
                    GameObject other = (GameObject)Instantiate(Resources.Load("RemotePlayer"));
                    other.GetComponent<RemotePlayerScript>().Id = entry.Key;
                    other.GetComponent<RemotePlayerScript>().Username = entry.Value;

                    players.Add(entry.Key, other);
                    queuedplayers.Remove(entry.Key);

                    Debug.Log("Player added <Id : Username> <" + other.GetComponent<RemotePlayerScript>().Id + " : "
                                + other.GetComponent<RemotePlayerScript>().Username + ">");
                }
            }
        }
    }

    public void OnDestroy() {
        applicationIsQuitting = true;
    }

    void OnApplicationQuit() {
        client.Disconnect();
    }

    public GameType gameType {
        get {
            return type;
        }
        set {
            type = value;
            NotifyGameTypeChange();
        }
    }

    private void NotifyGameTypeChange() {
        switch (type) {
            case GameType.MULTIPLAYER:
                client = SFSClient.Singleton;
                break;
            case GameType.SINGLEPLAYER:
                client = DummyClient.Singleton;
                break;
            default:
                Debug.LogError("Unkown gametype value set");
                break;
        }
    }

    public IClientController ClientController {
        get {
            return client;
        }
    }

    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    private Dictionary<int, string> queuedplayers = new Dictionary<int, string>();
    private GameObject clientPlayer = null;

    public Dictionary<int, GameObject> Players {
        get {
            return players;
        }
    }

    public void AddRemotePlayer(int id, string name) {
        Debug.Log("Attempting to add remote player.");
        //if (Application.loadedLevelName == "multiplayer" || Application.loadedLevelName == "singleplayer") {
            //if (!queuedplayers.ContainsKey(id)) {
        lock (mutex) {
            queuedplayers.Add(id, name);
        }
        Debug.Log("Queueing Player: " + id.ToString() + "/" + name);
            //}
        //}
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
        if (Application.loadedLevelName == "multiplayer" || Application.loadedLevelName == "singleplayer") {
            Screen.lockCursor = true;
        }

        if (Input.GetKeyDown(KeyCode.R) && Application.isEditor) {
            AddRemotePlayer(fkplayers, "FakePlayer#" + fkplayers);
            fkplayers++;
        }
    }

    public void CreateProjectile(Dictionary<string, object> data) {
        float px = (float)data["position.x"];
        float py = (float)data["position.y"];
        float pz = (float)data["position.z"];
        float rx = (float)data["rotation.x"];
        float ry = (float)data["rotation.y"];
        float rz = (float)data["rotation.z"];
        float rw = (float)data["rotation.w"];

        Vector3 position = new Vector3(px, py, pz);
        Quaternion rotation = new Quaternion(rx, ry, rz, rw);

        GameObject o = (GameObject)Instantiate(Resources.Load("RemoteProjectile"), position, rotation);
        o.GetComponent<Projectile>().Range = (float)data["range"];
        o.GetComponent<Projectile>().Damage = (float)data["damage"];
        o.GetComponent<Projectile>().Speed = (float)data["speed"];
        o.GetComponent<NetworkTransformer>().NetworkId = (int)data["networkId"];
    }

}