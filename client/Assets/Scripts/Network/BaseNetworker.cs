using UnityEngine;
using System.Collections;

public enum ECommunicationType
{
    SEND_ONLY,
    RECEIVE_ONLY,
    SEND_AND_RECEIVE,
    SEND_ONLY_REPEAT,
    SEND_REPEAT_AND_RECEIVE
}

public abstract class BaseNetworker : MonoBehaviour, IEventListener {
    protected virtual void _Start(){}
    protected virtual void _OnDestroy(){}
    protected virtual void _Update(){}
    public virtual void Notify(string eventType, object o) { }

    protected abstract object PrepareSendData();
    protected abstract bool TriggerSendData();

    public ECommunicationType MessageType;
    public DataType SendDataType;
    public float SendDelay;
    public float InitialSendDelay;

    public int NetworkID;

    void Start()
    {
        if (Receive())
        {
            GameManager.gameManager.ClientController.Register(this);
        }
        if (SendRepeat())
        {
            InvokeRepeating("SendData", InitialSendDelay, SendDelay);
        }
    
        _Start();
    }

    private bool Send()
    {
        return MessageType == ECommunicationType.SEND_ONLY || MessageType == ECommunicationType.SEND_AND_RECEIVE;
    }

    private bool SendRepeat()
    {
        return MessageType == ECommunicationType.SEND_ONLY_REPEAT || MessageType == ECommunicationType.SEND_REPEAT_AND_RECEIVE;
    }

    private bool Receive()
    {
        return MessageType == ECommunicationType.RECEIVE_ONLY || MessageType == ECommunicationType.SEND_AND_RECEIVE;
    }

    private void SendData()
    {
        if (TriggerSendData())
        {
            GameManager.gameManager.ClientController.Send(SendDataType, PrepareSendData());
        }
    }

    void OnDestroy()
    {
        if (SendRepeat())
        {
            CancelInvoke();
        }
        _OnDestroy();
    }

    void Update()
    {
        _Update();
        if (TriggerSendData() && Send())
        {
            SendData();
        }
    }
}
