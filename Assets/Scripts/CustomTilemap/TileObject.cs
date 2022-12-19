using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : IGridObject {
    int _tileIndex;
    public TileObject() {
        _tileIndex = -1;
    }
    public void SetIndex(int index) {
        _tileIndex = index;
    }
    public int GetIndex() {
        return _tileIndex;
    }
}