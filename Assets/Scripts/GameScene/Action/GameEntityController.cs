using UnityEngine;

public class GameEntityController
{
    private GameEntityBase _targetGameEntity;
    private string _entityName;

    public GameEntityController(string entityName, GameEntityBase target)
    {
        _entityName = entityName;
        _targetGameEntity = target;
    }

    public void progress(float deltaTime)
    {
        _targetGameEntity.Progress(deltaTime);
    }

    public string getName()
    {
        return _entityName;
    }

    public void changeAction(int actionIndex)
    {
        _targetGameEntity.setAction(actionIndex);
    }

    public void changeAction(string actionName)
    {
        _targetGameEntity.setAction(actionName);
    }

    public void setOffset(Vector3 offset)
    {
        _targetGameEntity.transform.localPosition = offset;
    }

    public void setParent(bool value, Transform parent)
    {
        _targetGameEntity.transform.SetParent(parent);
    }

    public void setEnable(bool value)
    {
        _targetGameEntity.gameObject.SetActive(value);
    }

    public bool isEnable()
    {
        return _targetGameEntity.gameObject.activeInHierarchy;
    }
}
