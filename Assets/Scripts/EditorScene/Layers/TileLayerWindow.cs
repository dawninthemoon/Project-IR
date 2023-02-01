using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectEditor {
    public class TileLayerWindow : MonoBehaviour {
        [SerializeField] LayerModel _layerModel = null;
        [SerializeField] EditorView _editorView = null;
        LayerWindow _projectLayerWindow;
        bool _ignoreCallback;
        OnTilesetChanged _onTilesetChanged;

        public void SetOnTilesetChanged(OnTilesetChanged callback) {
            _onTilesetChanged = callback;
        }
        void Start() {
            _projectLayerWindow = GetComponentInParent<LayerWindow>();
        }

        public void SetDropdownValueWithIgnoreCallback(Layer layer) {
            _ignoreCallback = true;
            TileLayer tileLayer = layer as TileLayer;
            _editorView.LayerTilesetNameDropdown.value = tileLayer.DropdownValue;
            _ignoreCallback = false;
        }

        public void SetInputFieldTextWithIgnoreCallback(Layer layer) {
            _ignoreCallback = true;
            _editorView.LayerNameInputField.text = layer.LayerName;
            _ignoreCallback = false;
        }

        public void OnTilesetDropdownChanged(TMP_Dropdown dropdown) {
            if (_ignoreCallback) return;
            string tilesetName = dropdown.options[dropdown.value].text;
            var layer = _layerModel.GetLayerByIndex(_projectLayerWindow.SelectedLayerIDInWindow) as TileLayer;
            layer.DropdownValue = dropdown.value;
            layer.TilesetName = tilesetName;

            if (_layerModel.SelectedLayerID.Equals(_projectLayerWindow.SelectedLayerIDInWindow)) {
                _onTilesetChanged(layer);
            }
        }

        public void OnLayerNameInputFieldChanged(InputField inputField) {
            if (_ignoreCallback) return;

            var layer = _layerModel.GetLayerByIndex(_projectLayerWindow.SelectedLayerIDInWindow);
            layer.LayerName = inputField.text;
            Button button = _projectLayerWindow.GetButtonByID(layer.LayerID);
            button.GetComponentInChildren<Text>().text = layer.LayerName;
        }
    }
}