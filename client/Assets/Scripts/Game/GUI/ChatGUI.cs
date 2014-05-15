using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChatGUI : MonoBehaviour, IEventListener {
    public float deltaTop;
    public float deltaWidth;

    private List<string> messages = new List<string>();
    Vector2 scrollPos;
    GUIStyle chat;
    string sendmessage = "";

	// Use this for initialization
	void Start () {
        chat = new GUIStyle();
        chat.normal.textColor = new Color(0.0f, 1.0f, 0.0f);
        chat.wordWrap = true;

        GameManager.gameManager.ClientController.Register(this);	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {
        GUILayout.BeginArea(new Rect(0, Screen.height - deltaTop, Screen.width - deltaWidth, deltaTop));
        GUILayout.Box("Chat", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUILayout.BeginVertical();
        GUILayout.BeginArea(new Rect(20, 25, Screen.width - deltaWidth - 64, deltaTop - 96));

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        foreach (string s in messages) {
            DrawMessage(s);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(30, 256.0f - 40.0f, Screen.width - deltaWidth, 40.0f));
        GUILayout.BeginHorizontal();
        sendmessage = GUILayout.TextField(sendmessage, 256, GUILayout.MaxWidth(Screen.width - deltaWidth - 128));

        if (GUILayout.Button("Send", GUILayout.MaxWidth(64)) || (Event.current.type == EventType.keyDown && Event.current.character == '\n')) {
            GameManager.gameManager.ClientController.Send(DataType.CHARMESSAGE, (string)sendmessage);
            sendmessage = "";
        }
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void DrawMessage(string s) {
        GUILayout.BeginHorizontal();
        GUILayout.Space(5);
        GUILayout.Label(s, chat);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(1);
    }


    public void Notify(string eventType, object o) {
        switch (eventType) {
            case "charmessage":
                if (o.GetType() != typeof(string)) {
                    return;
                }

                messages.Add((string)o);
                scrollPos.y = Mathf.Infinity;
                break;

            case "clearmessages":
                messages.Clear();
                break;
            
            default:
                break;
        }
    }
}
