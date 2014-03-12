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

        if ( GUI.Button ( new Rect ( 0, 128, Screen.width, 64 ), "Multiplayer" ) ) {
            GameManager.gameManager.gameType = GameType.MULTIPLAYER;
            Application.LoadLevel ( "login" );
        }
    }
}
