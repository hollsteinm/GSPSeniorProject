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
    private GunType gunType;
    private GameObject shipModelPrefab;
    private GameObject shipShieldPrefab;

    public string weapon = "Cannon";
    public int shipid = 1;

    public GameObject ShipModelPrefab {
        get {
            Debug.Log("Getting ShipModelPrefab");
            return shipModelPrefab;
        }
        set {
            Debug.Log("Setting ShipModelPrefab");
            shipModelPrefab = value;
        }
    }

    public GameObject ShipShieldPrefab {
        get {
            Debug.Log("Getting ShipSheildPrefab");
            return shipShieldPrefab;
        }
        set {
            Debug.Log("Setting ShipSheildPrefab");
            shipShieldPrefab = value;
        }
    }

    public GunType CurrentWeaponChoice {
        get {
            Debug.Log("Getting GunType");
            return gunType;
        }
        set {
            Debug.Log("Setting ShipModelPrefab");
            gunType = value;
        }
    }

    private string[] playerStrings;
    public string[] PlayerNames {
        get{
            playerStrings = new string[players.Values.Count];
            GameObject[] p = new GameObject[players.Values.Count];
            players.Values.CopyTo(p, 0);

            int index = 0;
            foreach(GameObject go in p){
                if (go != null) {
                    if (go.GetComponent<RemotePlayerScript>() != null) {
                        playerStrings[index] = go.GetComponent<RemotePlayerScript>() != null ?
                            go.GetComponent<RemotePlayerScript>().Username : "[!]ERROR[!]";
                        index++;
                    }
                }
            }
            return playerStrings;
        }
    }

    private GameManager() {
        weapon = "Cannon";
        shipid = 1;
    }

    void Start() {
        Debug.Log("Start");
        shipModelPrefab = (GameObject)Resources.Load("StandardShip");
        shipShieldPrefab = (GameObject)Resources.Load("ShieldEffectPrefab");
        weapon = "Cannon";
        shipid = 1;
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

        if (queuedplayers.Count > 0) {
            AddQueuedPlayers();
        }
    }

    void OnLevelWasLoaded(int id) {
        Debug.Log("Level Loaded");

        if (id == 4 || id == 2) { //multiplayer or singleplayer
            Debug.Log("Adding Queued Players and locking cursor");
            AddQueuedPlayers();
            Screen.lockCursor = true;
        }

        //clear the player queue
        if(id != 4 && id != 2 && id != 8){
            Debug.Log("Attempting Cleanup for level load");
            client.EvacuateTheDanceFloor();
            //need to cleanup in a special way
            lock (mutex) {
                foreach (KeyValuePair<int, GameObject> entry in players) {
                    Debug.Log("Destroying... <" + entry.Key.ToString() + " / " + entry.Value.ToString() + ">");
                    Destroy(entry.Value);
                }

                players.Clear();
                queuedplayers.Clear();

                if (playerStrings != null) {
                    Debug.Log("Nulling playerStrings");
                    playerStrings = null;
                }

                players = new Dictionary<int, GameObject>();
                queuedplayers = new Dictionary<int, string>();
            }
            
            Screen.lockCursor = false;
        }
    }

    private void AddQueuedPlayers() {

        if (Application.loadedLevelName == "multiplayer" || Application.loadedLevelName == "singleplayer") {

            Debug.Log("Adding Queued Players");
            Dictionary<int, string> copy = new Dictionary<int, string>(queuedplayers);

            lock (mutex) {
                foreach (KeyValuePair<int, string> entry in copy) {
                    GameObject other = (GameObject)Instantiate(Resources.Load("RemotePlayer"));
                    other.transform.rotation = Quaternion.Euler(new Vector3(-90.0f, -1.5f, 0.0f));
                    other.GetComponent<RemotePlayerScript>().Id = entry.Key;
                    other.GetComponent<RemotePlayerScript>().Username = entry.Value;

                    players.Add(entry.Key, other);
                    queuedplayers.Remove(entry.Key);

                    Debug.Log("Player added <Id : Username> <" + other.GetComponent<RemotePlayerScript>().Id + " : "
                                + other.GetComponent<RemotePlayerScript>().Username + ">");
                }
                queuedplayers.Clear();
            }
        }
    }

    public int getQueuedCount() {
        Debug.Log("Getting Queued Count");
        return queuedplayers.Count;
    }

    public void OnDestroy() {
        Debug.Log("OnDestroy");
        applicationIsQuitting = true;
    }

    void OnApplicationQuit() {
        Debug.Log("Quitting App...");
        client.Logout();
        client.Disconnect();
    }

    public GameType gameType {
        get {
            Debug.Log("Getting GameType");
            return type;
        }
        set {
            Debug.Log("Setting GameType");
            type = value;
            NotifyGameTypeChange();
        }
    }

    private void NotifyGameTypeChange() {
        Debug.Log("NotifyGameTypeChange");
        switch (type) {
            case GameType.MULTIPLAYER:
                Debug.Log("Multiplayer");
                client = SFSClient.Singleton;
                break;
            case GameType.SINGLEPLAYER:
                Debug.Log("Singleplayer");
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

    public Dictionary<int, GameObject> Players {
        get {
            return players;
        }
    }

    public void AddRemotePlayer(int id, string name) {
        Debug.Log("Attempting to add remote player.");
        lock (mutex) {
            queuedplayers.Add(id, name);
        }
        Debug.Log("Queueing Player: " + id.ToString() + "/" + name);
    }

    public void RemoveRemotePlayer(int id) {
        if (players.ContainsKey(id)) {
            Debug.Log("removing player: " + id.ToString());
            GameObject obj = players[id];
            Destroy(obj);
            players.Remove(id);
        } else if (queuedplayers.ContainsKey(id)) {
            Debug.Log("Removing queued player: " + id.ToString());
            queuedplayers.Remove(id);
        }
    }

    private int fkplayers = 10000;
    public void Update() {
        if (client != null) {
            client.Update();
        }

        if (Input.GetKeyDown(KeyCode.R) && Application.isEditor) {
            AddRemotePlayer(fkplayers, "FakePlayer#" + fkplayers);
            fkplayers++;
        }
    }

    public void CreateProjectile(Dictionary<string, object> data) {
        Debug.Log("Creating Projectile");
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
        o.GetComponent<NetworkTransformer>().NetworkId = (int)data["networkId"];
        //TODO: Get Projectile type and load mesh with same name
    }

}