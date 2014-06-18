using UnityEngine;
using System.Collections;

public class ControlsGUI : MonoBehaviour {
    public GUIStyle controlsStyle;
    public GUIStyle backStyle;

    public GUIContent backContent;
    public Rect backRect;

    public int x;
    public int y;
    public int width;
    public int height;
    public int xstride;
    public int ystride;

    public string[] controlsList;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public AudioSource click;
    void OnGUI() {
        if (GUI.Button(backRect, backContent, backStyle)) {
            click.Play();
            Application.LoadLevel("launch");
        }

        int index = 1;
        foreach (string s in controlsList){
            GUI.Label(new Rect(x + (xstride * index), y + (ystride * index), width, height), s, controlsStyle);
            index++;
        } 
    }
}
