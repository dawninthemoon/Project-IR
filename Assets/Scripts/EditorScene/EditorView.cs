using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectEditor {
    public class EditorView : MonoBehaviour {
        [SerializeField] Image _tilesetPreviewImage = null;
        public Image TilesetPreivewImage { get { return _tilesetPreviewImage; } }

        [SerializeField] Image _tilesetPickerImage = null;
        public Image TilesetPickerImage { get { return _tilesetPickerImage; } }

        [SerializeField] Dropdown _layerTilesetNameDropdown = null;
        public Dropdown LayerTilesetNameDropdown { get { return _layerTilesetNameDropdown; } }

        [SerializeField] InputField _layerNameInputField = null;
        public InputField LayerNameInputField { get { return _layerNameInputField; } }

        [SerializeField] Transform _tileLayerInfo = null;
        public Transform TileLayerInfo { get { return _tileLayerInfo; } }
    }
}