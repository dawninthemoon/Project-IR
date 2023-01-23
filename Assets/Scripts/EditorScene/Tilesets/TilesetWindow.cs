using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectEditor {
    public class TilesetWindow : SlidableUI {
        [SerializeField] private Image _tilesetPreview = null;

        public override void Initialize() {
            base.Initialize();
            
        }
    }
}