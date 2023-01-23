using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectEditor {
    public class LayerWindow : SlidableUI {
        [SerializeField] private LayerModel _layerModel = null;
        static readonly string DefaultTileLayerName = "Tile Layer";
        private string _selectedLayerID;
        private int _layerCounter;

        public override void Initialize() {
            base.Initialize();
        }

        public void CreateTileLayer() {
            if (_layerModel.IsLayerEmpty()) {
                _selectedLayerID = "0";
            }

            string layerName = DefaultTileLayerName;
            
        }

        public void CreateColliderLayer() {

        }

        public void CreateDoorLayer() {

        }

        public void CreateBackgroundLayer() {

        }
    }
}
