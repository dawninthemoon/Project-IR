using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>, ISetupable {
    public void Initialize() {

    }

    public T GetAsset<T>(string name) where T : Object {
        T asset = Resources.Load<T>(name);
        return asset;
    }
}
