using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundUI : MonoBehaviour
{
    #region member variables

    public RawImage _soundUI;

    private SoundDetector _soundDetector;

    #endregion

    void Start()
    {
        _soundUI = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_soundDetector)
        {
            _soundDetector = SoundDetector.Instance;
        }

        if (_soundDetector.MaxVolumeSet())
        {
            Color darkRed = new Color(0.5f, 0, 0, .1f);
            Color green = new Color(0, 1, 0, .1f);
            _soundUI.color = Color.Lerp(darkRed, green, _soundDetector.GetVolumeNormalized());

            // set image height to volume normalized
            float percent = _soundUI.rectTransform.sizeDelta.y * _soundDetector.GetVolumeNormalized();
            _soundUI.rectTransform.sizeDelta = new Vector2(_soundUI.rectTransform.sizeDelta.x, percent);
        }
        else
        {
            _soundUI.color = new Color(1, 1, 1, 0);
        }
    }
}
