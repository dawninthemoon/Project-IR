using System.Collections.Generic;

[System.Serializable]
public class ActionGraphBaseData
{
    public string                               _name;
    public ActionGraphNodeData[]                _actionNodeData = null;
    public ActionGraphBranchData[]              _branchData = null;
    public ActionGraphConditionCompareData[]    _conditionCompareData = null;
    public AnimationPlayDataInfo[]              _animationPlayData = null;

    public ActionGraphSubEntityNodeData[]       _subEntityData = null;

    public Dictionary<string, int>             _actionIndexMap = new Dictionary<string, int>();

    public int[]                                _defaultBuffList = null;

    public int                                  _defaultActionIndex = -1;

    public int                                  _actionNodeCount = -1;
    public int                                  _branchCount = -1;
    public int                                  _conditionCompareDataCount = -1;
    public int                                  _animationPlayDataCount = -1;
    public int                                  _subEntityDataCount = -1;

    public void buildActionIndexMap()
    {
        for(int i = 0; i < _actionNodeCount; ++i)
        {
            _actionIndexMap.Add(_actionNodeData[i]._nodeName,i);
        }
    }
}

[System.Serializable]
public class ActionGraphSubEntityNodeData
{
    public string   _name;
    public string   _actionGraphPath;
    public string   _aiGraphPath;

    public bool     _attachToParent = false;
    public UnityEngine.Vector3 _spawnOffset = UnityEngine.Vector3.zero;
}

[System.Serializable]
public class ActionGraphNodeData
{
    public ActionGraphNodeData()
    {
        _animationInfoIndex = -1;
        _movementType = MovementBase.MovementType.Empty;
        _directionType = DirectionType.AlwaysRight;
        _defenceDirectionType = DefenceDirectionType.Direction;
        
        _index = -1;
        _branchIndexStart = 0;
        _branchCount = 0;
    }

    public string                       _nodeName;
    public int                          _animationInfoIndex;
    public MovementBase.MovementType    _movementType = MovementBase.MovementType.Empty;
    public DirectionType                _directionType = DirectionType.AlwaysRight;
    public DefenceDirectionType         _defenceDirectionType = DefenceDirectionType.Direction;
    public RotationType                 _rotationType = RotationType.AlwaysRight;
    public FlipType                     _flipType = FlipType.AlwaysTurnOff;
    public DefenceType                  _defenceType = DefenceType.Empty;

    public int[]                        _applyBuffList = null;

    public float                        _defenceAngle = 0f;
    public float                        _moveScale = 1f;
    public bool                         _normalizedSpeed = false;

    public bool                         _isActionSelection = false;
    public bool                         _directionUpdateOnce = false;
    public bool                         _flipTypeUpdateOnce = false;

    public int                          _index;
    public int                          _branchIndexStart;
    public int                          _branchCount;
}

public enum DirectionType
{
    AlwaysRight = 0,
    Keep,
    MoveInput,
    MoveInputHorizontal,
    MousePoint,
    MousePointHorizontal,
    AttackedPoint,
    MoveDirection,
    AI,
    AITarget,
    Count,
}

public enum DefenceDirectionType
{
    Direction = 0,
    MousePoint,
    Count,
}

[System.Serializable]
public class ActionGraphBranchData
{
    public int      _branchActionIndex;

    public int      _conditionCompareDataIndex = -1;
    public int      _keyConditionCompareDataIndex = -1;
    public int      _weightConditionCompareDataIndex = -1;
}

public enum ConditionCompareType
{
    Equals = 0,
    Inverse,
    NotEquals,
    Or,
    And,
    Greater,
    Smaller,
    GreaterEqual,
    SmallerEqual,
    Count,
};

public enum DefenceType
{
    Empty,
    Guard,
    Parry,
    Evade,
    Count,
}

public enum FlipType
{
    AlwaysTurnOff = 0,
    Direction,
    MousePoint,
    Keep,
    Count,
};

public enum RotationType
{
    AlwaysRight = 0,
    Direction,
    MousePoint,
    MoveDirection,
    Keep,
    Count,
}

public enum ConditionNodeUpdateType
{
    Literal = 0,
    ConditionResult,

    Action_Test,
    Action_Dash,
    Action_AnimationEnd,
    Action_AngleBetweenStick,
    Action_SectorFromStick,
    Action_AngleDirection,
    Action_IsXFlip,
    Action_IsYFlip,
    Action_CurrentFrame,
    Action_OnGround,
    Action_VelocityX,
    Action_VelocityY,
    

    Input_AttackCharge,
    Input_AttackBlood,
    Input_Guard,

    Attack_Guarded,
    Attack_Success,
    Attack_Parried,
    Attack_Evaded,
    Attack_GuardBreak,

    Defence_Success,
    Defence_Crash,
    Defence_Parry,
    Defence_Evade,
    Defence_GuardBroken,

    Defence_Hit,

    Entity_Dead,

