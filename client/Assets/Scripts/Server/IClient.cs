using UnityEngine;
using System.Collections;

//TODO: add other data that can be sent
[System.Serializable]
public enum DataType{
    TRANSFORM = 0,
    CHARMESSAGE,
    ROOMREQUEST,
    ROOMJOIN
};

public interface IClient {
    void Login(string username, string password);
    void Disconnect();
    void Connect(string server, int port);
    void Send(DataType type, object data);
    void Update();

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