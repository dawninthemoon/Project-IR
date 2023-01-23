using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectEditor {
    public class LayerModel : MonoBehaviour {
        [SerializeField] LayerWindow _layerWindow = null;
        public Dictionary<string, Layer> CurrentLayerDictionary { get; private set;}
        public string SelectedLayerID;

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

        public bool IsLayerEmpty() => (CurrentLayerDictionary.Count == 0);
        public int NumOfLayers() => CurrentLayerDictionary.Count;
    }
}
