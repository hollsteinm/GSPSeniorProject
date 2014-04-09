using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighScoresGUI : MonoBehaviour, IEventListener {
    private long myscore;
    private List<long> topScores;
    private List<string> topNames;

    public GUIStyle highScoreStyle;
    public GUIStyle buttonStyle;
    public Rect backRect;
    public Rect myscoreRect;

    public GUIContent backContent;

    public float scoresXMin;
    public float scoresYMin;
    public float scoresWidth;
    public float scoresHeight;
    public float scoresPadding;

	// Use this for initialization
	void Start () {
        GameManager.gameManager.ClientController.Register(this);
        topScores = new List<long>();
        topNames = new List<string>();
        GameManager.gameManager.ClientController.Send(DataType.SCORES_GET, new object());
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Notify(string eventType, object data) {
        switch (eventType) {
            case "scores":
                Debug.Log("Data received.");
                DisplayScores(data);
                break;
                
            default:
                break;
        }
    }

    void OnGUI() {
        if (GUI.Button(backRect, backContent, buttonStyle)) {
            Application.LoadLevel("launch");
        }
        DrawMyScore();
        DrawTop();
    }

    private void DrawMyScore() {
        GUI.TextArea(myscoreRect, myscore.ToString(), highScoreStyle);
        
    }

    private void DrawTop() {

        float top = scoresYMin;
        float xshift = scoresXMin + scoresWidth;
        foreach(long l in topScores){
            GUI.TextArea(new Rect(xshift, top + scoresPadding, scoresWidth, scoresHeight), l.ToString(), highScoreStyle);
            top += scoresHeight;
        }

        top = scoresYMin;
        foreach (string s in topNames) {
            GUI.TextArea(new Rect(scoresXMin, top + scoresPadding, scoresWidth, scoresHeight), s, highScoreStyle);
            top += scoresHeight;
        }

    }

    private void DisplayScores(object data) {
        if (data.GetType() != typeof(Dictionary<string, object>)) {
            return;
        }

        Dictionary<string, object> cdata = data as Dictionary<string, object>;
        Dictionary<string, long> scores = cdata["scores"] as Dictionary<string, long>;
        Dictionary<string, string> names = cdata["names"] as Dictionary<string, string>;
        myscore = (long)cdata["my.score"];

        int size = scores.Count;
        for (int i = 0; i < size; ++i) {
            topScores.Add(scores["score" + i.ToString()]);
            topNames.Add(names["player" + i.ToString()]);
        }
    }
}
