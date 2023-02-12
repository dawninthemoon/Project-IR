using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class ManagedResourceItem<Value> where Value : class
{
	public Dictionary<string, Value> _singleResourceContainer = new Dictionary<string, Value>();
	public Dictionary<string, Value[]> _multiResourceContainer = new Dictionary<string, Value[]>();
	public Type _resourceType = typeof(Value);
	
	public Value GetOrLoadResource(string path)
	{
		if(_singleResourceContainer.ContainsKey(path))
			return _singleResourceContainer[path];
		
		Value obj = Load(path,GetResourceType()) as Value;
		if(obj == null)
		{
            DebugUtil.assert(false, "file does not exist : {0}",path);
			return null;
		}

		_singleResourceContainer.Add(path,obj);
		return obj;
	}

	public Value[] GetOrLoadResources(string path)
	{
		if(_multiResourceContainer.ContainsKey(path))
			return _multiResourceContainer[path];

		UnityEngine.Object[] obj = LoadAll(path, GetResourceType());
		if(obj.Length == 0)
		{
			DebugUtil.assert(false, "file does not exist : {0}",path);
			return null;
		}

		Value[] items = new Value[obj.Length];
		for(int i = 0; i < obj.Length; ++i)
		{
			items[i] = obj[i] as Value;
		}

		_multiResourceContainer.Add(path,items);

		return items;
	}

	public Type GetResourceType()
	{
		return _resourceType;
	}

	public UnityEngine.Object Load(string path, Type type)
	{
		return Resources.Load(path, type);
	}

	public UnityEngine.Object[] LoadAll(string path, Type type)
	{
		return Resources.LoadAll(path,type);
	}
}

public class DataResourceItem<Value, Loader> where Value : class where Loader : LoaderBase<Value>, new()
{
	class ValueWithTimeStamp
	{
		public Value 		_value;
		public DateTime 	_timeStamp;
	}

#if UNITY_EDITOR
	private Dictionary<string, ValueWithTimeStamp> _resourceContainer = new Dictionary<string, ValueWithTimeStamp>();
#else
	private Dictionary<string, Value> _resourceContainer = new Dictionary<string, Value>();
#endif

	private Loader loader = new Loader();

	public Value GetOrLoadResource(string path)
	{
#if UNITY_EDITOR
		DateTime timeStamp = getTimeStamp(path);
		if(_resourceContainer.ContainsKey(path))
		{
			if(_resourceContainer[path]._timeStamp == timeStamp)
				return _resourceContainer[path]._value;
		}
#else
		if(_resourceContainer.ContainsKey(path))
			return _resourceContainer[path];
#endif

		Value obj = loader.readFromXML(path);
		if(obj == null)
			return null;
#if UNITY_EDITOR
		if(_resourceContainer.ContainsKey(path))
		{
			ValueWithTimeStamp item = _resourceContainer[path];
			item._value = obj;
			item._timeStamp = timeStamp;
		}
		else
		{
			_resourceContainer.Add(path,new ValueWithTimeStamp(){_value = obj, _timeStamp = timeStamp});
		}
#else
		_resourceContainer.Add(path,obj);
#endif

		return obj;
	}

	private DateTime getTimeStamp(string path)
	{
        if (File.Exists(path) == false)
            return DateTime.MinValue;

        return File.GetLastWriteTime(path);
	}
}

public class ResourceContainerEx : Singleton<ResourceContainerEx>
{
    private ManagedResourceItem<Sprite> 				_spriteResource = new ManagedResourceItem<Sprite>();
	private ManagedResourceItem<ScriptableObject> 		_scriptableResource = new ManagedResourceItem<ScriptableObject>();
	private ManagedResourceItem<GameObject>		 		_prefabResource = new ManagedResourceItem<GameObject>();

	private DataResourceItem<ActionGraphBaseData,ActionGraphLoader>				_actionGraphResource = new DataResourceItem<ActionGraphBaseData,ActionGraphLoader>();
	private DataResourceItem<AIGraphBaseData,AIGraphLoader>						_aiGraphResource = new DataResourceItem<AIGraphBaseData,AIGraphLoader>();

	public ScriptableObject GetScriptableObject(string fileName)
	{
		return _scriptableResource.GetOrLoadResource(fileName);
	}

	public Sprite GetSprite(string fileName)
	{
		return _spriteResource.GetOrLoadResource(fileName);
	}

	public Sprite[] GetSpriteAll(string folderName)
	{
		string cut = folderName.Substring(folderName.IndexOf("Resources") + 10);
		
		return _spriteResource.GetOrLoadResources(cut);
	}

	public GameObject GetPrefab(string fileName)
	{
		return _prefabResource.GetOrLoadResource(fileName);
	}

	public ActionGraphBaseData GetActionGraph(string path)
	{
		return _actionGraphResource.GetOrLoadResource(path);
	}

	public AIGraphBaseData GetAIGraph(string path)
	{
		return _aiGraphResource.GetOrLoadResource(path);
	}

	public void UnLoadUnused()
	{
		Resources.UnloadUnusedAssets();
	}

	public void UnLoad(UnityEngine.Object obj)
	{
		Resources.UnloadAsset(obj);
	}
}