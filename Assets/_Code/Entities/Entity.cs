using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Entity : MonoBehaviour
{
    #region member variables

    public bool _alive = true;

    private EntityWander _entityWander;
    private EntityVoiceLines _entityVoiceLines;

    #endregion

    void Start()
    {
        _entityWander = GetComponent<EntityWander>();
        _entityVoiceLines = GetComponent<EntityVoiceLines>();

       if (_entityVoiceLines) 
       {
            _entityVoiceLines.OnVoiceLineCompleted += OnVoiceLineCompleted;
            _entityVoiceLines.StartVoiceLines();
       }
       if (_entityWander)
       {
            _entityWander.OnWanderCompleted += OnWanderCompleted;
            _entityWander.StartWandering();
       }
    }

    public void Die()
    {
        _alive = false;
        if (_entityVoiceLines)
        {
        }
        if (_entityWander)
        {
            _entityWander.Die();
        }
        GameController.Instance.RemoveTarget(this);
    }

    void OnVoiceLineCompleted()
    {
        _entityVoiceLines.StartVoiceLines();
    }

    void OnWanderCompleted()
    {
        _entityWander.StartWandering();
    }
}
