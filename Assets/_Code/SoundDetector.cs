using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

public class SoundDetector : Madd.Singleton<SoundDetector>
{

    #region member variables

    public float _sensitivity;
    public float _vol = 0;
    public float _maxVolume = 0;
    public float _offset;
    public float[] _samples;
    public bool _settingMaxVolume = false;

    private AudioSource _audio;
    private float _lastVolume = 0;

    #endregion

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _audio = GetComponent<AudioSource>();

        _audio.clip = Microphone.Start(Microphone.devices[0], true, 180, 44100);

        _audio.Play();

        _samples = new float[4096]; // 4096 = around 85 ms of samples
    }

    void Update()
    {
        _vol = GetRMS(0) + GetRMS(1);
        _vol *= _sensitivity - _offset;

        // lerp last volume
        _vol = Mathf.Lerp(_lastVolume, _vol, .05f);
        _lastVolume = _vol;

        if (_settingMaxVolume)
        {
            _maxVolume = _vol;
            _settingMaxVolume = false;
            Fungus.Flowchart.BroadcastFungusMessage ("laugh_ok");
        }
    }

    float GetRMS(int channel)
    {
        _audio.GetOutputData(_samples, channel); // fill array with samples
        float sum = 0;
        for (var i = 0; i < 4096; i++)
        {
            sum += _samples[i] * _samples[i]; // sum squared samples
        }
        return Mathf.Sqrt(sum / 4096); // rms = square root of average 
    }

    public float GetVolume()
    {
        return _vol;
    }

    public bool MaxVolumeSet()
    {
        return _maxVolume != 0;
    }

    public float GetVolumeNormalized()
    {
        if (_maxVolume == 0)
            return 0;
        return _vol / _maxVolume;
    }

    public void SetMaxVolume()
    {
        _settingMaxVolume = true;
    }

}