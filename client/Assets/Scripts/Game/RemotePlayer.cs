using UnityEngine;
using System.Collections;

public class RemotePlayer : MonoBehaviour {
    private string name = "";
    GUIText guiText;

	// Use this for initialization
	void Start () {
        guiText = gameObject.GetComponentInChildren<GUIText>();
        guiText.text = name;
        guiText.color = new Color(1.0f, 0.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update () {
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
}
