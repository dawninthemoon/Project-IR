using UnityEngine;
using System.Xml;
using System.Collections.Generic;

public enum FrameEventType
{
    FrameEvent_Test = 0,
    FrameEvent_Attack,
    FrameEvent_ApplyBuff,
    FrameEvent_ApplyBuffTarget,
    FrameEvent_DeleteBuff,
    FrameEvent_SetDefenceType,
    FrameEvent_SetFrameTag,
    FrameEvent_Projectile,
    FrameEvent_Danmaku,
    FrameEvent_SetAnimationSpeed,
    FrameEvent_KillEntity,
    FrameEvent_Movement,
    FrameEvent_Jump,
    FrameEvent_Effect,
    
    FrameEvent_SubEntity_SetAction,
    FrameEvent_SubEntity_SetOffset,
    FrameEvent_SubEntity_SetParent,

    Count,
}

public enum ChildFrameEventType
{
    ChildFrameEvent_OnHit,
    ChildFrameEvent_OnGuard,
    ChildFrameEvent_OnParry,
    ChildFrameEvent_OnEvade,
    ChildFrameEvent_OnGuardBreak,
    Count,
}

public enum SetTargetType
{
    SetTargetType_Self,
    SetTargetType_Target,
    SetTargetType_AITarget,
}

public class ChildFrameEventItem
{
    public ActionFrameEventBase[] _childFrameEvents;
    public int _childFrameEventCount;

}

public abstract class ActionFrameEventBase
{
    public float _startFrame;
    public float _endFrame;

    public Dictionary<ChildFrameEventType, ChildFrameEventItem> _childFrameEventItems = null;
    
    public abstract FrameEventType getFrameEventType();
    public virtual void initialize(){}
    public abstract bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null);
    public virtual void onExit(GameEntityBase executeEntity){}
    public abstract void loadFromXML(XmlNode node);

    public void executeChildFrameEvent(ChildFrameEventType eventType, GameEntityBase executeEntity, GameEntityBase targetEntity)
    {
        if(_childFrameEventItems == null || _childFrameEventItems.ContainsKey(eventType) == false)
            return;
        
        ChildFrameEventItem childFrameEventItem = _childFrameEventItems[eventType];
        for(int i = 0; i < childFrameEventItem._childFrameEventCount; ++i)
        {
            childFrameEventItem._childFrameEvents[i].initialize();
            childFrameEventItem._childFrameEvents[i].onExecute(executeEntity, targetEntity);
        }
    }
}


public class ActionFrameEvent_SubEntity_SetOffset : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_SubEntity_SetOffset;}

    private string _subEntityName = "";
    private Vector3 _value = Vector3.zero;

    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        executeEntity.setSubEntityOffset(_subEntityName, _value);

        return true;
    }

    public override void loadFromXML(XmlNode node)
    {
        XmlAttributeCollection attributes = node.Attributes;
        for(int i = 0; i < attributes.Count; ++i)
        {
            string attrName = attributes[i].Name;
            string attrValue = attributes[i].Value;

            if(attrName == "EntityName")
            {
                _subEntityName = attrValue;
            }
            else if(attrName == "Offset")
            {
                _value = XMLScriptConverter.valueToVector3(attrValue);
            }

        }

    }
}


public class ActionFrameEvent_SubEntity_SetParent : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_SubEntity_SetParent;}

    private string _subEntityName = "";
    private bool _value = false;

    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        executeEntity.setSubEntityParent(_subEntityName, _value);

        return true;
    }

    public override void loadFromXML(XmlNode node)
    {
        XmlAttributeCollection attributes = node.Attributes;
        for(int i = 0; i < attributes.Count; ++i)
        {
            string attrName = attributes[i].Name;
            string attrValue = attributes[i].Value;

            if(attrName == "EntityName")
            {
                _subEntityName = attrValue;
            }
            else if(attrName == "Value")
            {
                _value = bool.Parse(attrValue);
            }

        }

    }
}

