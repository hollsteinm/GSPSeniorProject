using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour {
    public float effectLength;
    public Camera camera;

	// Use this for initialization
	void Start () {
        Destroy ( gameObject, effectLength );
        camera = (Camera)Instantiate ( camera, transform.position, transform.rotation );
        camera.GetComponent<SASmoothFollowLook> ().target = transform;
	}

    void OnDestroy ( ) {
        GameManager.gameManager.ClientController.Send ( DataType.JOINGAME, "lobby" );
    }
	
	// Update is called once per frame
	void Update () {
        transform.localScale += new Vector3 ( transform.localScale.x + 1.0f * Time.deltaTime,
            transform.localScale.y + 1.0f * Time.deltaTime,
            transform.localScale.z + 1.0f * Time.deltaTime );
	
	}
}
