using UnityEngine;
using System.Collections;

public class MatchGUI : MonoBehaviour {
    private bool isActive;
    private string[] playerList;

    public GUIContent leaveMatchContent;
    public GUIContent logoutContent;

    public GUITexture menuBG;

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

    public int groupWidth;
    public int groupHeight;

	// Use this for initialization
	void Start () {
        
	
	}
	
	// Update is called once per frame
    bool showWinningScreen = false;
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isActive) {
                isActive = false;
                menuBG.gameObject.SetActive(false);
            } else {
                isActive = true;
                menuBG.gameObject.SetActive(true);
                Screen.lockCursor = false;
            }
        }
        if (!isActive) {
            Screen.lockCursor = true;
        }
        if (GameManager.gameManager.PlayerNames.Length <= 0) {
            showWinningScreen = true;
        }
	}

    void OnGUI() {
        if (isActive) {
            DrawOverlay();
        }
        if (showWinningScreen) {
            GUILayout.BeginArea(new Rect(Screen.width / 2.0f - 128, Screen.height / 2.0f - 32, 256, 64));
            GUILayout.BeginVertical();
            GUILayout.Label("You are the Ace!", infoStyle);
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }

    private void DrawOverlay() {
        int index = 0;
        GUI.BeginGroup(new Rect(Screen.width / 2.0f - groupWidth / 2.0f, Screen.height / 2.0f - groupHeight / 2.0f, groupWidth, groupHeight));

        playerList = GameManager.gameManager.PlayerNames;
        foreach (string s in playerList) {
            GUI.Label(new Rect(infoX + (infoXStride * index), infoY + (infoYStride * index), infoWidth, infoHeight), s, infoStyle);
            index++;
        }

        if (GUI.Button(leaveMatchRect, leaveMatchContent, buttonStyle)) {
            GameManager.gameManager.ClientController.Send(DataType.JOINGAME, "lobby");
        }

        if (GUI.Button(logoutRect, logoutContent, buttonStyle)) {
            GameManager.gameManager.ClientController.Send(DataType.JOINGAME, "lobby");
            GameManager.gameManager.ClientController.Logout();
            GameManager.gameManager.ClientController.Disconnect();
            Application.LoadLevel("launch");
        }

        GUI.EndGroup();
        
    }
}
