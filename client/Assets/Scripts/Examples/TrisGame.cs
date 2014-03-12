/**
 * SmartFoxTris Example Application for SmartFoxServer PRO
 * Unity version
 * 
 * version 1.0.0
 * (c) gotoAndPlay() 2009
 * 
 * www.smartfoxserver.com
 * www.gotoandplay.it
 */

using UnityEngine;
using System;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;

public class TrisGame {

	private SmartFox sfs;
	private string extensionName;
	private string gameStatus;
	private int player1Id;
	private int player2Id;
	private string player1Name;
	private string player2Name;
	private int whoseTurn;
	private Boolean gameStarted;
	private int myPlayerID;

	public TrisGame() {
		extensionName = "tris";
	}

	/**
	 * Initialize the game
	 */
	public void InitGame(SmartFox smartFox) {
		gameStarted = false;

		// Register to SmartFox events
		sfs = smartFox;
		sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
		
		// Show stage
		gameStatus = "";

		// Setup my properties
		myPlayerID = sfs.MySelf.PlayerId;

		ResetGameBoard();

		// Tell extension I'm ready to play
		sfs.Send(new ExtensionRequest("ready", new SFSObject(), sfs.LastJoinedRoom));
	}

	/**
	 * Destroy the game instance
	 */
	public void DestroyGame() {
		sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
	}

	/**
	 * Start the game
	 */
	private void StartGame(int whoseTurn, int p1Id, int p2Id, string p1Name, string p2Name) {
		this.whoseTurn = whoseTurn;
		player1Id = p1Id;
		player2Id = p2Id;
		player1Name = p1Name;
		player2Name = p2Name;

		ResetGameBoard();

		SetTurn();
		EnableBoard(true);

		gameStarted = true;

		// Reset GUI if this is a restart
		GameGUI gui = (GameGUI)GameObject.Find("Game GUI").GetComponent("GameGUI");
		gui.SetStartGame();
	}

	/**
	 * Set the "Player's turn" status message
	 */
	private void SetTurn() {
		gameStatus = ( myPlayerID == whoseTurn ) ? "It's your turn" : "It's your opponent's turn";
	}

	/**
	 * Clear the game board
	 */
	public void ResetGameBoard() {
		for ( int i = 1; i <= 3; i++ ) {
			for ( int j = 1; j <= 3; j++ ) {
				GameObject tile = GameObject.Find("tile" + i + j);
				TileController ctrl = (TileController)tile.GetComponent("TileController");
				// Player 1 gets the ring
				if ( myPlayerID == 1 ) {
					ctrl.Reset(this, false);
				} else {
					ctrl.Reset(this, true);
				}
			}
		}
	}

	/**
	 * Enable board click
	 */
	private void EnableBoard(bool enable) {
		if ( myPlayerID == whoseTurn ) {
			for ( int i = 1; i <= 3; i++ ) {
				for ( int j = 1; j <= 3; j++ ) {
					GameObject tile = GameObject.Find("tile" + i + j);
					TileController ctrl = (TileController)tile.GetComponent("TileController");
					ctrl.Enable(enable);
				}
			}
		}
	}

	/**
	 * On board click, send move to other players
	 */
	public void PlayerMoveMade(int tileX, int tileY) {
		EnableBoard(false);

		SFSObject obj = new SFSObject();
		obj.PutInt("x", tileX);
		obj.PutInt("y", tileY);
		
		sfs.Send(new ExtensionRequest("move", obj, sfs.LastJoinedRoom));
	}

	/**
	 * Handle the opponent move
	 */
	private void MoveReceived(int movingPlayer, int x, int y) {
		whoseTurn = ( movingPlayer == 1 ) ? 2 : 1;

		if ( movingPlayer != myPlayerID ) {
			GameObject tile = GameObject.Find("tile" + x + y);
			TileController ctrl = (TileController)tile.GetComponent("TileController");
			ctrl.SetEnemyMove();
		}

		SetTurn();
		EnableBoard(true);
	}

	/**
	 * Declare game winner
	 */
	private void ShowWinner(string cmd, int winnerId) {
		gameStarted = false;
		gameStatus = "";

		GameGUI gui = (GameGUI)GameObject.Find("Game GUI").GetComponent("GameGUI");

		if ( cmd == "win" ) {
			if ( myPlayerID == winnerId ) {
				// I WON! In the next match, it will be my turn first
				gui.SetGameOver("win");
			} else {
				// I've LOST! Next match I will be the second to move
				gui.SetGameOver("loss");
			}
		} else if ( cmd == "tie" ) {
			gui.SetGameOver("tie");
		}
		EnableBoard(false);
	}

	/**
	 * Restart the game
	 */
	public void RestartGame() {
		sfs.Send(new ExtensionRequest("restart", new SFSObject(), sfs.LastJoinedRoom));
	}

	/**
	 * One of the players left the game
	 */

	private void UserLeft() {
		gameStarted = false;
		gameStatus = "";
	}

	public string GetGameStatus() {
		return gameStatus;
	}

	//------------------------------------------------------------------------------------

	public void OnExtensionResponse(BaseEvent evt) {
		string cmd = (string)evt.Params["cmd"];
		SFSObject dataObject = (SFSObject)evt.Params["params"];
		
		switch ( cmd ) {
			case "start":
				StartGame(dataObject.GetInt("t"),
					dataObject.GetInt("p1i"),
					dataObject.GetInt("p2i"),
					dataObject.GetUtfString("p1n"),
					dataObject.GetUtfString("p2n")
					);
				break;

			case "stop":
				UserLeft();
				break;

			case "move":
				MoveReceived(dataObject.GetInt("t"), dataObject.GetInt("x"), dataObject.GetInt("y"));
				break;

			case "win":
				ShowWinner(cmd, (int)dataObject.GetInt("w"));
				break;
					
			case "tie":
				ShowWinner(cmd, -1);
				break;
		}
	}
}
