using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RegistrationGUI : MonoBehaviour, IEventListener {
    private string username = "";
    private string password = "";
    private string email = "";
    private string message = "";
    private IClientController server;

	// Use this for initialization
	void Start () {
        server = GameManager.gameManager.ClientController;
        server.Register(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {
        DrawUsername();
        DrawEmail();
        DrawPassword();

        if (GUI.Button(new Rect(0, 224, Screen.width, 64), "Register")) {
            if (!username.Equals("") && !password.Equals("") && !email.Equals("")) {
                message = "Submitting...";

                //need to temporarily log in, then in event listener, submit data.
                server.Login("", "");
            } else {
                message = "You must fill in all fields for registration.";
            }
        }

        if (GUI.Button(new Rect(0, 288, Screen.width, 64), "Back")) {
            Application.LoadLevel("login");
        }

        GUI.TextArea(new Rect(0, 352, Screen.width, 64), message);
    }

    private void DrawUsername() {
        username = GUI.TextField(new Rect(0, 32, Screen.width, 64), username, 64);
    }

    private void DrawEmail() {
        email = GUI.TextField(new Rect(0, 96, Screen.width, 64), email, 64);
    }

    private void DrawPassword() {
        password = GUI.PasswordField(new Rect(0, 160, Screen.width, 64), password, '*', 64);
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
