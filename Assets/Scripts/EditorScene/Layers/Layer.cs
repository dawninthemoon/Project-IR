using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectEditor {
    public abstract class Layer {
        public string LayerName { get; set; }
        public string LayerID { get; private set; }

        public Layer() { }
        public Layer(string layerName, int layerIndex) {
            LayerName = layerName;
            LayerID = layerIndex.ToString();
        }
        public abstract void SetTileIndex(Vector3 worldPosition, int textureIndex);
        public abstract void ResizeGrid(Vector3 originPosition, int widthDelta, int heightDelta);
        public abstract void Progress();
    }
}