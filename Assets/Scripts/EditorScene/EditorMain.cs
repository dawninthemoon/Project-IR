using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectEditor {
    public class EditorMain : MonoBehaviour {
        [SerializeField] private Transform _editorWindowParent = null;
        [SerializeField] private TilesetModel _tilesetModel = null;
        [SerializeField] private TilesetPickerWindow _tilesetPickerWindow = null;
        [SerializeField] private EditorView _editorView = null;
        [SerializeField] private LayerPicker _layerPicker = null;
        private SlidableUI[] _editorWindows = null;
        public static int CurrentGridWidth;
        public static int CurrentGridHeight;
        public static Vector3 CurrentOriginPosition;
        private int _selectedTileIndex = 0;

        private void Awake() {
            _layerPicker.SetOnTilesetChanged(OnTilesetChanged);
        }

        void Start() {
            int numOfSlidableUI = _editorWindowParent.childCount;
            _editorWindows = new SlidableUI[numOfSlidableUI];
            for (int i = 0; i < numOfSlidableUI; ++i) {
                _editorWindows[i] = _editorWindowParent.GetChild(i).GetComponent<SlidableUI>();
                _editorWindows[i].Initialize();
            }
        }

        public void DisableAllProjectWindows() {
            foreach (var window in _editorWindows) {
                window.SlideIn();
            }
        }

        public void OnTilesetChanged(TileLayer tileLayer) {
            var spr = _tilesetModel.GetTilesetSprite(tileLayer.TilesetName);
            _editorView.TilesetPreivewImage.sprite = spr;
            _editorView.TilesetPickerImage.sprite = spr;
            _editorView.TilesetPickerImage.rectTransform.sizeDelta = new Vector2(spr.texture.width, spr.texture.height);
            _tilesetPickerWindow.CalculateGridPos();
            _selectedTileIndex = _tilesetPickerWindow.ResetSelectedTile();
        }
    }
}