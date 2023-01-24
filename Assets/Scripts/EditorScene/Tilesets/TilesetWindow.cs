using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectEditor {
    public class TilesetWindow : SlidableUI {
        [SerializeField] private Transform _scrollViewTransform = null;
        [SerializeField] private Button _tilesetButtonPrefab = null;
        [SerializeField] private TilesetModel _tilesetModel = null;
        [SerializeField] private EditorView _editorView = null;

        public override void Initialize() {
            base.Initialize();
            _editorView.TilesetPreivewImage.color = Color.white;

            var tilesetMaterials = _tilesetModel.CurrentTilesetMaterials;
            int index = 0;
            foreach (var pair in _tilesetModel.CurrentTilesetMaterials) {
                var button = Instantiate(_tilesetButtonPrefab, _scrollViewTransform);
                button.GetComponentInChildren<Text>().text = pair.Value.name;
                
                int temp = index;
                button.onClick.AddListener(() => {
                    _tilesetModel.SetTilesetIndex(temp);
                });
                ++index;

                //Dropdown.OptionData optionData = new Dropdown.OptionData(pair.Key);
                //_editorView.LayerTilesetNameDropdown.options.Add(optionData);
            }

            _tilesetModel.SetTilesetIndex(0);
            gameObject.SetActive(false);
        }
    }
}