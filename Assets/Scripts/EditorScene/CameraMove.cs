using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectEditor {
    public class CameraMove : MonoBehaviour {
        [SerializeField] private float moveSpeed = 20f, scrollSpeed = 20f;
        [SerializeField] float maxZoom = 250f, minZoom = 45f;
        private Vector3 _prevMouseScreenPos;
        bool _isNotOnUI;

        private void Update() {
            Vector3 pos = transform.position;
            
            _isNotOnUI = !EventSystem.current.IsPointerOverGameObject();
            if (Input.GetKeyDown(KeyCode.Mouse2)) {
                _prevMouseScreenPos = Input.mousePosition;
            }
            if (_isNotOnUI && Input.GetKey(KeyCode.Mouse2)) {
                Vector3 cur = Input.mousePosition;
                Vector3 delta = _prevMouseScreenPos - cur;
                
                pos += delta.normalized * moveSpeed;

                _prevMouseScreenPos = cur;
            }

            Camera cam = Camera.main;
            float height = 2f * cam.orthographicSize;
            float width = height * cam.aspect;
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (_isNotOnUI) {
                cam.orthographicSize += -scroll * scrollSpeed * Time.deltaTime * 10f;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
            }
            pos.z = -10;
            transform.position = pos;
        }
    }
}