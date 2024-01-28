using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZhex1991.EZSoftBone;
using Fungus;

public class EntityWacker : MonoBehaviour
{
    #region member variables

    public float _wackForce = 10f;
    public float _wackRadius = 1f;
    public float _angle = 45f;

    public AudioClip _wackSound;
    public List<AudioClip> _deathNoises = new List<AudioClip>();

    private GameController _gameController;
    private Animator _animator;
    private AudioSource _audioSource;
    private bool _canWack = true;
    private int _warnings = 1;
    private int _wargningBeforeUpdating = 3;

    #endregion

    void Start()
    {
        _gameController = GameController.Instance;
        _animator = GetComponent<Animator>();
        _audioSource = gameObject.AddComponent<AudioSource>();
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
                    collider.GetComponent<Rigidbody>().AddForce(wackDir * _wackForce + Vector3.up * _wackForce, ForceMode.Impulse);
                    collider.GetComponent<Rigidbody>().AddTorque(wackDir * _wackForce, ForceMode.Impulse);
                    _audioSource.clip = _wackSound;
                    _audioSource.Play();
                    StartCoroutine(DeathNoiseCO());
                }
                print("Can Kill: " + _gameController.CanKill);
                if (!_gameController.CanKill)
                {
                    switch (_warnings)
                    {
                        case 1:
                        Fungus.Flowchart.BroadcastFungusMessage("warning_1");
                        break;
                        case 2:
                        Fungus.Flowchart.BroadcastFungusMessage("warning_2");
                            break;
                        case 3:
                        Fungus.Flowchart.BroadcastFungusMessage("warning_3");
                        break;
                    }
                    _wargningBeforeUpdating--;
                    if (_wargningBeforeUpdating <= 0)
                    {
                        _warnings++;
                        _wargningBeforeUpdating = 3;
                    }
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
    }

    private IEnumerator DeathNoiseCO()
    {
        yield return new WaitForSeconds(.5f);
        _audioSource.clip = _deathNoises[Random.Range(0, _deathNoises.Count)];
        _audioSource.Play();
        _canWack = true;
    } 
}