    AI_TargetDistance,
    AI_TargetExists,
    AI_ArrivedTarget,
    AI_CurrentPackageEnd,
    AI_PackageStateExecutedTime,
    AI_GraphStateExecutedTime,

    Status,
    Key,
    FrameTag,
    TargetFrameTag,
    Weight,

    Count,
}

public enum ConditionNodeType
{
    Int = 0,
    Float,
    Bool,
    Count,
}

[System.Serializable]
public class ConditionNodeInfo
{
    public ConditionNodeInfo(ConditionNodeUpdateType updateType, ConditionNodeType nodeTpye)
    {
        _updateType = updateType;
        _nodeType = nodeTpye;
    }

    public ConditionNodeUpdateType  _updateType;
    public ConditionNodeType        _nodeType;
}

static public class CommonConditionNodeData
{
    static public byte[] trueByte = new byte[1]{1};
    static public byte[] falseByte = new byte[1]{0};
}

public static class ConditionNodeInfoPreset
{
    public static System.Collections.Generic.Dictionary<string,ConditionNodeInfo> _nodePreset = new System.Collections.Generic.Dictionary<string, ConditionNodeInfo>
    {
        {"Literal_Bool",new ConditionNodeInfo(ConditionNodeUpdateType.Literal, ConditionNodeType.Bool)},
        {"Literal_Int",new ConditionNodeInfo(ConditionNodeUpdateType.Literal, ConditionNodeType.Int)},
        {"Literal_Float",new ConditionNodeInfo(ConditionNodeUpdateType.Literal, ConditionNodeType.Float)},
        {"RESULT",new ConditionNodeInfo(ConditionNodeUpdateType.ConditionResult, ConditionNodeType.Bool)},
        {"ActionTest",new ConditionNodeInfo(ConditionNodeUpdateType.Action_Test, ConditionNodeType.Bool)},
        {"ActionDash",new ConditionNodeInfo(ConditionNodeUpdateType.Action_Dash, ConditionNodeType.Bool)},
        {"End",new ConditionNodeInfo(ConditionNodeUpdateType.Action_AnimationEnd, ConditionNodeType.Bool)},
        {"AngleBetweenStick",new ConditionNodeInfo(ConditionNodeUpdateType.Action_AngleBetweenStick, ConditionNodeType.Float)},
        {"SectorFromStick",new ConditionNodeInfo(ConditionNodeUpdateType.Action_SectorFromStick, ConditionNodeType.Int)},
        {"AngleDirection",new ConditionNodeInfo(ConditionNodeUpdateType.Action_AngleDirection, ConditionNodeType.Float)},

        {"IsXFlip",new ConditionNodeInfo(ConditionNodeUpdateType.Action_IsXFlip, ConditionNodeType.Bool)},
        {"IsYFlip",new ConditionNodeInfo(ConditionNodeUpdateType.Action_IsYFlip, ConditionNodeType.Bool)},
        {"CurrentFrame",new ConditionNodeInfo(ConditionNodeUpdateType.Action_CurrentFrame, ConditionNodeType.Float)},
        {"OnGround",new ConditionNodeInfo(ConditionNodeUpdateType.Action_OnGround, ConditionNodeType.Bool)},

        {"VelocityX",new ConditionNodeInfo(ConditionNodeUpdateType.Action_VelocityX, ConditionNodeType.Float)},
        {"VelocityY",new ConditionNodeInfo(ConditionNodeUpdateType.Action_VelocityY, ConditionNodeType.Float)},


        {"InputAttackCharge",new ConditionNodeInfo(ConditionNodeUpdateType.Input_AttackCharge, ConditionNodeType.Bool)},
        {"InputAttackBlood",new ConditionNodeInfo(ConditionNodeUpdateType.Input_AttackBlood, ConditionNodeType.Bool)},
        {"InputGuard",new ConditionNodeInfo(ConditionNodeUpdateType.Input_Guard, ConditionNodeType.Bool)},

        {"AttackGuarded",new ConditionNodeInfo(ConditionNodeUpdateType.Attack_Guarded, ConditionNodeType.Bool)},
        {"AttackSuccess",new ConditionNodeInfo(ConditionNodeUpdateType.Attack_Success, ConditionNodeType.Bool)},
        {"AttackParried",new ConditionNodeInfo(ConditionNodeUpdateType.Attack_Parried, ConditionNodeType.Bool)},
        {"AttackEvaded",new ConditionNodeInfo(ConditionNodeUpdateType.Attack_Evaded, ConditionNodeType.Bool)},
        {"AttackGuardBreak",new ConditionNodeInfo(ConditionNodeUpdateType.Attack_GuardBreak, ConditionNodeType.Bool)},

        {"DefenceSuccess",new ConditionNodeInfo(ConditionNodeUpdateType.Defence_Success, ConditionNodeType.Bool)},
        {"DefenceCrash",new ConditionNodeInfo(ConditionNodeUpdateType.Defence_Crash, ConditionNodeType.Bool)},
        {"ParrySuccess",new ConditionNodeInfo(ConditionNodeUpdateType.Defence_Parry, ConditionNodeType.Bool)},
        {"Hit",new ConditionNodeInfo(ConditionNodeUpdateType.Defence_Hit, ConditionNodeType.Bool)},
        {"EvadeSuccess",new ConditionNodeInfo(ConditionNodeUpdateType.Defence_Evade, ConditionNodeType.Bool)},
        {"GuardBroken",new ConditionNodeInfo(ConditionNodeUpdateType.Defence_GuardBroken, ConditionNodeType.Bool)},

        {"Dead",new ConditionNodeInfo(ConditionNodeUpdateType.Entity_Dead, ConditionNodeType.Bool)},

        {"TargetExists",new ConditionNodeInfo(ConditionNodeUpdateType.AI_TargetExists, ConditionNodeType.Bool)},
        {"TargetDistance",new ConditionNodeInfo(ConditionNodeUpdateType.AI_TargetDistance, ConditionNodeType.Float)},
        {"ArrivedTarget",new ConditionNodeInfo(ConditionNodeUpdateType.AI_ArrivedTarget, ConditionNodeType.Bool)},
        {"CurrentPackageEnd",new ConditionNodeInfo(ConditionNodeUpdateType.AI_CurrentPackageEnd, ConditionNodeType.Bool)},
        {"PackageExecutedTime",new ConditionNodeInfo(ConditionNodeUpdateType.AI_PackageStateExecutedTime, ConditionNodeType.Float)},
        {"GraphExecutedTime",new ConditionNodeInfo(ConditionNodeUpdateType.AI_GraphStateExecutedTime, ConditionNodeType.Float)},

        {"Status",new ConditionNodeInfo(ConditionNodeUpdateType.Status, ConditionNodeType.Float)},
        {"Key",new ConditionNodeInfo(ConditionNodeUpdateType.Key, ConditionNodeType.Bool)},
        {"FrameTag",new ConditionNodeInfo(ConditionNodeUpdateType.FrameTag, ConditionNodeType.Bool)},
        {"TargetFrameTag",new ConditionNodeInfo(ConditionNodeUpdateType.TargetFrameTag, ConditionNodeType.Bool)},
        {"Weight",new ConditionNodeInfo(ConditionNodeUpdateType.Weight, ConditionNodeType.Bool)},
        
    };

