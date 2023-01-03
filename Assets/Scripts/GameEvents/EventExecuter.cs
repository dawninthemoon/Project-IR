using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class EventExecuter {
    EventCommand.SharedData _sharedData;
    EventCommand.SharedVariable _shraedVariable;
    Dictionary<string, EventCommand.EventCommandInterface> _stringCommandDic;

    public void Initalize(EventCommand.SharedData sharedData) {
        var nestedType = typeof(EventCommand).GetNestedTypes(System.Reflection.BindingFlags.Public);

        _sharedData = sharedData;
        _shraedVariable = new EventCommand.SharedVariable();

        string interfaceName = "EventCommandInterface";
        _stringCommandDic = nestedType
            .Where(type => (type.GetInterface(interfaceName) != null))
            .Select(type => type)
            .ToDictionary(type => type.Name, type => Activator.CreateInstance(type) as EventCommand.EventCommandInterface);
    }

    public IEnumerator ExecuteEvent(EntityInfo entity) {
        if (_stringCommandDic.TryGetValue(entity.name, out EventCommand.EventCommandInterface command)) {
            _sharedData.ExecuterEntity = entity;
            IEnumerator coroutine = command.Execute(_sharedData, _shraedVariable);
            
            while (coroutine.MoveNext()) {
                var nestCoroutine = coroutine?.Current as YieldInstruction;
                yield return nestCoroutine;
            }
        }
    }
}