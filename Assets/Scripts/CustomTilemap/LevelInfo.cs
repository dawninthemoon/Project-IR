using UnityEngine;

[System.Serializable]
public class LevelInfo {
    public int width;
    public int height;
    public string levelName;
    public Vector2 originPosition;
    public TilesetInfo[] tilesets;
    public EntityInfo[] entities;
    public ColliderInfo[] colliders;
    public LevelInfo(string n, int w, int h, Vector2 o, TilesetInfo[] t, EntityInfo[] e, ColliderInfo[] c) {
        levelName = n;
        width = w;
        height = h;
        originPosition = o;
        tilesets = t;
        entities = e;
        colliders = c;
    }
}

[System.Serializable]
public class TilesetInfo {
    public TileInfo[] tileInfo;
    public string tilesetName;
    public TilesetInfo(TileInfo[] i, string n) {
        tileInfo = i;
        tilesetName = n;
    }
}

[System.Serializable]
public class TileInfo {
    public int row;
    public int column;
    public int textureIndex;
    public TileInfo(int r, int c, int i) {
        row = r;
        column = c;
        textureIndex = i;
    }
}

[System.Serializable]
public class EntityInfo {
    public int row;
    public int column;
    public string name;
    public FieldInfo[] fields;
    public EntityInfo(int r, int c, string n, FieldInfo[] f) {
        row = r;
        column = c;
        name = n;
        fields = f;
    }
}

[System.Serializable]
public class ColliderInfo {
    public string tag;
    public Vector2[] points;
    public ColliderInfo(string t, Vector2[] p) {
        tag = t;
        points = p;
    }
}

[System.Serializable]
public class FieldInfo {
    public string name;
    public string value;
    public FieldInfo(string n, string v) {
        name = n;
        value = v;
    }
}