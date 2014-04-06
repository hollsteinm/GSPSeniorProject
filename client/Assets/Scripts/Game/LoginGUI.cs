using UnityEngine;
using System.Collections;

public class LoginGUI : MonoBehaviour, IEventListener {
    private IClientController server = null;
    private string username = "Username";
    private string password = "";

	// Use this for initialization
	void Start () {
        server = GameManager.gameManager.ClientController;
        server.Register ( this );
        server.Connect ( "ec2-54-201-26-249.us-west-2.compute.amazonaws.com", 9933 );	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Notify ( string eventType, object o ) {
        switch(eventType){
            default:
                break;
        }
    }

    void OnGUI ( ) {

        UsernameField();
        PasswordField();

        if ( GUI.Button ( new Rect ( 0, 160, Screen.width, 64 ), "Login" ) ) {
            if (!username.Equals("") && !username.Equals("")) {
                server.Login(username, password);
            }
        }

        if (GUI.Button(new Rect(0, 224, Screen.width, 64), "Login As Guest")) {
            server.Login("", "");
        }

        if (GUI.Button(new Rect(0, 288, Screen.width, 64), "Register")) {
            Application.LoadLevel("register");
        }
    }

    private void UsernameField() {
        username = GUI.TextField(new Rect(0, 32, Screen.width, 64), username, 64);
    }

    private void PasswordField() {
        password = GUI.PasswordField(new Rect(0, 96, Screen.width, 64), password, '*', 64);
    }
}
