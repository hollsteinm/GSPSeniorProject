using UnityEngine;
using System.Collections;

public class RemotePlayer : MonoBehaviour {
    private string name = "";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {
        Vector3 wts = Camera.main.WorldToScreenPoint(transform.position);
        GUI.TextArea(new Rect(wts.x, wts.y, 128, 32), name);
    }

    public string Name {
        get {
            return name;
        }
        set {
            name = value;
        }
    }
}
