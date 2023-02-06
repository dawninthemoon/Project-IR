using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aroma {
    public class LineUtility : Singleton<LineUtility> {
        ObjectPool<LineRenderer> _lineRendererPool;
        LineRenderer[] _rectRenderer;
        Material _gpuInstancing;
        public LineUtility() {
            _gpuInstancing = Resources.Load<Material>("Others/GPUInstancing");
            int gridSize = PlayerPrefs.GetInt(GridUtils.DefaultGridSizeKey);
            _lineRendererPool = new ObjectPool<LineRenderer>(gridSize, CreateLineRenderer, null, null);
            _rectRenderer = new LineRenderer[4] {
                CreateLineRenderer(),
                CreateLineRenderer(),
                CreateLineRenderer(),
                CreateLineRenderer(),
            };
        }

        public Material GetMaterial() => _gpuInstancing;

        public void ClearAllLines() {
            _lineRendererPool.Clear();
        }

        public void DisableRect() {
            foreach (var lr in _rectRenderer) {
                lr.gameObject.SetActive(false);
            }
        }

        public void DrawRect(Vector2 p00, Vector2 p10, Vector2 p11, Vector2 p01, Color color, float width) {
            foreach (var lr in _rectRenderer) {
                lr.gameObject.SetActive(true);
            }

            DrawLine(_rectRenderer[0], p00, p01, color, width);
            DrawLine(_rectRenderer[1], p01, p11, color, width);
            DrawLine(_rectRenderer[2], p11, p10, color, width);
            DrawLine(_rectRenderer[3], p10, p00, color, width);
        }

        LineRenderer CreateLineRenderer() {
            GameObject obj = new GameObject();
            var lr = obj.AddComponent<LineRenderer>();
            lr.startWidth = lr.endWidth = 0.5f;
            lr.material = _gpuInstancing;
            return lr;
        }

        public void DrawLine(Vector3 start, Vector3 end, Color color, float width = 0.5f) {
            var lr = _lineRendererPool.GetObject();
            DrawLine(lr, start, end, color, width);
        }

        public void DrawLine(LineRenderer lr, Vector3 start, Vector3 end, Color color, float width) {
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            lr.startColor = lr.endColor = color;
            lr.receiveShadows = false;
            lr.startWidth = lr.endWidth = width;
        }
    }
}