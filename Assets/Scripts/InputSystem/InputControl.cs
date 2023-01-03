using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using System.Linq;

public class InputControl : MonoBehaviour, ILoopable
{
    private class ActionInfo {
        public bool WasPressed;
        public bool IsKeyDown;
        public bool IsKeyUp;
        public PlayerAction Actions;
        public ActionInfo(PlayerAction action) {
            WasPressed = false;
            IsKeyUp = false;
            IsKeyDown = false;
            Actions = action;
        }
    }

    private HandleGroundMove _handleMove;

    private InputActions _actions;
    private Dictionary<string, ActionInfo> _wasPressedAtLastFrame;
    private bool _isJumpPressed;
    private PlayerFSM.States[] _inputIgnoreStates, _moveIgnoreStates;

    public void Initalize(PlayerFSM playerFSM, HandleGroundMove handleMove) {
        _handleMove = handleMove;

        InitKeyActions();

        _wasPressedAtLastFrame = new Dictionary<string, ActionInfo>();
        _wasPressedAtLastFrame.Add(InputActions.JumpActionName, new ActionInfo(_actions.Jump));
        _wasPressedAtLastFrame.Add(InputActions.AttackActionName, new ActionInfo(_actions.Attack));
        _wasPressedAtLastFrame.Add(InputActions.ThrowActionName, new ActionInfo(_actions.Throw));
    }

    private void InitKeyActions() {
        _actions = new InputActions();

        _actions.Left.AddDefaultBinding(Key.A);
        _actions.Left.AddDefaultBinding(InputControlType.DPadLeft);
        _actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);

        _actions.Right.AddDefaultBinding(Key.D);
        _actions.Right.AddDefaultBinding(InputControlType.DPadRight);
        _actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);

        _actions.Up.AddDefaultBinding(Key.W);
        _actions.Up.AddDefaultBinding(InputControlType.DPadUp);
        _actions.Up.AddDefaultBinding(InputControlType.LeftStickUp);

        _actions.Down.AddDefaultBinding(Key.S);
        _actions.Down.AddDefaultBinding(InputControlType.DPadDown);
        _actions.Down.AddDefaultBinding(InputControlType.LeftStickDown);

        _actions.Jump.AddDefaultBinding(Key.Space);
        _actions.Jump.AddDefaultBinding(InputControlType.Action1);

        _actions.Attack.AddDefaultBinding(Mouse.LeftButton);
        _actions.Attack.AddDefaultBinding(InputControlType.Action3);

        _actions.Throw.AddDefaultBinding(Key.A);
        _actions.Throw.AddDefaultBinding(InputControlType.Action2);
    }

    public bool GetKeyDown(string actionName) {
        ActionInfo info;
        if (_wasPressedAtLastFrame.TryGetValue(actionName, out info)) {
            return info.IsKeyDown;
        }
        return false;
    }

    public bool GetKeyUp(string actionName) {
        ActionInfo info;
        if (_wasPressedAtLastFrame.TryGetValue(actionName, out info)) {
            return info.IsKeyUp;
        }
        return false;
    }
    public void Progress() {
        SetupActionInfo();
        InputKeys();
    }

    void InputKeys() {
        //if (CheckCannotInput()) return;

        float horizontal = 0f, vertical = 0f;
        //if (!CheckCannotMove()) {
            horizontal = IgnoreSmallValue(_actions.Horizontal.Value);
            vertical = IgnoreSmallValue(_actions.Vertical.Value);
        //}
        
        _handleMove.SetDirectionalInput(new Vector2(horizontal, vertical));

        //if (!CheckCannotJump()) {
            if (GetKeyDown(InputActions.JumpActionName))
                _handleMove.OnJumpInputDown();
            else if (GetKeyUp(InputActions.JumpActionName))
                _handleMove.OnJumpInputUp();
        //}

        bool isAttackPressed = GetKeyDown(InputActions.AttackActionName);
        //SetAttack(isAttackPressed);

        _isJumpPressed = _actions.Jump.IsPressed;
    }

    void SetupActionInfo() {
        foreach (var pair in _wasPressedAtLastFrame) {
            var action = pair.Value;

            action.IsKeyDown = !action.WasPressed && action.Actions.IsPressed;
            action.IsKeyUp = action.WasPressed && !action.Actions.IsPressed;
            
            action.WasPressed = action.Actions.IsPressed;
        }
    }

    float IgnoreSmallValue(float value) {
        value = (Mathf.Abs(value) > 0.5f) ? value : 0f;
        value = (value == 0f) ? value : Mathf.Sign(value);
        return value;
    }
    /*
    bool CheckCannotInput() {
        var state = _playerFSM.State;
        
        if (_inputIgnoreStates == null) {
            _inputIgnoreStates = new PlayerStateControl.States[] {
                
            };
        }
        

        return _inputIgnoreStates.Contains(state);
    }

    bool CheckCannotMove() {
        var state = _playerFSM.State;
        
        if (_moveIgnoreStates == null) {
            _moveIgnoreStates = new PlayerStateControl.States[] {
                
            };
        }
        

        return _moveIgnoreStates.Contains(state);
    }

    bool CheckCannotJump() {
        var state = _playerFSM.State;
        return false;
        //return (state == PlayerStateControl.States.AttackA) || (state == PlayerStateControl.States.AttackB);
    }*/
}