using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectEditor {
    public delegate void OnPicked(int index);
    public class TilesetPickerWindow : MonoBehaviour {
        [SerializeField] Image _tilemapImage = null;
        [SerializeField] Transform _selectOutline = null;
        static readonly float ExpandedWidth = 800f;
        static readonly float DefaultWidth = 334f;
        RectTransform _transform;
        bool _isPointerEnter;
        Vector3 _prevMouseScreenPos;
        ScrollRect _scrollRect;
        Vector2 _gridOriginPos, _gridEndPos;
        OnPicked _onTilemapPicked;

        void Awake() {
            _transform = GetComponent<RectTransform>();
            _scrollRect = GetComponent<ScrollRect>();
        }

        void Start() {
            CalculateGridPos();
        }

        public void CalculateGridPos() {
            int height = _tilemapImage.mainTexture.height * 3;
            int width = _tilemapImage.mainTexture.width * 3;

            Vector2 vec = Vector2.down * height;
            _tilemapImage.rectTransform.anchoredPosition += vec;

            Vector3 pos = _tilemapImage.transform.position;
            _gridOriginPos = pos;
            _tilemapImage.rectTransform.anchoredPosition -= vec;

            vec = Vector2.right * width;
            _tilemapImage.rectTransform.anchoredPosition += vec;
            pos = _tilemapImage.transform.position;

            _gridEndPos = pos;
            _tilemapImage.rectTransform.anchoredPosition -= vec;
        }

        void Update() {
            if (!_isPointerEnter) return;
            CalculateGridPos();

            if (Input.GetKeyDown(KeyCode.Mouse2)) {
                _prevMouseScreenPos = Input.mousePosition;
            }
            if (Input.GetKey(KeyCode.Mouse2)) {
                Vector3 cur = Input.mousePosition;
                Vector3 delta = cur - _prevMouseScreenPos;
                
                _tilemapImage.rectTransform.localPosition += delta;

                _prevMouseScreenPos = cur;
            }
        }

        public void OnPointerClick() {
            if (Input.GetMouseButton(2) || Input.GetMouseButtonUp(2)) return;

            Vector2 mousePoint = EditorUtils.GetMouseWorldPosition();
            int r, c;
            Vector3 newPos = GetPositionOnTilemap(mousePoint, out r, out c);
            newPos.z = 0f;

            int cCounts = _tilemapImage.mainTexture.width / 16;

            _selectOutline.position = newPos;
            _onTilemapPicked?.Invoke(r * cCounts + c);
        }

        public int ResetSelectedTile() {
            Vector2 initalPos = _tilemapImage.transform.position;
            initalPos.x += 1f; initalPos.y -= 1f;
            int r, c;

            Vector3 newPos = GetPositionOnTilemap(initalPos, out r, out c);
            newPos.z = 0f;
            _selectOutline.position = newPos;

            int cCounts = _tilemapImage.mainTexture.width / 16;
            return r * cCounts + c;
        }

        Vector3 GetPositionOnTilemap(Vector2 pos, out int row, out int column) {
            Vector2 delta = pos - _gridOriginPos;
            Vector2 delta2 = _gridEndPos - _gridOriginPos;
            
            int rCounts = _tilemapImage.mainTexture.height / 16;
            int cCounts = _tilemapImage.mainTexture.width / 16;

            float rPadding = delta2.y / rCounts;
            float cPadding = delta2.x / cCounts;    
            
            row = Mathf.FloorToInt(delta.y / rPadding);
            column = Mathf.FloorToInt(delta.x / cPadding);

            Vector3 newPos = _gridOriginPos;
            newPos.x += column * cPadding;
            newPos.y += row * rPadding;

            row = rCounts - row - 1;
            return newPos;
        }

        public void OnPointerEnter() {
            _isPointerEnter = true;
            _transform.sizeDelta = new Vector2(ExpandedWidth, _transform.sizeDelta.y);
        }

        public void OnPointerExit() {
            _isPointerEnter = false;
            _transform.sizeDelta = new Vector2(DefaultWidth, _transform.sizeDelta.y);
        }

        public void AddOnTilemapPicked(OnPicked e) {
            _onTilemapPicked += e;
        }
    }
}
