using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISetupable {
    void Initalize();
}

public interface ILoopable {
    void Progress();
}