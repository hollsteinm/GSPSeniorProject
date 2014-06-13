using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementNetworker : BaseNetworker {
    protected override void _Start()
    {
        MessageType = ECommunicationType.RECEIVE_ONLY;
    }

    public override void Notify(string eventType, object o)
    {

        Dictionary<string, object> data = o as Dictionary<string, object>;
       // if ((int)data["networkid"] == NetworkID)
        //{
            switch (eventType)
            {
                case "player.update":
                    float px = (float)data["player.position.x"];
                    float py = (float)data["player.position.y"];
                    float pz = (float)data["player.position.z"];
                    float qx = (float)data["player.rotation.x"];
                    float qy = (float)data["player.rotation.y"];
                    float qz = (float)data["player.rotation.z"];
                    float qw = (float)data["player.rotation.w"];

                    transform.position = new Vector3(px, py, pz);
                    transform.rotation = new Quaternion(qx, qy, qz, qw);

                    break;

                case "projectile.update":
                    break;

                default:
                    break;
            }
        //}
    }


    protected override object PrepareSendData()
    {
        return null;
    }

    protected override bool TriggerSendData()
    {
        return false;
    }
}
