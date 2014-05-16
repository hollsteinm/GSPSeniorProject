using UnityEngine;
using System.Collections;

public class QueueGUI : MonoBehaviour {
    private string waitingText = "Waiting for players to join your game...";

    public GUIStyle queueStyle;
    public GUIContent readyContent;
    public GUIContent backContent;
    public GUIStyle textStyle;

    void OnGUI() {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        
        GUILayout.BeginArea(new Rect(32, 32, 1024, 128));
        GUILayout.BeginVertical();
        GUILayout.Label(waitingText, textStyle);
        GUILayout.Label("Number of Players Joined: " + GameManager.gameManager.getQueuedCount().ToString(), textStyle);
        GUILayout.EndVertical();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(32, 128, 1024, 64));
        GUILayout.BeginHorizontal();
        if (GameManager.gameManager.getQueuedCount() >= 0) {
            if (GUILayout.Button(readyContent, GUILayout.MaxWidth(512))) {
                Application.LoadLevel("multiplayer");
            }
        }
        GUILayout.Button(backContent, GUILayout.MaxWidth(512));
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUILayout.EndArea();
            
    }
}
