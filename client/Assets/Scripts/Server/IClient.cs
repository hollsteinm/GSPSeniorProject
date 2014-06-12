using UnityEngine;
using System.Collections;

//TODO: add other data that can be sent
[System.Serializable]
public enum DataType{
    TRANSFORM = 0,
    CHARMESSAGE,
    MAKEGAME,
    JOINGAME,
    SPAWNED,
    FIRE,
    DEATH,
    REGISTER,
    SCORES_GET,
    SHOOT,
    GAMES_GET,
    PLAYER_GAME_READY,
    POWERUP,
    //New Age Systems - keep deprecated for who knows what reason.
    SA_INPUT, //a user submitted command from input - translated to a single pair of string and float
    SA_COLLISION,
};

public interface IClient {
    void Login(string username, string password);
    void Disconnect();
    void Connect(string server, int port);
    void Send(DataType type, object data);
    void Update();
    void Logout();

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