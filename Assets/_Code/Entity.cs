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
        
       _entityVoiceLines.OnVoiceLineCompleted += OnVoiceLineCompleted;
       _entityWander.OnWanderCompleted += OnWanderCompleted;

        _entityVoiceLines.StartVoiceLines();
        _entityWander.StartWandering();
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
