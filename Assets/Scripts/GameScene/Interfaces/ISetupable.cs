using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISetupable {
    void Initialize();
}

public interface ILoopable {
    void Progress();
}