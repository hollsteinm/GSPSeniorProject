using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Logging;

public class GameGUI : MonoBehaviour {
	private SmartFox smartFox;
	private bool shuttingDown = false;

	public enum GameState {
		WAITING_FOR_PLAYERS = 0,
		RUNNING,
		GAME_WON,
		GAME_LOST,
		GAME_TIE,
		GAME_DISRUPTED
	};
	private TrisGame trisGameInstance;
	private ChatWindow chatWindow = null;
	private GameState currentGameState;

	public GUISkin gSkin;
	
	private bool started = false;

	/************
	 * Unity callback methods
	 ************/

	void OnApplicationQuit() {
		shuttingDown = true;
	}

	void FixedUpdate() {
		if (!started) return;
		smartFox.ProcessEvents();
	}

	void Awake() {
		Application.runInBackground = true;

		if ( SmartFoxConnection.IsInitialized ) {
			smartFox = SmartFoxConnection.Connection;
		} else {
			Application.LoadLevel("login");
			return;
		}

		// Register callbacks
		smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		smartFox.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
		smartFox.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
		smartFox.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserLeaveRoom);
		smartFox.AddEventListener(SFSEvent.USER_COUNT_CHANGE, OnUserCountChange);
		smartFox.AddEventListener(SFSEvent.OBJECT_MESSAGE, OnObjectReceived);
		smartFox.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);

		currentGameState = GameState.WAITING_FOR_PLAYERS;
		
		chatWindow = new ChatWindow();
		
		trisGameInstance = new TrisGame();
		trisGameInstance.InitGame(smartFox);
		
		started = true;
	}

	void StartGame() {
		chatWindow.AddSystemMessage("Game started! May the best man win");
		currentGameState = GameState.RUNNING;
	}

	void OnGUI() {
		if (!started) return;
		GUI.skin = gSkin;
		
		DrawGameGUI();
	}
	
	void DrawGameGUI() {
		float gamePanelWidth = Screen.width / 4 - 10;
		float gamePanelHeight = Screen.height / 3 - 10;
		float gamePanelPosX = Screen.width * 3/4;
		float gamePanelPosY = 10;
		
		float chatPanelWidth = gamePanelWidth;
		float chatPanelHeight = Screen.height - gamePanelHeight - 100;
		float chatPanelPosX = gamePanelPosX;
		float chatPanelPosY = gamePanelPosY + gamePanelHeight + 10;
										
		GUILayout.BeginArea(new Rect(gamePanelPosX, gamePanelPosY, gamePanelWidth, gamePanelHeight));
		GUILayout.Box ("Tris", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		GUILayout.BeginVertical();
		GUILayout.BeginArea(new Rect(20, 25, gamePanelWidth-40, gamePanelHeight-80), GUI.skin.customStyles[0]);
		if (smartFox != null && smartFox.LastJoinedRoom!=null) {
			GUILayout.Label("Current room: " + smartFox.LastJoinedRoom.Name);
			if (currentGameState == GameState.RUNNING ) {
				GUILayout.Label(trisGameInstance.GetGameStatus());
			}
		}
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(20, 25 + gamePanelHeight - 70, gamePanelWidth-40, 40));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Exit") ) {
			smartFox.Send(new JoinRoomRequest("The Lobby", null, smartFox.LastJoinedRoom.Id));
			trisGameInstance.DestroyGame();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea ();
						
		GUILayout.EndVertical();
		GUILayout.EndArea ();
		
		// Chat
		chatWindow.Draw(chatPanelPosX, chatPanelPosY, chatPanelWidth, chatPanelHeight);
		
		// Print the current game state
		if (currentGameState == GameState.WAITING_FOR_PLAYERS ) {
			ShowSimplePopup("Waiting", "Waiting for player to join");
		}
		if (currentGameState == GameState.GAME_DISRUPTED ) {
			ShowSimplePopup("Game Over", "Enemy player disconnected");

		} else if ( currentGameState == GameState.GAME_LOST ) {
			ShowGameOverPopup("Game Over", "You lost");

		} else if ( currentGameState == GameState.GAME_WON ) {
			ShowGameOverPopup("Game Over", "You win!!");

		} else if ( currentGameState == GameState.GAME_TIE ) {
			ShowGameOverPopup("Game Over", "It is a tie!!");
		} 	
	}

	public void SetGameOver(string result) {
		chatWindow.AddSystemMessage("Game over");
		if ( result == "win" ) {
			currentGameState = GameState.GAME_WON;
			chatWindow.AddSystemMessage("Result: Win");
		} else if ( result == "loss" ) {
			currentGameState = GameState.GAME_LOST;
			chatWindow.AddSystemMessage("Result: Loss");
		} else {
			currentGameState = GameState.GAME_TIE;
			chatWindow.AddSystemMessage("Result: Tie");
		}
	}

	public void SetStartGame() {
		currentGameState = GameState.RUNNING;
	}

	/************
	 * Helper methods
	 ************/

	private void UnregisterSFSSceneCallbacks() {
		// This should be called when switching scenes, so callbacks from the backend do not trigger code in this scene
		smartFox.RemoveEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		smartFox.RemoveEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
		smartFox.RemoveEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
		smartFox.RemoveEventListener(SFSEvent.USER_EXIT_ROOM, OnUserLeaveRoom);
		smartFox.RemoveEventListener(SFSEvent.USER_COUNT_CHANGE, OnUserCountChange);
		smartFox.RemoveEventListener(SFSEvent.OBJECT_MESSAGE, OnObjectReceived);
		smartFox.RemoveEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
	}

	private void ShowSimplePopup(string header, string text) {
		// Lets just quickly set up some GUI layout variables
		float panelWidth = 300;
		float panelHeight = 200;
		float panelPosX = Screen.width/2 - panelWidth/2;
		float panelPosY = Screen.height/2 - panelHeight/2;
		
		// Draw the box
		GUILayout.BeginArea(new Rect(panelPosX, panelPosY, panelWidth, panelHeight));
		GUILayout.Box (header, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		GUILayout.BeginVertical();
		GUILayout.BeginArea(new Rect(20, 25, panelWidth-40, panelHeight-60), GUI.skin.customStyles[0]);
		
		// Center label
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
			
		GUILayout.Label(text);
			
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea ();		
		GUILayout.EndVertical();
		GUILayout.EndArea ();	
	}

	private void ShowGameOverPopup(string header, string text) {
		// Lets just quickly set up some GUI layout variables
		float panelWidth = 300;
		float panelHeight = 200;
		float panelPosX = Screen.width/2 - panelWidth/2;
		float panelPosY = Screen.height/2 - panelHeight/2;
		
		// Draw the box
		GUILayout.BeginArea(new Rect(panelPosX, panelPosY, panelWidth, panelHeight));
		GUILayout.Box (header, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		GUILayout.BeginVertical();
		GUILayout.BeginArea(new Rect(20, 25, panelWidth-40, panelHeight-60), GUI.skin.customStyles[0]);
		
		// Center label
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		GUILayout.Label(text);
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea ();	
		
		GUILayout.BeginArea(new Rect(20, panelHeight - 35, panelWidth - 40, 40));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		if (GUILayout.Button("Restart")) {
			trisGameInstance.RestartGame();
			currentGameState = GameState.RUNNING;
			// Send "Lets restart" message to other player
			SFSObject restartObject = new SFSObject();
			restartObject.PutUtfString("cmd", "restart");
			smartFox.Send(new ObjectMessageRequest(restartObject));
			trisGameInstance.ResetGameBoard();			
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea ();
								
		GUILayout.EndVertical();
		GUILayout.EndArea ();	
	}

	/************
	 * Callbacks from the SFS API
	 ************/

	private void OnConnectionLost(BaseEvent evt) {
		UnregisterSFSSceneCallbacks();
		if ( shuttingDown == true ) return;
		Application.LoadLevel("login");
	}

	private void OnPublicMessage(BaseEvent evt) {
		string message = (string)evt.Params["message"];
		User sender = (User)evt.Params["sender"];
		chatWindow.AddChatMessage(sender.Name + " said: " + message);
	}

	private void OnJoinRoom(BaseEvent evt) {
		Room room = (Room)evt.Params["room"];
		Debug.Log("Joining lobby room " + room.Name);
		started = false;
		UnregisterSFSSceneCallbacks();
		Application.LoadLevel("lobby");
	}

	private void OnUserEnterRoom(BaseEvent evt) {
		User user = (User)evt.Params["user"];
		chatWindow.AddPlayerJoinMessage(user.Name + " joined room");
	}

	private void OnUserLeaveRoom(BaseEvent evt) {
		User user = (User)evt.Params["user"];
		chatWindow.AddPlayerLeftMessage(user.Name + " left room");
		currentGameState = GameState.GAME_DISRUPTED;
	}

	private void OnObjectReceived(BaseEvent evt) {
		SFSObject obj = (SFSObject)evt.Params["message"];
		User sender = (User)evt.Params["sender"];
		
		switch ( obj.GetUtfString("cmd") ) {
			case "restart":
				currentGameState = GameState.RUNNING;
				break;
		}
	}

	private void OnUserCountChange(BaseEvent evt) {
		Room room = (Room)evt.Params["room"];
		if ( room.UserCount == 2 ) {
			StartGame();
		}
	}
}
