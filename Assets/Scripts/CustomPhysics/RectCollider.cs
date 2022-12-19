#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics {
    [System.Serializable]
    public struct Rectangle {
        public Vector2 position;
        public float width;
        public float height;
        public float rotation;
        public Rectangle(float x, float y, float width, float height, float rotation = 0f) {
            this.position = new Vector2(x, y);
            this.width = width;
            this.height = height;
            this.rotation = rotation;
        }
        public Rectangle(Vector2 position, float width, float height, float rotation = 0f) {
            this.position = position;
            this.width = width;
            this.height = height;
            this.rotation = rotation;
        }

        public Vector2 GetP11() {
            return new Vector2(Mathf.Cos(rotation * Mathf.Deg2Rad), Mathf.Sin(rotation * Mathf.Deg2Rad)) * width;
        }

        public Vector2 GetP01() {
            return new Vector2(Mathf.Cos(rotation * Mathf.Deg2Rad + Mathf.PI), Mathf.Sin(rotation * Mathf.Deg2Rad + Mathf.PI)) * width * 0.5f;
        }
        public Vector2 GetP10() {
            return GetP11() + new Vector2(Mathf.Cos(rotation * Mathf.Deg2Rad + Mathf.PI * 0.5f), Mathf.Sin(rotation * Mathf.Deg2Rad + Mathf.PI * 0.5f)) * height;
        }
        public Vector2 GetP00() {
            return new Vector2(Mathf.Cos(rotation * Mathf.Deg2Rad + Mathf.PI * 0.5f), Mathf.Sin(rotation * Mathf.Deg2Rad + Mathf.PI * 0.5f)) * height;
        }
    }

    public class RectCollider : CustomCollider {
        [SerializeField] Rectangle _rect;
        protected override void Start() {
            base.Start();
        }
        public override Rectangle GetBounds() {
            Rectangle newRectangle = _rect;
            newRectangle.position += (Vector2)transform.position;
            newRectangle.width *= Mathf.Abs(transform.localScale.x);
            newRectangle.height *= Mathf.Abs(transform.localScale.y);
            newRectangle.rotation += transform.localRotation.eulerAngles.z;
            return newRectangle;
        }

        public override bool IsCollision(CustomCollider other) {
            return IsCollision(other);
        }
        public bool IsCollision(RectCollider other) {
            return CollisionManager.GetInstance().IsCollision(this, other);
        }
        public bool IsCollision(CircleCollider other) {
            return CollisionManager.GetInstance().IsCollision(this, other);
        }
        public bool IsCollision(PolygonCollider other) => false;

        public Vector2 GetWidthVector() {
            Vector2 ret;
            ret.x = _rect.width * Mathf.Abs(transform.localScale.x) * Mathf.Cos(_rect.rotation) * 0.5f;
            ret.y = -_rect.width * Mathf.Abs(transform.localScale.x) * Mathf.Sin(_rect.rotation) * 0.5f;
            return ret;
        }
        public Vector2 GetHeightVector() {
            Vector2 ret;
            ret.x = _rect.height * transform.localScale.y * Mathf.Cos(_rect.rotation) * 0.5f;
            ret.y = -_rect.height * transform.localScale.y * Mathf.Sin(_rect.rotation) * 0.5f;
            return ret;
        }

        void OnDrawGizmos() {
            Vector2 pos = (Vector2)transform.position + _rect.position;
            float halfWidth = _rect.width * Mathf.Abs(transform.localScale.x) * 0.5f;
            float height = _rect.height * Mathf.Abs(transform.localScale.y);
            
            float rotation = _rect.rotation + transform.localRotation.eulerAngles.z;
            float radian = rotation * Mathf.Deg2Rad;
            
            Vector2 vec = new Vector2(Mathf.Cos(radian + Mathf.PI * 0.5f), Mathf.Sin(radian + Mathf.PI * 0.5f)) * height;
            Vector2 p11 = pos + new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * halfWidth;
            Vector2 p01 = pos + new Vector2(Mathf.Cos(radian + Mathf.PI), Mathf.Sin(radian + Mathf.PI)) * halfWidth;
            Vector2 p00 = p01 + vec;
            Vector2 p10 = p11 + vec;

            Gizmos.color = _gizmoColor;
            Gizmos.DrawLine(p00, p10);
            Gizmos.DrawLine(p10, p11);
            Gizmos.DrawLine(p11, p01);
            Gizmos.DrawLine(p01, p00);
        }
    }
}