    public static int[] _dataSize =
    {
        4, // int
        4, // float
        1, // boolean
    };
}
[System.Serializable]
public class ActionGraphConditionNodeData
{
    public string _symbolName;
}

[System.Serializable]
public class ActionGraphConditionNodeData_FrameTag : ActionGraphConditionNodeData
{
    public string _targetFrameTag = "";

    public ActionGraphConditionNodeData_FrameTag()
    {
        
    }
    
    public string getFrameTag() {return _targetFrameTag;}
    
}
[System.Serializable]
public class ActionGraphConditionNodeData_Weight : ActionGraphConditionNodeData
{
    public string _weightGroupKey = "";
    public string _weightName = "";

    public ActionGraphConditionNodeData_Weight()
    {
        
    }
    
}
[System.Serializable]
public class ActionGraphConditionNodeData_Key : ActionGraphConditionNodeData
{
    public string _targetKeyName = "";

    public ActionGraphConditionNodeData_Key()
    {
        
    }
    
    public string getKeyName() {return _targetKeyName;}
    
}
[System.Serializable]
public class ActionGraphConditionNodeData_Status : ActionGraphConditionNodeData
{
    public string _targetStatus = "";

    public ActionGraphConditionNodeData_Status()
    {
        
    }
    
    public string getStatus() {return _targetStatus;}
    
}
[System.Serializable]
public class ActionGraphConditionNodeData_Literal : ActionGraphConditionNodeData
{
    private byte[]       _data;

    public ActionGraphConditionNodeData_Literal()
    {
        _data = null;
    }
    
    public byte[] getLiteral()
    {
        DebugUtil.assert(_data != null, "literal data is null");
        return _data;
    }

    public void setLiteral(byte[] data)
    {
        DebugUtil.assert(data != null, "literal data is null to set");
        _data = data;
    }
    
}
[System.Serializable]
public class ActionGraphConditionNodeData_ConditionResult : ActionGraphConditionNodeData
{
    public int          _resultIndex;

    public ActionGraphConditionNodeData_ConditionResult()
    {
        _symbolName = "RESULT_";
        _resultIndex = -1;
    }
    
    public int getResultIndex()
    {
        DebugUtil.assert(_resultIndex != -1, "resultIndex is not valid");
        return _resultIndex;
    }

    public void setResultIndex(int index)
    {
        _resultIndex = index;
    }
}

[System.Serializable]
public class ActionGraphConditionCompareData
{
    public ActionGraphConditionNodeData[]   _conditionNodeDataArray;
    public ConditionCompareType[]           _compareTypeArray;

    public int                              _conditionNodeDataCount;
    public int                              _compareTypeCount;

}