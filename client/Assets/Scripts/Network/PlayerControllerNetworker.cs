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
        if (Input.GetButtonDown("Fire1"))
        {
            bInput = true;
            command = EInputCommand.FIRE1;
        }
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0.0f)
        {
            bInput = true;
            if (horizontal > 0.0f)
            {
                command = EInputCommand.RIGHT;
            }
            else
            {
                command = EInputCommand.LEFT;
            }
            value = horizontal;
        }

        if (vertical != 0.0f)
        {
            bInput = true;
            if (vertical > 0.0f)
            {
                command = EInputCommand.UP;
            }
            else
            {
                command = EInputCommand.DOWN;
            }
            value = vertical;
        }

        float mousex = Input.GetAxis("Mouse X");
        float mousey = Input.GetAxis("Mouse Y");

        if (mousex != 0.0f)
        {
            bInput = true;
            command = EInputCommand.HORIZONTAL;
            value = mousex;
        }

        if (mousey != 0.0f)
        {
            bInput = true;
            command = EInputCommand.VERTICAL;
            value = mousey;

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

    public override void Notify(string eventType, object o)
    {
        switch (eventType)
        {
            case "player.spawn":
                Dictionary<string, object> data = o as Dictionary<string, object>;
                GameObject ship = (GameObject)Resources.Load((string)data["shipname"]);
                ship.transform.parent = gameObject.transform;
                
                break;

            default:
                break;
        }
    }
}
