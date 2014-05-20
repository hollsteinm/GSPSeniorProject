using UnityEngine;
using System.Collections;

public class MatchGUI : MonoBehaviour {
    private bool isActive;
    private string[] playerList;

    public GUIContent leaveMatchContent;
    public GUIContent logoutContent;

    public Rect leaveMatchRect;
    public Rect logoutRect;

    public GUIStyle buttonStyle;
    public GUIStyle infoStyle;

    public int infoX;
    public int infoY;
    public int infoWidth;
    public int infoHeight;

    public int infoXStride;
    public int infoYStride;

	// Use this for initialization
	void Start () {
        
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isActive) {
                isActive = false;
            } else {
                isActive = true;
                Screen.lockCursor = false;
            }
        }
        if (!isActive) {
            Screen.lockCursor = true;
        }
	}

    void OnGUI() {
        if (isActive) {
            DrawOverlay();
        }
    }

    private void DrawOverlay() {
        int index = 0;
        playerList = GameManager.gameManager.PlayerNames;
        foreach (string s in playerList) {
            GUI.Label(new Rect(infoX + (infoXStride * index), infoY + (infoYStride * index), infoWidth, infoHeight), s, infoStyle);
            index++;
        }

        if (GUI.Button(leaveMatchRect, leaveMatchContent, buttonStyle)) {
            GameManager.gameManager.ClientController.Send(DataType.JOINGAME, "lobby");
        }

        if (GUI.Button(logoutRect, logoutContent, buttonStyle)) {
            GameManager.gameManager.ClientController.Logout();
            GameManager.gameManager.ClientController.Disconnect();
            Application.LoadLevel("launch");
        }
        
    }
}