public class ActionFrameEvent_SubEntity_SetAction : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_SubEntity_SetAction;}

    private string _subEntityName = "";
    private string _actionName = "";

    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        executeEntity.setSubEntityAction(_subEntityName, _actionName);

        return true;
    }

    public override void loadFromXML(XmlNode node)
    {
        XmlAttributeCollection attributes = node.Attributes;
        for(int i = 0; i < attributes.Count; ++i)
        {
            string attrName = attributes[i].Name;
            string attrValue = attributes[i].Value;

            if(attrName == "EntityName")
            {
                _subEntityName = attrValue;
            }
            else if(attrName == "Action")
            {
                _actionName = attrValue;
            }

        }

    }
}

public class ActionFrameEvent_Jump : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_Jump;}

    private float _jumpPower = 0f;
    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        MovementBase currentMovement = executeEntity.getCurrentMovement();
        if(currentMovement == null)
            return false;
            
        if(currentMovement.getMovementType() != MovementBase.MovementType.FrameEvent)
        {
            DebugUtil.assert(false,"movement frame event is only can use, when movement type is frameEvent movement : currentType[{0}]", currentMovement.getMovementType().ToString());
            return false;
        }

        ((FrameEventMovement)currentMovement).setJump(_jumpPower);

        return true;
    }

    public override void loadFromXML(XmlNode node)
    {
        XmlAttributeCollection attributes = node.Attributes;
        for(int i = 0; i < attributes.Count; ++i)
        {
            string attrName = attributes[i].Name;
            string attrValue = attributes[i].Value;

            if(attrName == "Power")
            {
                float jumpPower;
                if(float.TryParse(attrValue, out jumpPower) == false)
                {
                    DebugUtil.assert(false,"invalid jump frameeevent value string: {0}",attrValue);
                    continue;
                }

                _jumpPower = jumpPower;
            }

        }

    }
}

public class ActionFrameEvent_Movement : ActionFrameEventBase
{
    struct MovementSetValueType
    {
        public float _value;
        public int _targetValue;
    };

    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_Movement;}

    private MovementSetValueType[] _setValueList = null;
    private int _valueListCount = 0;
    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        MovementBase currentMovement = executeEntity.getCurrentMovement();
        if(currentMovement == null)
            return false;
            
        if(currentMovement.getMovementType() != MovementBase.MovementType.FrameEvent)
        {
            DebugUtil.assert(false,"movement frame event is only can use, when movement type is frameEvent movement : currentType[{0}]", currentMovement.getMovementType().ToString());
            return false;
        }

        for(int i = 0; i < _valueListCount; ++i)
        {
            ((FrameEventMovement)currentMovement).setMovementValue(_setValueList[i]._value,_setValueList[i]._targetValue);
        }
        
        return true;
    }

    public override void loadFromXML(XmlNode node)
    {
        List<MovementSetValueType> movementSetValueList = new List<MovementSetValueType>();
        XmlAttributeCollection attributes = node.Attributes;
        for(int i = 0; i < attributes.Count; ++i)
        {
            string attrName = attributes[i].Name;
            string attrValue = attributes[i].Value;

            if(attrName != "Speed" && attrName != "Velocity" && attrName != "MaxVelocity" && attrName != "Friction")
                continue;

            float value;
            if(float.TryParse(attrValue, out value) == false)
            {
                DebugUtil.assert(false,"invalid movement frameeevent value string: {0}",attrValue);
                continue;
            }

            int targetValue = (int)((FrameEventMovement.FrameEventMovementValueType)System.Enum.Parse(typeof(FrameEventMovement.FrameEventMovementValueType), attrName));
            movementSetValueList.Add(new MovementSetValueType{_value = value, _targetValue = targetValue});
        }

        _setValueList = movementSetValueList.ToArray();
        _valueListCount = movementSetValueList.Count;
    }
}

