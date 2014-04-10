using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour {
    public float effectLength;
    public Camera camera;
    public bool isClient;

	// Use this for initialization
	void Start () {
        Destroy ( gameObject, effectLength );
        Destroy ( this, effectLength );
        if (isClient) {
            camera = (Camera)Instantiate(camera, transform.position, transform.rotation);
            camera.GetComponent<SASmoothFollowLook>().target = transform;
        }
	}

    void OnDestroy ( ) {
        if (isClient) {
            GameManager.gameManager.ClientController.Send(DataType.JOINGAME, "lobby");
        } else {
            gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        transform.localScale += new Vector3 ( transform.localScale.x + 0.0025f * Time.deltaTime,
            transform.localScale.y + 0.0025f * Time.deltaTime,
            transform.localScale.z + 0.0025f * Time.deltaTime );
	
	}
}
