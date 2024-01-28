using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZhex1991.EZSoftBone;

public class EntityWacker : MonoBehaviour
{
    #region member variables

    public float _wackForce = 10f;
    public float _wackRadius = 1f;
    public float _angle = 45f;

    private GameController _gameController;
    private Animator _animator;
    private bool _canWack = true;

    #endregion

    void Start()
    {
        _gameController = GameController.Instance;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        Entity target = _gameController._currentTarget;
        if (!target) return;
        bool canHit = Vector3.Angle(transform.forward, target.transform.position - transform.position) < _angle;
        if (target && canHit && _canWack)
        {
            if (Vector3.Distance(transform.position, target.transform.position) < _wackRadius)
            {
                _canWack = false;
                _animator.SetTrigger("Stab");
                StartCoroutine(SnapshotCO());
                // get all the colliders within the wack radius
                string enemyTag = "Enemy";
                target.transform.GetComponentInChildren<EZSoftBone>().enabled = false;
                Collider[] colliders = Physics.OverlapSphere(transform.position, _wackRadius, LayerMask.GetMask(enemyTag));
                Vector3 wackDir = (target.transform.position - transform.position).normalized;
                foreach (Collider collider in colliders)
                {
                    // if the collider is a target, wack it
                    target.Die();
                    _gameController.ChooseNextTarget();
                    collider.GetComponent<Rigidbody>().AddForce(wackDir * _wackForce + Vector3.up * _wackForce, ForceMode.Impulse);
                    collider.GetComponent<Rigidbody>().AddTorque(wackDir * _wackForce, ForceMode.Impulse);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _wackRadius);
    }

    private IEnumerator SnapshotCO()
    {
        yield return new WaitForSeconds(0.5f);
        Visualizer.Instance.GetScreenshot();
        yield return new WaitForSeconds(0.5f);
        _canWack = true;
    }
}