public class ActionFrameEvent_KillEntity : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_KillEntity;}

    private string _path;

    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        //executeEntity.dispose(false);
        executeEntity.gameObject.SetActive(false);

        return true;
    }

    public override void loadFromXML(XmlNode node)
    {
    }
}


public class ActionFrameEvent_Danmaku : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_Danmaku;}

    private string _path;

    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        GameEntityBase requester = (GameEntityBase)executeEntity;
        
        //requester.addDanmaku(_path);

        return true;
    }

    public override void loadFromXML(XmlNode node)
    {
        XmlAttributeCollection attributes = node.Attributes;
        for(int i = 0; i < attributes.Count; ++i)
        {
            if(attributes[i].Name == "Path")
                _path = attributes[i].Value;
        }
    }
}


public class ActionFrameEvent_SetFrameTag : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_SetFrameTag;}

    private string _frameTag;

    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        return executeEntity.applyFrameTag(_frameTag);
    }

    public override void onExit(GameEntityBase executeEntity)
    {
        executeEntity.deleteFrameTag(_frameTag);
    }

    public override void loadFromXML(XmlNode node)
    {
        XmlAttributeCollection attributes = node.Attributes;
        for(int i = 0; i < attributes.Count; ++i)
        {
            if(attributes[i].Name == "Tag")
                _frameTag = attributes[i].Value;
        }
    }
}

public class ActionFrameEvent_SetDefenceType : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_SetDefenceType;}

    private DefenceType _defenceType;

    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        GameEntityBase requester = (GameEntityBase)executeEntity;
        
        //requester.setDefenceType(_defenceType);
        
        return true;
    }

    public override void loadFromXML(XmlNode node)
    {
        XmlAttributeCollection attributes = node.Attributes;
        for(int i = 0; i < attributes.Count; ++i)
        {
            if(attributes[i].Name == "DefenceType")
                _defenceType = (DefenceType)System.Enum.Parse(typeof(DefenceType), attributes[i].Value);
        }
    }
}

public class ActionFrameEvent_SetAnimationSpeed : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_SetAnimationSpeed;}

    private float _speed = 1f;

    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        if(executeEntity is GameEntityBase == false)
            return false;
        
        GameEntityBase requester = (GameEntityBase)executeEntity;
        //requester.setAnimationSpeed(_speed);

        return true;
    }

    public override void loadFromXML(XmlNode node)
    {
        XmlAttributeCollection attributes = node.Attributes;
        for(int i = 0; i < attributes.Count; ++i)
        {
            if(attributes[i].Name == "Speed")
                _speed = float.Parse(attributes[i].Value);
        }
    }
}

public class ActionFrameEvent_DeleteBuff : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_DeleteBuff;}

    private int[] buffKeyList = null;

    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        if(executeEntity is GameEntityBase == false)
            return false;
        
        //((GameEntityBase)executeEntity).deleteActionBuffList(buffKeyList);

        return true;
    }

    public override void loadFromXML(XmlNode node)
    {
        XmlAttributeCollection attributes = node.Attributes;
        for(int i = 0; i < attributes.Count; ++i)
        {
            if(attributes[i].Name == "BuffList")
            {
                string[] buffList = attributes[i].Value.Split(' ');

                buffKeyList = new int[buffList.Length];
                for(int j = 0; j < buffList.Length; ++j)
                {
                    buffKeyList[j] = int.Parse(buffList[j]);
                }

            }
        }
    }
}

public class ActionFrameEvent_ApplyBuff : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_ApplyBuff;}

    private int[] buffKeyList = null;

    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        if(executeEntity is GameEntityBase == false)
            return false;
        
        //((GameEntityBase)executeEntity).applyActionBuffList(buffKeyList);
        
        return true;
    }

    public override void loadFromXML(XmlNode node)
    {
        XmlAttributeCollection attributes = node.Attributes;
        for(int i = 0; i < attributes.Count; ++i)
        {
            if(attributes[i].Name == "ApplyBuff")
            {
                string[] buffList = attributes[i].Value.Split(' ');

                buffKeyList = new int[buffList.Length];
                for(int j = 0; j < buffList.Length; ++j)
                {
                    buffKeyList[j] = int.Parse(buffList[j]);
                }

            }
        }
    }
}

