using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

namespace ProjectEditor {
    public delegate void OnGridResized();
    public class GridController : MonoBehaviour {
        [SerializeField] GridScaler _gridScalerPrefab = null;
        [SerializeField] float _rectWidth = 1f;
        [SerializeField] LayerModel _layerModel = null;
        float _cellSize = 16;
        Vector3[] _vertexes = new Vector3[8];
        Vector3[] _gridScalerPos = new Vector3[8];
        Vector3[] _units;
        static readonly Vector2 DefaultColliderSize = new Vector2(16f, 16f);
        GridScaler[] _gridScalers;

        void Awake() {
            _gridScalers = new GridScaler[8];
            for (int i = 0; i < _gridScalers.Length; ++i) {
                _gridScalers[i] = Instantiate(_gridScalerPrefab, _vertexes[i], Quaternion.identity);
                _gridScalers[i].SetCallbackFunc(OnScalerChanging, OnScalerChanged);
            }

            _gridScalerPos[0] = new Vector2(-1f, 1f) * _cellSize;
            _gridScalerPos[1] = Vector2.one * _cellSize;
            _gridScalerPos[2] = new Vector3(1f, -1f) * _cellSize;
            _gridScalerPos[3] = -Vector2.one * _cellSize;
            _gridScalerPos[4] = Vector2.up * _cellSize;
            _gridScalerPos[5] = Vector2.right * _cellSize;
            _gridScalerPos[6] = Vector2.down * _cellSize;
            _gridScalerPos[7] = Vector2.left * _cellSize;

            _layerModel.SetOnGridResized(UpdateGridLine);
        }

        void Start() {
            UpdateGridScalers();
            DrawGridLine();
        }

        void UpdateGridScalers() {
            int width = EditorMain.CurrentGridWidth;
            int height = EditorMain.CurrentGridHeight;

            _vertexes[0] = GridUtils.GetWorldPosition(0, height, _cellSize, EditorMain.CurrentOriginPosition);
            _vertexes[1] = GridUtils.GetWorldPosition(width, height, _cellSize, EditorMain.CurrentOriginPosition);
            _vertexes[2] = GridUtils.GetWorldPosition(width, 0, _cellSize, EditorMain.CurrentOriginPosition);
            _vertexes[3] = GridUtils.GetWorldPosition(0, 0, _cellSize, EditorMain.CurrentOriginPosition);

            for (int i = 0; i < _vertexes.Length / 2; ++i) {
                _vertexes[i + 4] = (_vertexes[(i + 1) % 4] - _vertexes[i]) * 0.5f + _vertexes[i];
            }
            
            for (int i = 0; i < _vertexes.Length; ++i) {
                _gridScalers[i].transform.position = _vertexes[i] + _gridScalerPos[i];
                _gridScalers[i].ScalerIndex = i;
            }
        }

        void OnScalerChanging(GridScaler scaler) {
            if (_layerModel.IsLayerEmpty()) return;
            if (!CanResizeGrid(scaler.ScalerIndex)) {
                Aroma.LineUtility.GetInstance().DisableRect();
                return;
            }

            Vector2 p00, p10, p11, p01;
            GetRectPoints(scaler.ScalerIndex, out p00, out p10, out p11, out p01);

            Aroma.LineUtility.GetInstance().DrawRect(p00, p10, p11, p01, Color.yellow, _rectWidth);
        }

        void OnScalerChanged(GridScaler scaler) {
            if (_layerModel.IsLayerEmpty()) return;
            Aroma.LineUtility.GetInstance().DisableRect();
            if (!CanResizeGrid(scaler.ScalerIndex)) return;
            
            Vector2 p00, p10, p11, p01;
            GetRectPoints(scaler.ScalerIndex, out p00, out p10, out p11, out p01);
            
            EditorUtils.SortRectanglePoints(ref p00, ref p10, ref p11, ref p01);

            int curWidth = Mathf.FloorToInt(Mathf.Abs((p10 - p01).x) / 16f);
            int curHeight = Mathf.FloorToInt(Mathf.Abs((p10 - p01).y) / 16f);

            _layerModel.ResizeAllLayers(p01, curWidth - EditorMain.CurrentGridWidth, curHeight - EditorMain.CurrentGridHeight);
        }

