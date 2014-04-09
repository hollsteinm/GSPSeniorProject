using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RegistrationGUI : MonoBehaviour, IEventListener {
    private string username = "";
    private string password = "";
    private string email = "";
    private string message = "";
    private IClientController server;

    public Rect usernameRect;
    public Rect passwordRect;
    public Rect emailRect;
    public Rect registerRect;
    public Rect backRect;
    public Rect messageRect;

    public GUIContent registerContent;
    public GUIContent backContent;

    public GUIStyle registrationStyle;
    public GUIStyle messageStyle;
    public float groupWidth;
    public float groupHeight;


	// Use this for initialization
	void Start () {
        server = GameManager.gameManager.ClientController;
        server.Register(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {
        GUI.BeginGroup(new Rect(Screen.width / 2.0f - groupWidth / 2.0f, Screen.height / 2.0f - groupHeight / 2.0f, groupWidth, groupHeight));
        DrawUsername();
        DrawEmail();
        DrawPassword();

        if (GUI.Button(registerRect, registerContent, registrationStyle)) {
            if (!username.Equals("") && !password.Equals("") && !email.Equals("")) {
                message = "Submitting...";

                //need to temporarily log in, then in event listener, submit data.
                server.Login("", "");
            } else {
                message = "You must fill in all fields for registration.";
            }
        }

        if (GUI.Button(backRect, backContent, registrationStyle)) {
            Application.LoadLevel("login");
        }

        GUI.TextArea(messageRect, message, messageStyle);
        GUI.EndGroup();
    }

    private void DrawUsername() {
        username = GUI.TextField(usernameRect, username, 64);
    }

    private void DrawEmail() {
        email = GUI.TextField(emailRect, email, 128);
    }

    private void DrawPassword() {
        password = GUI.PasswordField(passwordRect, password, '*', 64);
    }

    public void Notify(string eventType, object o) {
        switch (eventType) {
            case "register":
                message = (string)o;
                break;

            case "login.success":
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("username", username);
                data.Add("password", password);
                data.Add("email", email);
                print(username);
                print(password);
                print(email);
                server.Send(DataType.REGISTER, data);
                server.Logout();
                break;

            default:
                break;
        }
    }
}
