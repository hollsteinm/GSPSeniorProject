using UnityEngine;
using System.Collections;

public interface IEventMessenger{
    void Register(IEventListener listener);
    void Unregister(IEventListener listener);
    void OnEvent(string eventType, object o);
}
