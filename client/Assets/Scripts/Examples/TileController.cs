using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

	public enum TileState {
		EMPTY = 0,
		CROSS,
		RING
	};
	private TileState currentTileState;
	private bool playerHasCross;
	public GameObject crossObject;
	public GameObject ringObject;
	private GameObject myObject;
	private TrisGame gameInstance;

	private bool clickEnabled;

	// Use this for initialization
	public void Reset (TrisGame instance, bool playerHasCross) {
		gameInstance = instance;
		//currentTileState = TileState.EMPTY;
		SetTileState(TileState.EMPTY);
		this.playerHasCross = playerHasCross;
		clickEnabled = false;
	}

	public void Enable(bool enabled) {
		clickEnabled = enabled;
	}

	private void OnMouseDown() {
		if ( !clickEnabled ) return;

		if ( currentTileState == TileState.EMPTY ) {
			if (playerHasCross) {
				SetTileState(TileState.CROSS);
			} else {
				SetTileState(TileState.RING);
			}
			gameInstance.PlayerMoveMade(int.Parse(this.transform.name.Substring(4, 1)), int.Parse(this.transform.name.Substring(5, 1)));
		}
	}

	public void SetEnemyMove() {
		if ( currentTileState == TileState.EMPTY ) {
			if ( playerHasCross ) {
				SetTileState(TileState.RING);
			} else {
				SetTileState(TileState.CROSS);
			}
		}
	}

	private void SetTileState(TileState newState) {
		currentTileState = newState;
		if ( newState == TileState.CROSS ) {
			myObject = (GameObject)Instantiate(crossObject);
			Vector3 pos = this.transform.position;
			myObject.transform.position = pos;
		} else if ( newState == TileState.RING ) {
			myObject = (GameObject)Instantiate(ringObject);
			Vector3 pos = this.transform.position;
			myObject.transform.position = pos;
		}
		else if ( newState == TileState.EMPTY ) {
			if (myObject!=null) Destroy(myObject);
		} else {
			myObject = null;
		}
	}
}
