using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTilemap;

namespace ProjectEditor {
    public delegate void OnTilesetChanged(TileLayer tileLayer);
    public class TilesetModel : MonoBehaviour {
        private List<TilemapVisual> _currentTilemapVisuals;
        public Dictionary<string, Material> CurrentTilesetMaterials { get; private set; }
        private Dictionary<string, Sprite> _currentTilesetSprites;
        private int _selectedTilesetIndex;
        public static string DefaultTilesetName;

        private void Awake() {
            _currentTilemapVisuals = new List<TilemapVisual>();
            _currentTilesetSprites = new Dictionary<string, Sprite>();
            CurrentTilesetMaterials = new Dictionary<string, Material>();

            var tilesetMaterials = Resources.LoadAll<Material>("Tileset/");
            var tilesetSprites = Resources.LoadAll<Sprite>("Tileset/");

            if (tilesetMaterials == null || tilesetMaterials.Length == 0) 
                return;

            DefaultTilesetName = tilesetSprites[0].name;
            SetTilesets(tilesetMaterials, tilesetSprites);
        }

        public void ChangeTileset(string tilesetName, TilemapVisual visual) {
            Material material = CurrentTilesetMaterials[tilesetName];
            visual.Initialize(null, material);
        }

        public Material GetMaterialByName(string name) {
            return CurrentTilesetMaterials[name];
        }

        public Material GetFirstMaterial() {
            foreach (var material in CurrentTilesetMaterials.Values) {
                return material;
            }
            return null;
        }

        public void SetTilesets(Material[] materials, Sprite[] sprites) {
            if (materials.Length != sprites.Length)
                Debug.LogError("Material-Sprite pair error");
            for (int i = 0; i < materials.Length; ++i) {
                string key = sprites[i].name;
                _currentTilesetSprites.Add(key, sprites[i]);
                CurrentTilesetMaterials.Add(key, materials[i]);
            }
        }

        public void SetTilesetIndex(int index) {
            _selectedTilesetIndex = index;
        }

        public void AddTilesetVisual(TilemapVisual visual) {
            _currentTilemapVisuals.Add(visual);
        }

        public Sprite GetTilesetSprite(string tilesetName) {
            return _currentTilesetSprites[tilesetName];
        }
    }
}
