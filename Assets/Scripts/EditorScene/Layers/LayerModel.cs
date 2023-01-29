using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectEditor {
    public class LayerModel : MonoBehaviour {
        [SerializeField] LayerWindow _layerWindow = null;
        public Dictionary<string, Layer> CurrentLayerDictionary { get; private set;}
        public string SelectedLayerID { get; set; }

        private void Awake() {
            LoadLayers();
        }

        public void LoadLayers() {
            int defaultGridSize = 16;
            EditorMain.CurrentGridWidth = defaultGridSize;
            EditorMain.CurrentGridHeight = defaultGridSize;
            EditorMain.CurrentOriginPosition = Vector3.zero;

            CurrentLayerDictionary = new Dictionary<string, Layer>();
        }

        public void AddLayer(Layer layer) {
            CurrentLayerDictionary.Add(layer.LayerID, layer);
            //_layerWindow.
        }

        public bool IsLayerEmpty() => (CurrentLayerDictionary.Count == 0);
        public int NumOfLayers() => CurrentLayerDictionary.Count;
    }
}