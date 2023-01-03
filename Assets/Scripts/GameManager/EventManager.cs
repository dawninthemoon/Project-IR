using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : SingletonWithMonoBehaviour<EventManager> {
    EventExecuter _executer;
    public void Initalize(EventCommand.SharedData sharedData) {
        _executer = new EventExecuter();
        _executer.Initalize(sharedData);
    }

    public void OnRoomChanged(EntityInfo[] entities) {
        foreach (var entity in entities) {
            StartCoroutine(_executer.ExecuteEvent(entity));
        }
    }
}