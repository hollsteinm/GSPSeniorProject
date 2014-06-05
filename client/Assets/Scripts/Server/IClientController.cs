using UnityEngine;
using System.Collections;

public interface IClientController : IClient, IEventMessenger{
    void EvacuateTheDanceFloor();
}