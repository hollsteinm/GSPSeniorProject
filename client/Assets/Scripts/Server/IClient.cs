using UnityEngine;
using System.Collections;
public interface IClient {
    void Login(string username, string password);
    void Disconnect();
    void Connect(string server, int port);
    void SendRequest(string name, object data);
    void Update();
    void OnResponse();

    string Username {
        get;
    }

    string Server {
        get;
    }

    int Port {
        get;
    }

    string Room {
        get;
    }

    string Zone {
        get;
    }
}