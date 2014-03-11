using UnityEngine;
using System.Collections;

public interface IEventListener{
    void Notify(string eventType, object o);
}
