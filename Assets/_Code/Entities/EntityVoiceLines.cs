using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EntityVoiceLines : MonoBehaviour
{
    #region member variables

    public List<AudioClip> _voiceLines = new List<AudioClip>();
    public float _minTimeBetweenVoiceLines = 5f;
    public float _maxTimeBetweenVoiceLines = 10f;
    public System.Action OnVoiceLineCompleted;

    private Entity _entity;
    private AudioSource _audioSource;
    private List<AudioClip> _availableClips = new List<AudioClip>();

    #endregion

    void Awake()
    {
        _entity = GetComponent<Entity>();
        _audioSource = GetComponent<AudioSource>();
        _availableClips.AddRange(_voiceLines);
    }

    public void StartVoiceLines()
    {
        StartCoroutine(VoiceLines());
    }

    public void StopVoiceLines()
    {
        StopAllCoroutines();
    }

    private IEnumerator VoiceLines()
    {
        if (_entity && _entity._alive)
        {
            // wait for a bit
            yield return new WaitForSeconds(Random.Range(_minTimeBetweenVoiceLines, _maxTimeBetweenVoiceLines));

            if (_availableClips.Count == 0)
                _availableClips.AddRange(_voiceLines);

            // if you have a line to say
            if (_availableClips.Count > 0)
            {
                // say your line
                _audioSource.clip = _availableClips[Random.Range(0, _voiceLines.Count)];
                _availableClips.Remove(_audioSource.clip);
                _audioSource.Play();
                // _audioSource.pitch = Random.Range(0.8f, 1.2f);
                yield return new WaitForSeconds(_audioSource.clip.length);
                _audioSource.Stop();
                _audioSource.pitch = 1f;
                OnVoiceLineCompleted?.Invoke();
            }
        }
    }
}