public class ActionFrameEvent_ApplyBuffTarget : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_ApplyBuffTarget;}

    private int[] buffKeyList = null;

    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        //target.applyActionBuffList(buffKeyList);

        return true;
    }

    public override void loadFromXML(XmlNode node)
    {
        XmlAttributeCollection attributes = node.Attributes;
        for(int i = 0; i < attributes.Count; ++i)
        {
            if(attributes[i].Name == "ApplyBuff")
            {
                string[] buffList = attributes[i].Value.Split(' ');

                buffKeyList = new int[buffList.Length];
                for(int j = 0; j < buffList.Length; ++j)
                {
                    buffKeyList[j] = int.Parse(buffList[j]);
                }

            }
        }
    }
}

public class ActionFrameEvent_Attack : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_Attack;}

    //private CollisionInfo           _collisionInfo;
    //private CollisionDelegate       _collisionDelegate;
    private System.Action           _collisionEndEvent;
    
    private DefenceType[]           _ignoreDefenceType = null;

    private AttackType              _attackType;
    private Vector3     _pushVector = Vector3.zero;

    private HashSet<GameEntityBase> _collisionList = new HashSet<GameEntityBase>();
    //private List<CollisionSuccessData> _collisionOrder = new List<CollisionSuccessData>();

    public ActionFrameEvent_Attack()
    {
        // _collisionDelegate = attackPrepare;
        // _collisionEndEvent = attackProcess;
    }

    public override void initialize()
    {
        // _collisionList.Clear();
        // _collisionOrder.Clear();
    }

    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        // _collisionInfo.updateCollisionInfo(executeEntity.transform.position,executeEntity.getDirection());
        // _collisionInfo.drawCollosionArea(Color.red,1f);
        // CollisionManager.Instance().collisionRequest(_collisionInfo,executeEntity,_collisionDelegate,_collisionEndEvent);

        return true;
    }   

    public void attackProcess()
    {
        // for(int i = 0; i < _collisionOrder.Count; ++i)
        // {
        //     if(attackTarget(_collisionOrder[i]) == false)
        //         break;
        // }

        // _collisionOrder.Clear();
    }

    // public void attackPrepare(CollisionSuccessData successData)
    // {
    //     if(successData._requester is GameEntityBase == false || successData._target is GameEntityBase == false)
    //         return;

    //     GameEntityBase requester = (GameEntityBase)successData._requester;
    //     GameEntityBase targetEntity = (GameEntityBase)successData._target;

    //     if(targetEntity._searchIdentifier == requester._searchIdentifier)
    //         return;
        
    //     float distanceSq = (((GameEntityBase)successData._target).transform.position - successData._startPoint).sqrMagnitude;
    //     for(int i = 0; i < _collisionOrder.Count; ++i)
    //     {
    //         GameEntityBase target = (GameEntityBase)_collisionOrder[i]._target;

    //         if((target.transform.position - successData._startPoint).sqrMagnitude > distanceSq)
    //         {
    //             _collisionOrder.Insert(i,successData);
    //             return;
    //         }
    //     }

    //     _collisionOrder.Add(successData);
    // }

    // private bool attackTarget(CollisionSuccessData successData)
    // {
    //     _collisionInfo.drawCollosionArea(Color.green,1f);

    //     GameEntityBase requester = (GameEntityBase)successData._requester;
    //     GameEntityBase target = (GameEntityBase)successData._target;

    //     if(_collisionList.Contains(target) == true)
    //         return true;
    //     else
    //         _collisionList.Add(target);


    //     ChildFrameEventType eventType = ChildFrameEventType.Count;

    //     Vector3 toTargetDirection = (requester.transform.position - target.transform.position).normalized;

    //     target.setAttackPoint(successData._startPoint);

    //     float attackInAngle = Vector3.Angle(target.getCurrentDefenceDirection(), (successData._startPoint - target.transform.position).normalized);

    //     bool guardSuccess = (attackInAngle < target.getDefenceAngle() * 0.5f);
    //     bool canIgnore = canIgnoreDefenceType(target.getDefenceType());

    //     bool attackSuccess = false;


    //     if(_pushVector.sqrMagnitude > float.Epsilon)
    //     {
    //         Vector3 attackPointDirection = (target.transform.position - successData._startPoint).normalized;
    //         target.addVelocity(Quaternion.Euler(0f,0f,Mathf.Atan2(attackPointDirection.y,attackPointDirection.x) * Mathf.Rad2Deg) * _pushVector);
    //     }

    //     if(((guardSuccess == false || target.getDefenceType() == DefenceType.Empty) && target.getDefenceType() != DefenceType.Evade) || canIgnore)
    //     {
    //         if(_attackType == AttackType.Default)
    //         {
    //             requester.setAttackState(AttackState.AttackSuccess);
    //             target.setDefenceState(DefenceState.Hit);

    //             if(requester is GameEntityBase)
    //                 ((GameEntityBase)requester).executeAIEvent(AIChildEventType.AIChildEvent_OnAttack);
    //             target.executeAIEvent(AIChildEventType.AIChildEvent_OnAttacked);

    //             eventType = ChildFrameEventType.ChildFrameEvent_OnHit;

    //         }
    //         else if(_attackType == AttackType.GuardBreak)
    //         {
    //             requester.setAttackState(AttackState.AttackGuardBreak);
    //             target.setDefenceState(DefenceState.GuardBroken);

    //             if(requester is GameEntityBase)
    //                 ((GameEntityBase)requester).executeAIEvent(AIChildEventType.AIChildEvent_OnGuardBreak);
    //             target.executeAIEvent(AIChildEventType.AIChildEvent_OnGuardBroken);

    //             eventType = ChildFrameEventType.ChildFrameEvent_OnGuardBreak;
    //         }

    //         attackSuccess = true;

    //     }
    //     else if(guardSuccess && target.getDefenceType() == DefenceType.Guard)
    //     {
    //         requester.setAttackState(AttackState.AttackGuarded);
    //         target.setDefenceState(DefenceState.DefenceSuccess);

    //         if(requester is GameEntityBase)
    //                 ((GameEntityBase)requester).executeAIEvent(AIChildEventType.AIChildEvent_OnGuarded);
    //         target.executeAIEvent(AIChildEventType.AIChildEvent_OnGuard);

    //         eventType = ChildFrameEventType.ChildFrameEvent_OnGuard;
    //     }
    //     else if(guardSuccess && target.getDefenceType() == DefenceType.Parry)
    //     {
    //         requester.setAttackState(AttackState.AttackParried);
    //         target.setDefenceState(DefenceState.ParrySuccess);
            
    //         if(requester is GameEntityBase)
    //                 ((GameEntityBase)requester).executeAIEvent(AIChildEventType.AIChildEvent_OnParried);
    //         target.executeAIEvent(AIChildEventType.AIChildEvent_OnParry);

    //         eventType = ChildFrameEventType.ChildFrameEvent_OnParry;
    //     }
    //     else if(guardSuccess && target.getDefenceType() == DefenceType.Evade)
    //     {
    //         requester.setAttackState(AttackState.AttackEvade);
    //         target.setDefenceState(DefenceState.EvadeSuccess);

    //         if(requester is GameEntityBase)
    //                 ((GameEntityBase)requester).executeAIEvent(AIChildEventType.AIChildEvent_OnEvaded);
    //         target.executeAIEvent(AIChildEventType.AIChildEvent_OnEvade);

    //         eventType = ChildFrameEventType.ChildFrameEvent_OnEvade;
    //     }
        

    //     executeChildFrameEvent(eventType, requester, target);

    //     return attackSuccess;
    // }

    private bool canIgnoreDefenceType(DefenceType defenceType)
    {
        if(_ignoreDefenceType == null || _ignoreDefenceType.Length == 0)
            return false;

        for(int i = 0; i < _ignoreDefenceType.Length; ++i)
        {
            if(_ignoreDefenceType[i] == defenceType)
                return true;
        }

        return false;
    }

    public override void loadFromXML(XmlNode node)
    {
        XmlAttributeCollection attributes = node.Attributes;
        float radius = 0f;
        float angle = 0f;
        float startDistance = 0f;
        _attackType = AttackType.Default;


        for(int i = 0; i < attributes.Count; ++i)
        {
            if(attributes[i].Name == "Radius")
            {
                radius = float.Parse(attributes[i].Value);
            }
            else if(attributes[i].Name == "Angle")
            {
                angle = float.Parse(attributes[i].Value);
            }
            else if(attributes[i].Name == "StartDistance")
            {
                startDistance = float.Parse(attributes[i].Value);
            }
            else if(attributes[i].Name == "AttackType")
            {
                _attackType = (AttackType)System.Enum.Parse(typeof(AttackType), attributes[i].Value);
            }
            else if(attributes[i].Name == "AttackPreset")
            {
                // AttackPreset preset = ResourceContainerEx.Instance().GetScriptableObject("Preset\\AttackPreset") as AttackPreset;
                // AttackPresetData presetData = preset.getPresetData(attributes[i].Value);
                // if(presetData == null)
                // {
                //     DebugUtil.assert(false, "failed to load attack preset: {0}",attributes[i].Value);
                //     return;
                // }

                // radius = presetData._attackRadius;
                // angle = presetData._attackAngle;
                // startDistance = presetData._attackStartDistance;
                // _pushVector = presetData._pushVector;
            }
            else if(attributes[i].Name == "IgnoreDefenceType")
            {
                string value = attributes[i].Value;
                
                if(value == null || value == "")
                {
                    DebugUtil.assert(false,"ignoreDefenceType must need type");
                    return;
                }

                string[] defencies = value.Split(' ');
                _ignoreDefenceType = new DefenceType[defencies.Length];

                for(int index = 0; index < defencies.Length; ++index)
                {
                    _ignoreDefenceType[index] = (DefenceType)System.Enum.Parse(typeof(DefenceType), defencies[index]);
                }
            }
            else if(attributes[i].Name == "Push")
            {
                string[] value = attributes[i].Value.Split(' ');
                if(value == null || value.Length > 3)
                {
                    DebugUtil.assert(false, "invalid Action Frame Event Data: Push, {0}",attributes[i].Value);
                    return;
                }

                _pushVector = new Vector3(float.Parse(value[0]),float.Parse(value[1]),float.Parse(value[2]));
            }

        }

        // CollisionInfoData data = new CollisionInfoData(radius,angle,startDistance, CollisionType.Attack);
        // _collisionInfo = new CollisionInfo(data);

        
    }
}

public class ActionFrameEvent_Test : ActionFrameEventBase
{
    public override FrameEventType getFrameEventType(){return FrameEventType.FrameEvent_Test;}

    private string _debugLog = "";

    public override bool onExecute(GameEntityBase executeEntity, GameEntityBase targetEntity = null)
    {
        Debug.Log("Test Frame Event : " + _debugLog);

        return true;
    }

    public override void loadFromXML(XmlNode node)
    {
        XmlAttributeCollection attributes = node.Attributes;
        for(int i = 0; i < attributes.Count; ++i)
        {
            if(attributes[i].Name == "Log")
                _debugLog = attributes[i].Value;
        }
    }
}

