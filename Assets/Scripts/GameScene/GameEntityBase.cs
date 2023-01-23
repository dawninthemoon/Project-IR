using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityBase : MonoBehaviour
{
    public string               actionGraphPath = "Assets\\Data\\ActionGraph\\TestPlayer.xml";

    private ActionGraph         _actionGraph;
    private MovementControl     _movementControl = new MovementControl();


    private AttackState         _attackState = AttackState.Default;
    private DefenceState        _defenceState = DefenceState.Default;
    private DefenceType         _currentDefenceType = DefenceType.Empty;

    private SpriteRenderer      _spriteRenderer;

    private FlipState           _flipState = new FlipState();
    
    private Vector3             _direction = Vector3.right;
    private Quaternion          _spriteRotation = Quaternion.identity;

    private Vector3             _currentVelocity = Vector3.zero;
    private Vector3             _recentlyAttackPoint = Vector3.zero;
    private Vector3             _defenceDirection = Vector3.zero;

    private bool                _updateDirection = true;
    private bool                _updateFlipType = true;


    public void Assign()
    {
        _actionGraph = new ActionGraph(ActionGraphLoader.readFromXML(IOControl.PathForDocumentsFile(actionGraphPath)));
        _actionGraph.assign();

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize()
    {        
        _actionGraph.initialize();
        _movementControl.changeMovement(this,_actionGraph.getCurrentMovement());
        _movementControl.setMoveScale(_actionGraph.getCurrentMoveScale());
    }

    public void Progress(float deltaTime)
    {

        // if(_aiGraph != null)
        // {
        //     _aiGraph.updateConditionData();
            
        //     _actionGraph.setActionConditionData_Float(ConditionNodeUpdateType.AI_TargetDistance, getDistance(_currentTarget));
        //     _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.AI_TargetExists, _currentTarget != null);
        //     _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.AI_ArrivedTarget, _aiGraph.isAIArrivedTarget());
        //     _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.AI_CurrentPackageEnd, _aiGraph.isCurrentPackageEnd());
        //     _actionGraph.setActionConditionData_TargetFrameTag(_currentTarget == null ? null : _currentTarget.getCurrentFrameTagList());
        //     _actionGraph.setActionConditionData_Float(ConditionNodeUpdateType.AI_PackageStateExecutedTime, _aiGraph.getCurrentPackageExecutedTime());
        //     _actionGraph.setActionConditionData_Float(ConditionNodeUpdateType.AI_GraphStateExecutedTime, _aiGraph.getCurrentGraphExecutedTime());

        //     _aiGraph.progress(deltaTime,this);
        // }
        // else
        {
            _actionGraph.setActionConditionData_Float(ConditionNodeUpdateType.AI_TargetDistance, 0f);
            _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.AI_TargetExists, false);
            _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.AI_ArrivedTarget, false);
            _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.AI_CurrentPackageEnd, false);
            _actionGraph.setActionConditionData_Float(ConditionNodeUpdateType.AI_PackageStateExecutedTime, 0f);
            _actionGraph.setActionConditionData_Float(ConditionNodeUpdateType.AI_GraphStateExecutedTime, 0f);
        }

        if(_actionGraph != null)
        {
            string prevActionName = _actionGraph.getCurrentActionName();
            
            updateConditionData();

            //action,movementGraph 바뀌는 시점
            if(_actionGraph.progress() == true)
            {
                if(_actionGraph.isActionLoop() == false)
                {
                    _movementControl.changeMovement(this,_actionGraph.getCurrentMovement());
                    _movementControl.setMoveScale(_actionGraph.getCurrentMoveScale());
                }

                _currentDefenceType = _actionGraph.getCurrentDefenceType();
                _updateDirection = true;
                _updateFlipType = true;
            }

            updateDirection();
            
            //animation 바뀌는 시점
            _actionGraph.updateAnimation(deltaTime, this);
            _movementControl?.progress(deltaTime, _direction);
            
            _spriteRenderer.sprite = _actionGraph.getCurrentSprite();
            _spriteRenderer.transform.localRotation = _actionGraph.getCurrentAnimationRotation();
            _spriteRenderer.transform.localScale = _actionGraph.getCurrentAnimationScale();
            
            if(_updateFlipType)
            {
                _flipState = getCurrentFlipState();
                _updateFlipType = _actionGraph.getCurrentFlipTypeUpdateOnce() == false;
            }

            _spriteRenderer.flipX = _flipState.xFlip;
            _spriteRenderer.flipY = _flipState.yFlip;
        }

        updateRotation();
    }


    public FlipState getCurrentFlipState()
    {
        FlipState currentFlipState = _actionGraph.getCurrentFlipState();
        FlipState flipState = new FlipState();

        FlipType flipType = _actionGraph.getCurrentFlipType();

        switch(flipType)
        {
            case FlipType.Direction:
                flipState = _flipState;
                
                if(MathEx.abs(_direction.x) != 0f && currentFlipState.xFlip == true)
                    flipState.xFlip = _direction.x < 0;
                if(MathEx.abs(_direction.y) != 0f && currentFlipState.yFlip == true)
                    flipState.yFlip = _direction.y < 0;

                break;
            case FlipType.MousePoint:
                Vector3 direction = ControllerEx.GetInstance().getJoystickAxisR(transform.position);
                if(MathEx.abs(direction.x) != 0f && currentFlipState.xFlip == true)
                    flipState.xFlip = direction.x < 0;
                if(MathEx.abs(direction.y) != 0f && currentFlipState.yFlip == true)
                    flipState.yFlip = direction.y < 0;
                break;
            case FlipType.Keep:
                flipState.xFlip = _spriteRenderer.flipX;
                flipState.yFlip = _spriteRenderer.flipY;
                break;
        }

        DebugUtil.assert((int)FlipType.Count == 4, "flip type count error");

        return flipState;
    }


    private void updateDirection()
    {
        if(_updateDirection == false)
            return;

        DirectionType directionType = DirectionType.AlwaysRight;
        DefenceDirectionType defenceDirectionType = DefenceDirectionType.Direction;

        if(_actionGraph != null)
        {
            directionType = _actionGraph.getDirectionType();
            defenceDirectionType = _actionGraph.getDefenceDirectionType();
        }

        _direction = getDirectionFromType(directionType);

        switch(defenceDirectionType)
        {
            case DefenceDirectionType.Direction:
                _defenceDirection = _direction;
                break;
            case DefenceDirectionType.MousePoint:
                _defenceDirection = ControllerEx.GetInstance().getJoystickAxisR(transform.position);
                break;
        }

        _updateDirection = _actionGraph.getCurrentDirectionUpdateOnce() == false;
    }

    public Vector3 getDirectionFromType(DirectionType directionType)
    {
        Vector3 direction = _direction;
        switch(directionType)
        {
            case DirectionType.AlwaysRight:
                direction = Vector3.right;
                break;
            case DirectionType.Keep:
                break;
            case DirectionType.MoveInput:
            case DirectionType.MoveInputHorizontal:
            {
                Vector3 input = ControllerEx.GetInstance().GetJoystickAxis();
                if(MathEx.equals(input.sqrMagnitude,0f,float.Epsilon) == false )
                {
                    direction = input;
                    direction.Normalize();
                }
                else
                {
                    direction = Vector3.zero;
                }

                if(directionType == DirectionType.MoveInputHorizontal)
                {
                    direction.y = 0f;
                    direction.Normalize();
                }
            }
            break;
            case DirectionType.MousePoint:
            case DirectionType.MousePointHorizontal:
            {
                direction = ControllerEx.GetInstance().getJoystickAxisR(transform.position);

                if(directionType == DirectionType.MousePointHorizontal)
                {
                    direction.y = 0f;
                    direction.Normalize();
                }
            }
            break;
            case DirectionType.AttackedPoint:
                direction = (_recentlyAttackPoint - transform.position).normalized;
                break;
            case DirectionType.AI:
                // direction = _aiGraph.getRecentlyAIDirection();
                break;
            case DirectionType.AITarget:
                // if(_currentTarget != null && _currentTarget.isValid())
                //     direction = (_currentTarget.transform.position - transform.position).normalized;
                break;
            case DirectionType.MoveDirection:
                direction = getMovementControl().getMoveDirection();
                break;
            case DirectionType.Count:
                DebugUtil.assert(false, "invalid direction type : {0}",_actionGraph.getDirectionType());
                break;
        }

        return direction;
    }

    private void updateRotation()
    {
        RotationType rotationType = RotationType.AlwaysRight;
        if(_actionGraph != null)
            rotationType = _actionGraph.getCurrentRotationType();

        switch(rotationType)
        {
            case RotationType.AlwaysRight:
                _spriteRotation = Quaternion.identity;
                break;
            case RotationType.Direction:
                _spriteRotation = Quaternion.Euler(0f,0f,MathEx.directionToAngle(_direction));
                break;
            case RotationType.MousePoint:
                _spriteRotation = Quaternion.Euler(0f,0f,MathEx.directionToAngle( ControllerEx.GetInstance().getJoystickAxisR(transform.position)));
                break;
            case RotationType.MoveDirection:
                _spriteRotation = Quaternion.Euler(0f,0f,MathEx.directionToAngle(getMovementControl().getMoveDirection()));
                break;
            case RotationType.Keep:
                break;
    
        }

        DebugUtil.assert((int)RotationType.Count == 5, "check this");

        float zRotation = _spriteRotation.eulerAngles.z;
        if(rotationType != RotationType.AlwaysRight)
            zRotation -= (_flipState.xFlip ? -180f : 0f);

        _spriteRenderer.transform.localRotation *= Quaternion.Euler(0f,0f,zRotation);
    }

    public void updateConditionData()
    {
        Vector3 input = ControllerEx.GetInstance().GetJoystickAxis();
        Vector3 inputDirection = ControllerEx.GetInstance().getJoystickAxisR(transform.position);

        float angleBetweenStick = MathEx.clampDegree(Vector3.SignedAngle(input, inputDirection,Vector3.forward));
        float angleDirection = MathEx.clampDegree(Vector3.SignedAngle(Vector3.right, _direction, Vector3.forward));
        
        float directionAngle = MathEx.directionToAngle(inputDirection);
        int sector = 0;
        if((directionAngle < 22.5) || (angleDirection >= 337.5))
            sector = 0;
        else if((directionAngle < 67.5) && (directionAngle >= 22.5))
            sector = 1;
        else if((directionAngle < 112.5) && (directionAngle >= 67.5))
            sector = 2;
        else if((directionAngle < 157.5) && (directionAngle >= 112.5))
            sector = 3;
        else if((directionAngle < 202.5) && (directionAngle >= 157.5))
            sector = 4;
        else if((directionAngle < 247.5) && (directionAngle >= 202.5))
            sector = 5;
        else if((directionAngle < 292.5) && (directionAngle >= 247.5))
            sector = 6;
        else if((directionAngle < 337.5) && (directionAngle >= 292.5))
            sector = 7;

        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Action_Test, MathEx.equals(input.sqrMagnitude,0f,float.Epsilon) == false);
        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Action_Dash, Input.GetKey(KeyCode.Space));
        _actionGraph.setActionConditionData_Float(ConditionNodeUpdateType.Action_AngleBetweenStick, angleBetweenStick);
        _actionGraph.setActionConditionData_Int(ConditionNodeUpdateType.Action_SectorFromStick, sector);
        _actionGraph.setActionConditionData_Float(ConditionNodeUpdateType.Action_AngleDirection, angleDirection);

        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Action_IsXFlip, _flipState.xFlip);
        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Action_IsYFlip, _flipState.yFlip);
        _actionGraph.setActionConditionData_Float(ConditionNodeUpdateType.Action_CurrentFrame, _actionGraph.getCurrentFrame());

        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Input_AttackCharge, Input.GetMouseButton(0));
        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Input_AttackBlood, Input.GetKey(KeyCode.R));
        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Input_Guard, Input.GetMouseButton(1));

        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Attack_Guarded, _attackState == AttackState.AttackGuarded);
        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Attack_Success, _attackState == AttackState.AttackSuccess);
        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Attack_Parried, _attackState == AttackState.AttackParried);
        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Attack_Evaded, _attackState == AttackState.AttackEvade);
        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Attack_GuardBreak, _attackState == AttackState.AttackGuardBreak);

        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Defence_Crash, _defenceState == DefenceState.DefenceCrash);
        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Defence_Success, _defenceState == DefenceState.DefenceSuccess);
        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Defence_Parry, _defenceState == DefenceState.ParrySuccess);
        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Defence_Hit, _defenceState == DefenceState.Hit);
        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Defence_Evade, _defenceState == DefenceState.EvadeSuccess);
        _actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Defence_GuardBroken, _defenceState == DefenceState.GuardBroken);

        //_actionGraph.setActionConditionData_Bool(ConditionNodeUpdateType.Entity_Dead, _statusInfo.isDead());

    }


    public FlipState getFlipState() {return _flipState;}

    public ActionGraph getActionGraph() {return _actionGraph;}

    public void setAttackState(AttackState state) {_attackState = state;}
    public void setDefenceState(DefenceState state) {_defenceState = state;}


    public float getDistance(GameEntityBase obj)
    {
        if(obj == null)
            return -1f;
        
        return Vector3.Distance(transform.position,obj.transform.position);
    }
    public float getDistanceSq(GameEntityBase obj) {return (transform.position - obj.transform.position).sqrMagnitude;}

    public Transform getSpriteRendererTransform()
    {
        if(_spriteRenderer == null)
            return null;

        return _spriteRenderer.transform;
    }

    public bool applyFrameTag(string tag) {return _actionGraph.applyFrameTag(tag);}
    public void deleteFrameTag(string tag) {_actionGraph.deleteFrameTag(tag);}
    public bool checkFrameTag(string tag) {return _actionGraph.checkFrameTag(tag);}
    
    public Vector3 getDirection() {return _direction;}
    public MovementBase getCurrentMovement() {return _movementControl.getCurrentMovement();}
    public MovementControl getMovementControl(){return _movementControl;}
}
