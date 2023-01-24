using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectEditor {
    public class SlidableUI : MonoBehaviour, ISetupable {
        [SerializeField] private float _xPositionWhenDisable = 0f;
        [SerializeField] private float _xPositionWhenActive = 0f;
        [SerializeField] private float _slideDuration = 0.5f;

        [HideInInspector] public new RectTransform transform;
        private Coroutine _slideCoroutine;

        public virtual void Initialize() {
            transform = GetComponent<RectTransform>();

            Vector2 initialPosition = transform.anchoredPosition;
            initialPosition.x = _xPositionWhenDisable;
            transform.anchoredPosition = initialPosition;
            gameObject.SetActive(false);
        }

        public void Toggle() {
            if (!gameObject.activeSelf) {
                SlideOut();    
            }
            else {
                SlideIn();
            }
        }

        public void SlideIn() {
            if (!gameObject.activeSelf) return;

            if (_slideCoroutine != null) {
                StopCoroutine(_slideCoroutine);
            }
            _slideCoroutine = StartCoroutine(ExecuteSlide(transform, false));
        }

        public void SlideOut() {
            gameObject.SetActive(true);

            if (_slideCoroutine != null) {
                StopCoroutine(_slideCoroutine);
            }
            _slideCoroutine = StartCoroutine(ExecuteSlide(transform, true));
        }

        IEnumerator ExecuteSlide(RectTransform target, bool isSlideOut) {
            float timeAgo = 0f;
            while (timeAgo - 1f < Mathf.Epsilon) {
                timeAgo += Time.deltaTime / _slideDuration;

                float startX = isSlideOut ? _xPositionWhenDisable : _xPositionWhenActive;
                float endX = isSlideOut ? _xPositionWhenActive : _xPositionWhenDisable;
                float x = Mathf.Lerp(startX, endX, timeAgo);

                Vector2 nextPosition = target.anchoredPosition;
                nextPosition.x = x;
                target.anchoredPosition = nextPosition;

                yield return null;
            }

            if (!isSlideOut)
                gameObject.SetActive(false);
            _slideCoroutine = null;
        }
    }
}