using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTilemap;
using UnityEngine.UI;

namespace ProjectEditor {
    public class LayerWindow : SlidableUI {
        [SerializeField] private Transform _contentTransform = null;
        [SerializeField] private LayerModel _layerModel = null;
        [SerializeField] private TilemapVisual _tilemapVisualPrefab = null;
        [SerializeField] private TilesetModel _tilesetModel = null;
        [SerializeField] private Button _layerButtonPrefab = null;
        TileLayerWindow _tileLayerWindow;
        static readonly string DefaultTileLayerName = "Tile Layer";
        private string _selectedLayerID;
        public string SelectedLayerIDInWindow { get { return _selectedLayerID;} }
        private Image _selectedButtonImage;
        private int _layerCounter;
        Dictionary<string, Button> _buttons = new Dictionary<string, Button>();

        public override void Initialize() {
            base.Initialize();
            _tileLayerWindow = GetComponentInChildren<TileLayerWindow>(true);
        }

        public void CreateTileLayer() {
            if (_layerModel.IsLayerEmpty()) {
                _selectedLayerID = "0";
            }

            string layerName = DefaultTileLayerName;
            var tilemapLayer = new TileLayer(name, _layerCounter, 16);

            CreateButtonByTileLayer(tilemapLayer);
            _layerModel.AddLayer(tilemapLayer);

            ++_layerCounter;
        }

        private void CreateButtonByTileLayer(TileLayer tilemapLayer) {
            var tilemapVisual = Instantiate(_tilemapVisualPrefab);

            string name = tilemapLayer.TilesetName;
            Material material = (name != null) ? _tilesetModel.GetMaterialByName(name) : _tilesetModel.GetFirstMaterial();
            tilemapVisual.Initialize(tilemapLayer.GetGrid(), material);
            _tilesetModel.AddTilesetVisual(tilemapVisual);

            var button = Instantiate(_layerButtonPrefab, _contentTransform);
            button.GetComponentInChildren<Text>().text = tilemapLayer.LayerName;
            _buttons.Add(tilemapLayer.LayerID, button);

            string id = tilemapLayer.LayerID;
            System.Action callback = () => { OnTileLayerButtonClicked(tilemapLayer); };
            button.onClick.AddListener(() => { 
                _selectedLayerID = id; 
                OnLayerButtonClicked(button, tilemapLayer, callback);
            });
        }

        void OnTileLayerButtonClicked(TileLayer tilemapLayer) {
            _tileLayerWindow.gameObject.SetActive(true);
            _tileLayerWindow.SetDropdownValueWithIgnoreCallback(tilemapLayer);
            _tileLayerWindow.SetInputFieldTextWithIgnoreCallback(tilemapLayer);
        }

        public void CreateColliderLayer() {

        }

        public void CreateDoorLayer() {

        }

        public void CreateBackgroundLayer() {

        }

        private void CreateLayerButton() {

        }

        void OnLayerButtonClicked(Button button, Layer layer, System.Action callback) {
            if (_selectedButtonImage != null) {
                _selectedButtonImage.color = new Color(0.2117647f, 0.2f, 0.2745098f);
                _selectedButtonImage.GetComponentInChildren<Text>().color = Color.white;
            }
            _selectedButtonImage = button.image;

            button.image.color = new Color(1f, 0.7626624f, 0.2122642f);
            _selectedButtonImage.GetComponentInChildren<Text>().color = Color.black;

            callback();
        }

        public Button GetButtonByID(string id) {
            return _buttons[id];
        }
    }
}
