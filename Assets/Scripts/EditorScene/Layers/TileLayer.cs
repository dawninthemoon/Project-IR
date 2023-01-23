using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTilemap;

namespace ProjectEditor {
    public class TileLayer : MonoBehaviour {
        public int DropdownValue { get; set; }
        public string TilesetName { get; set; }
        private CustomGrid<TileObject> _grid;
        private TilemapVisual _tilemapVisual;
        private List<List<TileObject>> _gridList;

        public TileLayer() {
            int width = EditorMain.CurrentGridWidth;
            int height = EditorMain.CurrentGridHeight;
            Vector3 pos = EditorMain.CurrentOriginPosition;
            _grid = new CustomGrid<TileObject>(16, width, height, pos, () => new TileObject());
            _gridList = _grid.GetGridList();
        }
    }
}