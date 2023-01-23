using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectEditor {
    public class EditorMain : MonoBehaviour {
        [SerializeField] private Transform _editorWindowParent = null;
        private SlidableUI[] _editorWindows = null;
        public static int CurrentGridWidth;
        public static int CurrentGridHeight;
        public static Vector3 CurrentOriginPosition;

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

        public void OnTilesetChanged() {

        }

        void Update() {
            
        }
    }
}