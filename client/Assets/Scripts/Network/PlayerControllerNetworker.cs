using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControllerNetworker : BaseNetworker{
    [System.Serializable]
    public enum EInputCommand
    {
        NONE,
        LEFT,
        RIGHT,
        UP,
        DOWN,
        FIRE1,
        FIRE2,
        RELOAD,
        VERTICAL,
        HORIZONTAL,
        MANUEVER_VERTICAL,
        MANUEVER_HORIZONTAL
    };

    EInputCommand command;
    private bool bInput;
    private float value;

    protected override void _Start()
    {
        command = EInputCommand.NONE;
        SendDataType = DataType.SA_INPUT;
        MessageType = ECommunicationType.SEND_ONLY;
        bInput = false;
    }

    private void Refresh()
    {
        bInput = false;
        command = EInputCommand.NONE;
        value = 0.0f;
    }

    private void GetInput()
    {
        if (Input.GetButton("Fire1"))
        {
            bInput = true;
            command = EInputCommand.FIRE1;
        }
    }

    protected override void _Update()
    {
        Refresh();
        GetInput();
    }

    protected override object PrepareSendData()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("command", command.ToString().ToLower());
        data.Add("value", value);
        return data;
    }

    protected override bool TriggerSendData()
    {
        return bInput && command != EInputCommand.NONE;
    }
}
