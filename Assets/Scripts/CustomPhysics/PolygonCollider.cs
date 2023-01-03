using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics {
    [System.Serializable]
    public class Polygon {
        public Vector2[] points;
        public Vector2 offset;
        Rectangle _bounds;
        public Rectangle GetBounds() => _bounds;
        public void CalculateMinMaxBounds() {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (Vector2 point in points) {
                if (point.x < minX) { minX = point.x; }
                if (point.y < minY) { minY = point.y; }
                if (point.x > maxX) { maxX = point.x; }
                if (point.y > maxY) { maxY = point.y; }
            }

            float width = maxX - minX;
            float height = maxY - minY;

            _bounds = new Rectangle(minX + width * 0.5f, minY, width, height);
        }
    }

    public class PolygonCollider : CustomCollider {
        [SerializeField] Polygon _polygon = null;
        public void Initalize(Vector2[] points) {
            if (_polygon == null)
                _polygon = new Polygon();

            _polygon.points = points.Clone() as Vector2[];
            _polygon.offset = Vector2.zero;
            _polygon.CalculateMinMaxBounds();

            gameObject.SetActive(true);
        }
        public Polygon GetPolygon() => _polygon;
        public override bool IsCollision(CustomCollider other) {
            return false;
            //return IsCollision(other);
        }
        public bool IsCollision(PolygonCollider other) {
            return CollisionManager.GetInstance().IsCollision(_polygon, transform.position, other.GetPolygon(), other.transform.position);
        }

        public bool IsCollision(CircleCollider other) => false;
        public bool IsCollision(RectCollider other) => false;
        public override Rectangle GetBounds() {
            return _polygon.GetBounds();
        }

        void OnDrawGizmos() {
            if (_polygon == null) return;
            var points = _polygon.points;
            if (points == null) return;
            int pLength = points.Length;
            if (pLength < 2) return;

            Gizmos.color = _gizmoColor;
            Vector2 cur = transform.position;
            for (int i = 0; i < pLength; ++i) {
                int nextIdx = (i + 1) % pLength;
                Gizmos.DrawLine(points[i] + _polygon.offset + cur, points[nextIdx] + _polygon.offset + cur);
            }
        }
    }
}