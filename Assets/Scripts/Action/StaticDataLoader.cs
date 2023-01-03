using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticDataLoader : MonoBehaviour
{
    public string statusInfoPath = "Assets\\Data\\StaticData\\StatusInfo.xml";
    public string buffInfoPath = "Assets\\Data\\StaticData\\BuffInfo.xml";
    public string keyPresetPath = "Assets\\Data\\StaticData\\ActionKeyPreset.xml";
    public string weightRandomPath = "Assets\\Data\\StaticData\\ActionKeyPreset.xml";
    
    void Awake()
    {
        // StatusInfo.setStatusInfoDataDictionary(StatusInfoLoader.readFromXML(statusInfoPath));
        // StatusInfo.setBuffDataDictionary(BuffDataLoader.readFromXML(buffInfoPath));
        ActionKeyInputManager.GetInstance().setPresetData(ActionKeyPresetDataLoader.readFromXML(keyPresetPath));
        // WeightRandomManager.Instance().setWeightGroupData(WeightRandomExporter.readFromXML(weightRandomPath));

    }
}
