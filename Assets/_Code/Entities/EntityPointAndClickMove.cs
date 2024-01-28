using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class EntityPointAndClickMove : MonoBehaviour
{
    #region member variables

    public float _speed = 1f;
    public System.Action OnMoveCompleted;
    
    private Entity _entity;
    private UnityEngine.AI.NavMeshAgent _agent;
    private Animator _animator;

    #endregion

    void Start()
    {
        _entity = GetComponent<Entity>();
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _agent.speed = _speed;
    }

    void Update()
    {
        if (_entity._alive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    MoveTo(hitInfo.point);
                }
            }

            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                OnMoveCompleted?.Invoke();
                StopMoving();
            }
        }
    }

    
    public void MoveTo(Vector3 target)
    {
        _agent.SetDestination(target);
        float distance = Vector3.Distance(transform.position, target);
        _animator.SetBool("IsWalking", true);
    }

    public void StopMoving()
    {
        _agent.SetDestination(transform.position);
        _agent.velocity = Vector3.zero;
        _animator.SetBool("IsWalking", false);
    }
}
