using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceContainerEx : Singleton<ResourceContainerEx>
{
    private Dictionary<string ,Sprite> sprite = new Dictionary<string, Sprite>();
	private Dictionary<string ,Sprite[]> spriteSet = new Dictionary<string, Sprite[]>();

	private Dictionary<string ,ScriptableObject> _scriptableObject = new Dictionary<string, ScriptableObject>();

	private static string spritesFilePath = "Sprites/";
	private static string scriptableFilePath = "ScriptableObject/";

	private static Type _spriteType = typeof(Sprite);
	private static Type _scriptableType = typeof(ScriptableObject);

	public ScriptableObject GetScriptableObject(string fileName)
	{
		if(_scriptableObject.ContainsKey(fileName))
			return _scriptableObject[fileName];

		string path = scriptableFilePath + fileName;
		if(Load(path, _scriptableType) != null)
		{
			if(Load(path, _scriptableType) as ScriptableObject == null)
				DebugUtil.assert(false, "???");
		}

		ScriptableObject obj = Load(path,_scriptableType) as ScriptableObject;
		if(obj == null)
		{
            DebugUtil.assert(false, "file does not exist : {0}",path);
			return null;
		}
		_scriptableObject.Add(fileName,obj);

		return obj;
	}

	public Sprite GetSprite(string fileName)
	{
		if(sprite.ContainsKey(fileName) == true)
			return sprite[fileName];

		string path = spritesFilePath + fileName;

		if(Load(path, _spriteType) != null)
		{
			if(Load(path, _spriteType) as Sprite == null)
				DebugUtil.assert(false, "???");
		}

		Sprite obj = Load(path,_spriteType) as Sprite;
		if(obj == null)
		{
            DebugUtil.assert(false, "file does not exist : {0}",path);
			return null;
		}
		sprite.Add(fileName,obj);

		return obj;
	}

	public Sprite[] GetSpriteAll(string folderName, bool ignorePreload = false)
	{
		string cut = folderName.Substring(folderName.IndexOf("Resources") + 10);
		if(spriteSet.ContainsKey(cut) && ignorePreload == false)
			return spriteSet[cut];

		string path = cut;
		UnityEngine.Object[] obj = LoadAll(path, _spriteType);
		if(obj.Length == 0)
		{
			DebugUtil.assert(false, "file does not exist : {0}",cut);
			return null;
		}

		Sprite[] sprites = new Sprite[obj.Length];
		for(int i = 0; i < obj.Length; ++i)
		{
			sprites[i] = obj[i] as Sprite;
		}

		if(ignorePreload && spriteSet.ContainsKey(cut))
			spriteSet[cut] = sprites;
		else
			spriteSet.Add(cut,sprites);

		return sprites;
	}

	public Sprite[] GetSpriteSet(string folderName)
	{
		if(spriteSet.ContainsKey(folderName))
			return spriteSet[folderName];

		string path = spritesFilePath + folderName;
		UnityEngine.Object[] obj = LoadAll(path, _spriteType);
		if(obj.Length == 0)
		{
			DebugUtil.assert(false, "file does not exist : {0}",path);
			return null;
		}

		Sprite[] sprites = new Sprite[obj.Length];
		for(int i = 0; i < obj.Length; ++i)
		{
			sprites[i] = obj[i] as Sprite;
		}

		spriteSet.Add(folderName,sprites);

		return sprites;
	}

	public bool UnLoadSpriteSet(string fileName)
	{
		string path = spritesFilePath + fileName;
		if(sprite.ContainsKey(path))
		{
			Sprite[] res = spriteSet[path];
			spriteSet.Remove(path);
			for(int i = 0; i < res.Length; ++i)
				UnLoad(res[i]);

			return true;
		}

		return false;
	}

	public bool UnLoadSprite(string fileName)
	{
		string path = spritesFilePath + fileName;
		if(sprite.ContainsKey(path))
		{
			Sprite res = sprite[path];
			sprite.Remove(path);
			UnLoad(res);

			return true;
		}

		return false;
	}

	public void UnLoadUnused()
	{
		Resources.UnloadUnusedAssets();
	}

	public void UnLoad(UnityEngine.Object obj)
	{
		Resources.UnloadAsset(obj);
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