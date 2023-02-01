using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectEditor {
    public class EditorView : MonoBehaviour {
        [SerializeField] Image _tilesetPreviewImage = null;
        public Image TilesetPreivewImage { get { return _tilesetPreviewImage; } }

        [SerializeField] Image _tilesetPickerImage = null;
        public Image TilesetPickerImage { get { return _tilesetPickerImage; } }

        [SerializeField] TMP_Dropdown _layerTilesetNameDropdown = null;
        public TMP_Dropdown LayerTilesetNameDropdown { get { return _layerTilesetNameDropdown; } }

        [SerializeField] TMP_InputField _layerNameInputField = null;
        public TMP_InputField LayerNameInputField { get { return _layerNameInputField; } }

        [SerializeField] Transform _tileLayerInfo = null;
        public Transform TileLayerInfo { get { return _tileLayerInfo; } }
    }
}