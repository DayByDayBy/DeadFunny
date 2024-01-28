using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameController : Madd.Singleton<GameController>
{
    #region member variables

    public List<Entity> _targets = new List<Entity>();
    public Entity _currentTarget;
    public Entity _comedian;
    public List<AudioClip> _laughTracks = new List<AudioClip>();
    public bool CanKill = false;
    public TMPro.TextMeshProUGUI _targetUI;

    #endregion

    void Start()
    {
        InitGame();
    }

    void InitGame()
    {
        var entities = FindObjectsOfType<Entity>();
        foreach (var entity in entities)
        {
            if (entity.gameObject.tag == "Enemy")
                _targets.Add(entity);
        }
        ChooseNextTarget();
        _comedian.GetComponent<EntityVoiceLines>().OnVoiceLineCompleted += OnVoiceLineCompleted;
        _comedian.GetComponent<EntityVoiceLines>().StartVoiceLines();

        // randomly make people wander
        foreach (var entity in _targets)
        {
            if (Random.Range(0, 2) == 0)
            {
                entity.GetComponent<EntityWander>().StartWandering();
            }
        }
        StartCoroutine(SmileCheckCO());
    }

    IEnumerator SmileCheckCO()
    {
        yield return new WaitForSeconds(2);
        if (Visualizer.Instance._similarity < .8f)
        {
            Fungus.Flowchart.BroadcastFungusMessage("smile_more");
        }
        StartCoroutine(SmileCheckCO());
    }

    void OnVoiceLineCompleted()
    {
        // play random laugh track
        GetComponent<AudioSource>().clip = _laughTracks[Random.Range(0, _laughTracks.Count)];
        GetComponent<AudioSource>().Play();
        GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
        // get entities to stop, laugh and face the comedian
        foreach (var entity in _targets)
        {
            entity.Laugh();
            entity.GetComponent<EntityWander>().FaceDirection(_comedian.transform.position);
        }
        StartCoroutine(CompleteLaughTrack());
        CanKill = true;
        _targetUI.text = "Kill";
    }

    IEnumerator CompleteLaughTrack()
    {
        yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().pitch = 1f;
        _comedian.GetComponent<EntityVoiceLines>().StartVoiceLines();

        // randomly make people wander
        foreach (var entity in _targets)
        {
            if (Random.Range(0, 2) == 0)
            {
                entity.GetComponent<EntityWander>().StartWandering();
            }
        }
        ChooseNextTarget();
        CanKill = false;
        _targetUI.text = "Wait";
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
        _targets = _targets.FindAll(t => t != target);
        _currentTarget = null;
    }
}
