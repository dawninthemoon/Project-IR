using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTilemap;

namespace ProjectEditor {
    public class TileLayer : Layer {
        public int DropdownValue { get; set; }
        public string TilesetName { get; set; }
        private CustomGrid<TileObject> _grid;
        private TilemapVisual _tilemapVisual;
        public TilemapVisual  Visual { 
            get { return _tilemapVisual; }
            set { _tilemapVisual = value; }
        }
        private List<List<TileObject>> _gridList;

        public TileLayer() {
            int width = EditorMain.CurrentGridWidth;
            int height = EditorMain.CurrentGridHeight;
            Vector3 origin = EditorMain.CurrentOriginPosition;
            _grid = new CustomGrid<TileObject>(16, width, height, origin, () => new TileObject());
            _gridList = _grid.GetGridList();
        }

        public override void SetTileIndex(Vector3 worldPosition, int tileIndex) {
            TileObject tilemapObject = _grid.GetGridObject(worldPosition, EditorMain.CurrentOriginPosition);
            tilemapObject?.SetIndex(tileIndex);
        }

        public TileLayer(string layerName, int layerIndex, float cellSize)
        : base(layerName, layerIndex) {
            int width = EditorMain.CurrentGridWidth;
            int height = EditorMain.CurrentGridHeight;
            Vector3 origin = EditorMain.CurrentOriginPosition;
            TilesetName = TilesetModel.DefaultTilesetName;

            _grid = new CustomGrid<TileObject>(cellSize, width, height, origin, () => new TileObject());
            _gridList = _grid.GetGridList();
        }

        public override void ResizeGrid(Vector3 originPosition, int widthDelta, int heightDelta) {
            int width = EditorMain.CurrentGridWidth;
            int height = EditorMain.CurrentGridHeight;
            Vector3 origin = EditorMain.CurrentOriginPosition;

            _grid.ResizeGrid(origin, width, height, originPosition, widthDelta, heightDelta);
            _gridList = _grid.GetGridList();
        }

        public CustomGrid<TileObject> GetGrid() {
            return _grid;
        }

        public override void Progress() {
            _tilemapVisual.UpdateHeatMapVisual();
        }
    }
}