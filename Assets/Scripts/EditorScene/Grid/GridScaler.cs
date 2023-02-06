using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

namespace ProjectEditor {
    public class GridScaler : MonoBehaviour {
        public static bool ScalerDraging;
        bool _draging;
        public Vector2 StartPosition { get; private set; }
        public delegate void OnScalerChanged(GridScaler scaler);
        public delegate void OnScalerChanging(GridScaler scaler);
        OnScalerChanging _onScalerChangingCallback;
        OnScalerChanged _onScalerChangedCallback;
        public int ScalerIndex { get; set; }

        private void Update() {
            if (_draging) {
                _onScalerChangingCallback(this);
            }
        }

        void OnMouseDown() {
            ScalerDraging = true;
            _draging = true;
            StartPosition = transform.position;
        }

        void OnMouseUp() {
            ScalerDraging = false;
            _draging = false;
            _onScalerChangedCallback(this);
        }

        public void SetCallbackFunc(OnScalerChanging callback1, OnScalerChanged callback2) {
            _onScalerChangingCallback = callback1;
            _onScalerChangedCallback = callback2;
        }

        public void GetWHDelta(out int wDelta, out int hDelta, GridScaler scaler) {
            Vector2 startPos = StartPosition;
            Vector2 endPos = EditorUtils.GetMouseWorldPosition();
            Vector2 delta = endPos - StartPosition;

            wDelta = Mathf.FloorToInt(delta.x / 16f);
            hDelta = Mathf.FloorToInt(delta.y / 16f);
        }
    }
}