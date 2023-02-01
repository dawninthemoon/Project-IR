using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectEditor {
    public class LayerModel : MonoBehaviour {
        [SerializeField] LayerPicker _layerPicker = null;
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
            _layerPicker.AddLayerButton(this, layer);
        }

        public void DeleteLayerByID(string layerID) {
            CurrentLayerDictionary.Remove(layerID);
            _layerPicker.DeleteLayerButton(layerID);
        }

        public void SetTile(Vector3 worldPosition, int tileIndex) {
            if (CurrentLayerDictionary.Count == 0) return;
            CurrentLayerDictionary[SelectedLayerID].SetTileIndex(worldPosition, tileIndex);
        }

        public Layer GetSelectedLayer() {
            return GetLayerByIndex(SelectedLayerID);
        }

        public Layer GetLayerByIndex(string index) {
            foreach (Layer layer in CurrentLayerDictionary.Values) {
                if (layer.LayerID == index) {
                    return layer;
                }
            }
            return null;
        }

        public bool IsLayerEmpty() => (CurrentLayerDictionary.Count == 0);
        public int NumOfLayers() => CurrentLayerDictionary.Count;
    }
}
