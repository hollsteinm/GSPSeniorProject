using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RemotePlayerScript : MonoBehaviour {
    private string name = "";
    GUIText guiText;
    private int remoteId = -1;
    private float localHealth = 0.0f;
    IClient server;

	// Use this for initialization
	void Start () {
        guiText = gameObject.GetComponentInChildren<GUIText>();
        guiText.text = name;
        guiText.color = new Color(1.0f, 0.0f, 0.0f);
        server = GameManager.gameManager.ClientController;
	}
	
	// Update is called once per frame
	void Update () {
        if (localHealth <= 0.0f) {
            Destroy(this);
        }
	}

    public string Name {
        get {
            return name;
        }
        set {
            name = value;
            guiText.text = name;
        }
    }

    public int Id {
        get {
            return remoteId;
        }
        set {
            remoteId = value;
        }
    }

}
