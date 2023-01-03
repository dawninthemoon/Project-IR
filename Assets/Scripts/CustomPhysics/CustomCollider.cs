using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CustomPhysics {
    public enum ColliderLayerMask {
        Default,
        Door,
        Ground,
        DetectPlayer,
        PlayerHitbox,
        EnemyHitbox,
        PlayerAttack,
        EnemyAttack,
    }
    public abstract class CustomCollider : MonoBehaviour, IQuadTreeObject {
        [SerializeField] ColliderLayerMask _colliderLayer = ColliderLayerMask.Default;
        public ColliderLayerMask Layer { 
            get { return _colliderLayer; }
            set {
                if (_colliderLayer == value) return;
                _colliderLayer = value;
                InitalizeLayerMask();
            }
        }
        public string Tag { get; set; }
        public UnityEvent OnCollisionEvent { get; private set; } = new UnityEvent();
        private int _layerMask;
        [SerializeField] protected Color _gizmoColor = Color.red;

        protected virtual void Start() {
            CollisionManager.GetInstance().AddCollider(this);
            InitalizeLayerMask();
        }

        void InitalizeLayerMask() {
            switch (_colliderLayer) {
            case ColliderLayerMask.Default:
                _layerMask = 1;
                break;
            case ColliderLayerMask.DetectPlayer:
                AddBitMask(ColliderLayerMask.PlayerHitbox);
                break;
            case ColliderLayerMask.PlayerHitbox:
                AddBitMask(ColliderLayerMask.EnemyAttack);
                AddBitMask(ColliderLayerMask.EnemyHitbox);
                break;
            case ColliderLayerMask.EnemyHitbox:
                AddBitMask(ColliderLayerMask.PlayerAttack);
                break;
            case ColliderLayerMask.Door:
            case ColliderLayerMask.Ground:
            case ColliderLayerMask.PlayerAttack:
            case ColliderLayerMask.EnemyAttack:
                _layerMask = 0;
                break;
            }
        }

        void AddBitMask(ColliderLayerMask targetMask) {
            _layerMask |= (1 << (int)targetMask);
        }
        void RemoveBitMask(ColliderLayerMask targetMask) {
            _layerMask &= ~(1 << (int)targetMask);
        }
        public bool CannotCollision(ColliderLayerMask other) {
            return (_layerMask & (1 << (int)other)) == 0;
        }
        public abstract bool IsCollision(CustomCollider collider);
        public void OnCollision(CustomCollider collider) {
            OnCollisionEvent?.Invoke();
        }
        public abstract Rectangle GetBounds();
    }
}