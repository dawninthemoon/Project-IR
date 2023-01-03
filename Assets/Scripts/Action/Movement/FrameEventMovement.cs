using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameEventMovement : MovementBase
{
    public static readonly float _gravity = -400.0f;


    private GameEntityBase _targetEntity;

    public enum FrameEventMovementValueType
    {
        Speed = 0,
        Velocity,
        MaxVelocity,
        Friction,
        Count,
    };

    private float[] _movementValues = new float[(int)FrameEventMovementValueType.Count];
    private Vector3 _currentVelocity = Vector3.zero;
    private float _gravityAccumulate = 0f;

    private GroundController _controller;

    public override MovementType getMovementType(){return MovementType.FrameEvent;}

    public override void initialize(GameEntityBase targetEntity)
    {
        _targetEntity = targetEntity;
        _currentDirection = Vector3.zero;

        if(_controller == null)
        {
            _controller = targetEntity.GetComponent<GroundController>();
            _controller.Initialize();
        }

        int numMovementValue = (int)FrameEventMovementValueType.Count;
        for(int i = 0; i < numMovementValue; ++i)
        {
            _movementValues[i] = 0f;
        }

        _currentVelocity = Vector3.zero;
    }

    public override void updateFirst(GameEntityBase targetEntity)
    {
        
    }

    public override bool progress(float deltaTime, Vector3 direction)
    {
        if(_targetEntity == null || _controller == null)
        {
            DebugUtil.assert(false, "invalid movement update call");
            return false;
        }

        _currentDirection = direction;

        float resultSpeed = _movementValues[0] + (_movementValues[0] >= 0 ? -_movementValues[3] : _movementValues[3]);
        
        Vector3 moveDelta = (_currentDirection * _movementValues[0]) * deltaTime;
        _gravityAccumulate += _gravity * deltaTime;

        _currentVelocity += moveDelta;

        Vector3 velocityDirection = _currentVelocity.normalized;
        _currentVelocity -= _currentVelocity.normalized * _movementValues[3] * deltaTime;

        if(Vector3.Angle(_currentVelocity.normalized, velocityDirection) > 100f)
            _currentVelocity = Vector3.zero;
        else if(_currentVelocity.sqrMagnitude > _movementValues[2] * _movementValues[2])
            _currentVelocity = _currentVelocity.normalized * _movementValues[2];

        Vector3 movementOfFrame = (_currentVelocity + (Vector3.up * _gravityAccumulate)) * deltaTime;
        _controller.Move(new Vector2(movementOfFrame.x,movementOfFrame.y),false);

        bool onGround = _controller.collisions.above || _controller.collisions.below;

        if (onGround) 
        {
			// if (_controller.collisions.slidingDownMaxSlope) 
            // {
			// 	_currentVelocity.y += _controller.collisions.slopeNormal.y * -_gravity * Time.deltaTime;
			// } 
            // else 
            {
				_currentVelocity.y = 0f;
                _gravityAccumulate = 0f;
			}
		}

        _targetEntity.getActionGraph().setActionConditionData_Bool(ConditionNodeUpdateType.Action_OnGround, onGround);
        _targetEntity.getActionGraph().setActionConditionData_Float(ConditionNodeUpdateType.Action_VelocityX, _currentVelocity.x);
        _targetEntity.getActionGraph().setActionConditionData_Float(ConditionNodeUpdateType.Action_VelocityY, _gravityAccumulate + _currentVelocity.y);

        return true;
    }

    public override void release()
    {
        
    }

    public void addVelocity(Vector3 velocity, bool resetGravity)
    {
        if(resetGravity)
            _gravityAccumulate = 0f;
        
        _currentVelocity += velocity;
    }

    public void addJump(float power)
    {
        _gravityAccumulate += power;
    }

    public void setJump(float power)
    {
        _gravityAccumulate = power;
    }

    public void setMovementValue(float value, int valueType)
    {
        if(valueType == 1)
        {
            _gravityAccumulate = 0f;

            if(MathEx.equals(_targetEntity.getDirection().sqrMagnitude, 0f, float.Epsilon) == false)
                _currentVelocity = _targetEntity.getDirection() * value;

            return;
        }

        _movementValues[valueType] = value;
    }


    public override Vector3 getCurrentDirection() {return _currentVelocity.normalized;}
}