        bool CanResizeGrid(int index) {
            Vector2 cur = EditorUtils.GetMouseWorldPosition();
            int width = EditorMain.CurrentGridWidth, height = EditorMain.CurrentGridHeight;

            int x, y;
            GridUtils.GetXY(cur, out x, out y, _cellSize, EditorMain.CurrentOriginPosition);

            bool[] canResizeGridArr = new bool[8] {
                (x < width - 1) && (y > 0),
                (x > 0) && (y > 0),
                (x > 0) && (y < height - 1),
                (x < width - 1) && (y < height - 1),
                y > 0,
                x > 0,
                y < height - 1,
                x < width - 1
            };

            return canResizeGridArr[index];
        }

        void GetRectPoints(int index, out Vector2 p00, out Vector2 p10, out Vector2 p11, out Vector2 p01) {
            Vector3 cur = EditorUtils.GetMouseWorldPosition() - _gridScalerPos[index];
            if (index >= 4) {
                if (index % 2 == 0) cur.x = _vertexes[index].x;
                else cur.y = _vertexes[index].y;
            }
            int x, y;
            GridUtils.GetXY(cur, out x, out y, _cellSize, EditorMain.CurrentOriginPosition);
            Vector2 cursorPos = GridUtils.GetWorldPosition(x, y, _cellSize, EditorMain.CurrentOriginPosition);

            if (index < 4) {
                int diagonalIndex = (index + 2) % 4;
                p00 = _vertexes[diagonalIndex];
                p10 = new Vector2(cursorPos.x, p00.y);
                p11 = new Vector2(cursorPos.x, cursorPos.y);
                p01 = new Vector2(p00.x, cursorPos.y);
            }
            else {
                int parallelIndex = (index + 2) % 4 + 4;
                Vector2 p = _vertexes[parallelIndex];
                float halfWidth = 
                    (GridUtils.GetWorldPosition(EditorMain.CurrentGridWidth, 0, _cellSize, EditorMain.CurrentOriginPosition).x - 
                    GridUtils.GetWorldPosition(0, 0, _cellSize, EditorMain.CurrentOriginPosition).x) * 0.5f;
                float halfHeight = 
                    (GridUtils.GetWorldPosition(0, EditorMain.CurrentGridHeight, _cellSize, EditorMain.CurrentOriginPosition).y -
                    GridUtils.GetWorldPosition(0, 0, _cellSize, EditorMain.CurrentOriginPosition).y) * 0.5f;

                if (index % 2 == 0) {
                    p00 = new Vector2(p.x - halfWidth, p.y);
                    p10 = new Vector2(p.x + halfWidth, p.y);
                    p11 = new Vector2(p.x + halfWidth, cursorPos.y);
                    p01 = new Vector2(p.x - halfWidth, cursorPos.y);
                }
                else {
                    p00 = new Vector2(p.x, p.y + halfHeight);
                    p10 = new Vector2(cursorPos.x, p.y + halfHeight);
                    p11 = new Vector2(cursorPos.x, p.y - halfHeight);
                    p01 = new Vector2(p.x, p.y - halfHeight);
                }
            }
        }

        public void UpdateGridLine() {
            Aroma.LineUtility.GetInstance().ClearAllLines();
            UpdateGridScalers();
            DrawGridLine();
        }

        public void DrawGridLine() {
            var lineUtility = Aroma.LineUtility.GetInstance();
            int width = EditorMain.CurrentGridWidth;
            int height = EditorMain.CurrentGridHeight;
            
            for (int i = 0; i <= height; ++i) {
                Vector2 p1 = GridUtils.GetWorldPosition(0, i, _cellSize, EditorMain.CurrentOriginPosition);
                Vector2 p2 = GridUtils.GetWorldPosition(width, i, _cellSize, EditorMain.CurrentOriginPosition);
                lineUtility.DrawLine(p1, p2, Color.white);
            }
            for (int i = 0; i <= width; ++i) {
                Vector2 p1 = GridUtils.GetWorldPosition(i, 0, _cellSize, EditorMain.CurrentOriginPosition);
                Vector2 p2 = GridUtils.GetWorldPosition(i, height, _cellSize, EditorMain.CurrentOriginPosition);
                lineUtility.DrawLine(p1, p2, Color.white);
            }
        }
    }
}