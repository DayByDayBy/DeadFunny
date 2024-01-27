using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EntityWander : MonoBehaviour
{
    #region member variables

    public float _maxWanderDistance = 5f;
    public float _speed = 1f;
    public System.Action OnWanderCompleted;

    private Entity _entity;
    private UnityEngine.AI.NavMeshAgent _agent;
    private Vector3 _target, _previousTarget;

    #endregion

    void Start()
    {
        _entity = GetComponent<Entity>();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _agent.speed = _speed;
    }

    void Update()
    {
        if (_entity._alive)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                OnWanderCompleted?.Invoke();
            }
        }
    }

    public void StartWandering()
    {
        _previousTarget = _target;
        _target = GetRandomPoint();
        _agent.SetDestination(_target);
        float distance = Vector3.Distance(transform.position, _target);
    }

    public void StopWandering()
    {
        _agent.SetDestination(transform.position);
        _agent.velocity = Vector3.zero;
    }

    private Vector3 GetRandomPoint()
    {
        Vector3 randomPoint = Random.insideUnitSphere * _maxWanderDistance;
        randomPoint += transform.position;
        randomPoint.y = transform.position.y;
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, _maxWanderDistance, 1);
        return hit.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _maxWanderDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_target, 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_previousTarget, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_previousTarget, _target);
    }
}
