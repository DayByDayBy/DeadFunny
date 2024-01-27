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

    #endregion

    void Start()
    {
        _entity = GetComponent<Entity>();
        _audioSource = GetComponent<AudioSource>();
        StartCoroutine(VoiceLines());
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
        if (_entity._alive)
        {
            // wait for a bit
            yield return new WaitForSeconds(Random.Range(_minTimeBetweenVoiceLines, _maxTimeBetweenVoiceLines));

            // if you have a line to say
            if (_voiceLines.Count > 0)
            {
                // say your line
                _audioSource.clip = _voiceLines[Random.Range(0, _voiceLines.Count)];
                _audioSource.Play();
                _audioSource.pitch = Random.Range(0.8f, 1.2f);
                yield return new WaitForSeconds(_audioSource.clip.length);
                _audioSource.Stop();
                _audioSource.pitch = 1f;
                OnVoiceLineCompleted?.Invoke();
            }
        }
    }
}