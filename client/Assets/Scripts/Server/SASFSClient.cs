using UnityEngine;
using System.Collections;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Entities;
using Sfs2X;
using System.Collections.Generic;

public class SASFSClient : IClientController {
    public List<IEventListener> listeners = new List<IEventListener>();
    private SmartFox SFSInstance;

    private static SASFSClient client;
    private static object mutex = new object();

    private SASFSClient()
    {
        SFSInstance = SFSClient.Singleton.SmartFoxInstance;
        SFSInstance.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, SFSClient.Singleton.OnExtensionReponse);
        SFSInstance.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        string cmd = ( string ) evt.Params[ "cmd" ];
        SFSObject sfsdata = ( SFSObject ) evt.Params[ "params" ];

        Debug.Log(cmd + " : " + sfsdata.ToString());

        switch (cmd)
        {
            case "gamelist":
                SFSClient.Singleton.OnExtensionReponse(evt);
                break;

            case "projectile.spawn":
                OnProjectileSpawnResponse(sfsdata);
                break;

            case "projectile.update":
                OnProjectileUpdateResponse(sfsdata);
                break;

            case "transform":
                //eat
                break;

            case "player.hit":
                //eat
                break;

            case "death":
                //eat
                break;

            case "spawn":
                //eat
                break;

            case "$SignUp.Submit":
                SFSClient.Singleton.OnExtensionReponse(evt);
                break;

            case "scores":
                SFSClient.Singleton.OnExtensionReponse(evt);
                break;

            case "shoot":
                SFSClient.Singleton.OnExtensionReponse(evt);
                break;

            case "game.start":
                SFSClient.Singleton.OnExtensionReponse(evt);
                break;

            case "gamelist.remove":
                SFSClient.Singleton.OnExtensionReponse(evt);
                break;

            case "projectile.expire":
                //eat
                break;

            case "powerup":
                //eat
                break;

            default:
                break;
        }
        
    }

    //singleton caller
    public static SASFSClient Singleton
    {
        get
        {
            if (client == null)
            {
                lock (mutex)
                {
                    if (client == null)
                    {
                        client = new SASFSClient();
                    }
                }
            }
            return client;
        }
    }

    public void EvacuateTheDanceFloor()
    {
        listeners.Clear();
    }

    public void Login(string username, string password)
    {
        SFSClient.Singleton.Login(username, password);
    }

    public void Disconnect()
    {
        SFSClient.Singleton.Disconnect();
    }

    public void Connect(string server, int port)
    {
        SFSClient.Singleton.Connect(server, port);
    }

    public void Send(DataType type, object data)
    {
        switch (type)
        {
            case DataType.DEATH:
                //eat
                break;

            case DataType.FIRE:
                //eat
                break;

            case DataType.POWERUP:
                //eat
                break;

            case DataType.SHOOT:
                //eat
                break;

            case DataType.SPAWNED:
                //eat
                break;

            case DataType.TRANSFORM:
                //eat
                break;

            case DataType.SA_COLLISION:
                sendCollisionRequest(type, data);
                break;

            case DataType.SA_INPUT:
                sendInputRequest(type, data);
                break;

            default:
                SFSClient.Singleton.Send(type, data);
                break;
        }
    }

    public void Update()
    {
        SFSClient.Singleton.Update();
    }

    public void Logout()
    {
        SFSClient.Singleton.Logout();
    }

    public string Username
    {
        get
        {
            return SFSClient.Singleton.Username;
        }
    }

    public string Server
    {
        get { return SFSClient.Singleton.Server; }
    }

    public int Port
    {
        get { return SFSClient.Singleton.Port; }
    }

    public string Room
    {
        get { return SFSClient.Singleton.Room; }
    }

    public string Zone
    {
        get { return SFSClient.Singleton.Zone; }
    }

    public void Register(IEventListener listener)
    {
        listeners.Add(listener);
        SFSClient.Singleton.Register(listener);
    }

    public void Unregister(IEventListener listener)
    {
        listeners.Remove(listener);
        SFSClient.Singleton.Unregister(listener);
    }

    public void OnEvent(string eventType, object o)
    {
        foreach (IEventListener l in listeners)
        {
            l.Notify(eventType, o);
        }
    }

    //New Senders
    private string srvcmd(DataType dataType)
    {
        return "server." + dataType.ToString().ToLower().Trim();
    }

    private void _send(DataType dataType, SFSObject sfs)
    {
        SFSInstance.Send(new ExtensionRequest(srvcmd(dataType),
           sfs, SFSInstance.LastJoinedRoom));
    }

    private void sendCollisionRequest(DataType dataType, object data)
    {
        SFSObject sfs = new SFSObject();

        Dictionary<string, object> cdata = data as Dictionary<string, object>;

        int otherNetworkId = (int)cdata["other.networkId"];
        int selfNetworkId = (int)cdata["self.networkId"];
        string selftype = ((ENetworkColliderType)cdata["self.type"]).ToString().ToLower().Trim();
        string othertype = ((ENetworkColliderType)cdata["other.type"]).ToString().ToLower().Trim();
        float distance = (float)cdata["distance"];
        ContactPoint contacts = (ContactPoint)cdata["contacts"];
        
        sfs.PutInt("other.networkId", otherNetworkId);
        sfs.PutInt("self.networkId", selfNetworkId);
        sfs.PutUtfString("self.type", selftype);
        sfs.PutUtfString("other.type", othertype);
        sfs.PutFloat("distance", distance);
        sfs.PutFloat("contact.x", contacts.point.x);
        sfs.PutFloat("contact.y", contacts.point.y);
        sfs.PutFloat("contact.z", contacts.point.z);
        sfs.PutFloat("normal.x", contacts.normal.x);
        sfs.PutFloat("normal.y", contacts.normal.y);
        sfs.PutFloat("normal.z", contacts.normal.z);

        _send(dataType, sfs);
    }

    private void sendInputRequest(DataType dataType, object data)
    {
        SFSObject sfs = new SFSObject();
        Dictionary<string, object> cdata = data as Dictionary<string, object>;

        float value = (float)cdata["value"];
        string command = (string)cdata["command"];

        sfs.PutFloat("value", value);
        sfs.PutUtfString("command", command);

        _send(dataType, sfs);
    }

    private void OnProjectileSpawnResponse(SFSObject sfsdata){
        Debug.Log("YAY - PSpawn");
    }

    private void OnProjectileUpdateResponse(SFSObject sfsdata){
        Debug.Log("YAY - PUpdate");
    }
}
