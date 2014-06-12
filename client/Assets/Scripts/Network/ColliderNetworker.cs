using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ENetworkColliderType{
    PROJECTILE,
    PLAYER,
    OBSTACLE,
    POWERUP
}

public class ColliderNetworker : BaseNetworker {
    private bool bHasCollided = false;
    private Dictionary<string, object> data = null;
    public ENetworkColliderType type;
    
    protected override void _Start(){
        SendDataType = DataType.SA_COLLISION;
    }
    

    protected override bool TriggerSendData()
    {
        return bHasCollided && data != null;
    }

    protected override object PrepareSendData()
    {
        return data;
    }

    void OnCollisionEnter(Collision collision){
        //See if we collided with a collider network communication type object
        ColliderNetworker other = collision.gameObject.GetComponent<ColliderNetworker>();
        if (other != null)
        {
            //send signal
            bHasCollided = true;

            //refresh the data
            data = new Dictionary<string, object>();

            //fill the data
            data.Add("other.networkId", other.NetworkID);
            data.Add("self.networkId", NetworkID);
            data.Add("other.type", other.type);
            data.Add("self.type", type);
            data.Add("contacts", collision.contacts[0]);
            data.Add("distance", Vector3.Distance(transform.root.position, other.transform.root.position));
        }
    }

    void OnCollisionStay(Collision collision)
    {
        OnCollisionEnter(collision);
    }
    
    void OnCollisionExit(Collision collision){
        data = null;
        bHasCollided = false;
    }
}
