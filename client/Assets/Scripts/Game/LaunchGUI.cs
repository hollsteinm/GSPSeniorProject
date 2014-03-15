using UnityEngine;
using System.Collections;

public class LaunchGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI ( ) {
        if ( GUI.Button ( new Rect ( 0, 32, Screen.width, 64 ), "Single Player" ) ) {
            GameManager.gameManager.gameType = GameType.SINGLEPLAYER;
            Application.LoadLevel ( "singleplayer" );
        }

        if ( GUI.Button ( new Rect ( 0, 128, Screen.width, 64 ), "Multi Player" ) ) {
            GameManager.gameManager.gameType = GameType.MULTIPLAYER;
            Application.LoadLevel ( "login" );
        }

        if (GUI.Button(new Rect(0, 224, Screen.width, 64), "High Scores")) {
            GameManager.gameManager.gameType = GameType.MULTIPLAYER;
            Application.LoadLevel("highscores");
        }

        if (GUI.Button(new Rect(0, 320, Screen.width, 64), "Credits")) {
            GameManager.gameManager.gameType = GameType.SINGLEPLAYER;
            Application.LoadLevel("credits");
        }

        if (GUI.Button(new Rect(0, 416, Screen.width, 64), "Quit")) {
            Application.Quit();
        }
    }
}
