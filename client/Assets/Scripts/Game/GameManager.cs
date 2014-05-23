﻿using UnityEngine;
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

    public GunType CurrentWeaponChoice {
        get {
            return gunType;
        }
        set {
            gunType = value;
        }
    }

    public string[] PlayerNames {
        get{
            string[] playerstrings = new string[players.Values.Count];
            GameObject[] p = new GameObject[players.Values.Count];
            players.Values.CopyTo(p, 0);

            int index = 0;
            foreach(GameObject go in p){
                playerstrings[index] = go.GetComponent<RemotePlayerScript>().Username;
                index++;
            }
            return playerstrings;
        }
    }

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
            Screen.lockCursor = true;
        }

        //clear the player queue
        if(id == 3 || id == 0){
            players.Clear();
            queuedplayers.Clear();
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
            }
        }
    }

    public int getQueuedCount() {
        return queuedplayers.Count;
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
            GameObject obj = players[id];
            Destroy(obj);
            players.Remove(id);
        } else if (queuedplayers.ContainsKey(id)) {
            queuedplayers.Remove(id);
        }
    }

    private int fkplayers = 10000;
    public void Update() {
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
        o.GetComponent<NetworkTransformer>().NetworkId = (int)data["networkId"];
    }

}