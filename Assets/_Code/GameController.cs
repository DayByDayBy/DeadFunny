using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Madd.Singleton<GameController>
{
    #region member variables

    public List<Entity> _targets = new List<Entity>();
    public Entity _currentTarget;

    #endregion

    void Start()
    {
        var entities = FindObjectsOfType<Entity>();
        foreach (var entity in entities)
        {
            if (entity.gameObject.tag == "Enemy")
                _targets.Add(entity);
        }
        ChooseNextTarget();
    }

    public void ChooseNextTarget()
    {
        if (_targets.Count > 0)
        {
            _currentTarget = _targets[Random.Range(0, _targets.Count)];
        }
    }

    public void RemoveTarget(Entity target)
    {
        _targets.Remove(target);
    }
}
