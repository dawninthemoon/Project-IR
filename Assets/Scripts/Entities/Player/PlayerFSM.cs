using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using MonsterLove.StateMachine;

public partial class PlayerFSM : MonoBehaviour {
    public enum States { 
        Idle, Move,
    };
    [SerializeField] private SpriteAtlas _spriteAtlas = null;
    private StateMachine<States> _fsm;
    private SpriteAtlasAnimator _animator;

    #region Non-reference Fields
    private Vector2 _direction;
    private bool _jumpRequested;
    #endregion
    
    public States State { get => _fsm.State; }

    public void Initalize() {
        //_animator = new SpriteAtlasAnimator(GetComponentInChildren<SpriteRenderer>(), "PLAYER_", "Idle_loop", true, 1f);
        _fsm = GetComponent<StateMachineRunner>().Initialize<States>(this);
        _fsm.ChangeState(States.Idle);
    }

    public void Progress() {
        //_animator.Progress(_spriteAtlas);
    }

    public void ApplyAnimation(float dirX, float velocityY) {
        _direction.x = dirX;
        _direction.y = velocityY;

        if (Mathf.Abs(dirX) > Mathf.Epsilon) {
            SetDirection(dirX);
        }
    }

    public void OnJumpButtonDown() {
        _jumpRequested = true;
    }

    public void SetDirection(float dirX) {
        Vector3 scaleVector = new Vector3(Mathf.Sign(dirX), 1f, 1f);
        transform.localScale = scaleVector;
    }
}