using UnityEngine;
using System.Collections;

public class LoginGUI : MonoBehaviour, IEventListener {
    private IClientController server = null;
    private string username = "Username";
    private string password = "";

    public GUIStyle loginStyle;

    public Rect usernameRect;
    public Rect passwordRect;
    public Rect loginRect;
    public Rect guestRect;
    public Rect registerRect;
    public Rect backRect;

    public GUIContent loginContent;
    public GUIContent guestContent;
    public GUIContent registerContent;
    public GUIContent backContent;
    public float groupWidth;
    public float groupHeight;

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

    public AudioSource click;
    void OnGUI ( ) {
        GUI.BeginGroup(new Rect(Screen.width / 2.0f - groupWidth / 2.0f, Screen.height / 2.0f - groupHeight / 2.0f, groupWidth, groupHeight));

        UsernameField();
        PasswordField();

        if (GUI.Button(backRect, backContent, loginStyle)) {
            click.Play();
            Application.LoadLevel("launch");
        }

        if ( GUI.Button ( loginRect, loginContent, loginStyle ) ) {
            if (!username.Equals("") && !username.Equals("")) {
                click.Play();
                server.Login(username, password);
            }
        }

        if (GUI.Button(guestRect, guestContent, loginStyle)) {
            click.Play();
            server.Login("", "");
        }

        if (GUI.Button(registerRect, registerContent, loginStyle)) {
            click.Play();
            Application.LoadLevel("register");
        }

        GUI.EndGroup();
    }

    private void UsernameField() {
        username = GUI.TextField(usernameRect, username, 64);
    }

    private void PasswordField() {
        password = GUI.PasswordField(passwordRect, password, '*', 64);
    }
}
