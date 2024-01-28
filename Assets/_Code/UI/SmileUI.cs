using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmileUI : MonoBehaviour
{
    #region member variables

    public Slider _soundUI;
    public Image _sliderBG;

    private Visualizer _smileDetector;

    #endregion

    void Start()
    {
        _soundUI = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_smileDetector)
        {
            _smileDetector = Visualizer.Instance;
        }
        else if (_smileDetector.FaceSet())
        {
            Color darkRed = new Color(0.5f, 0, 0, .1f);
            Color green = new Color(0, 1, 0, .1f);
            _sliderBG.color = Color.Lerp(darkRed, green, _smileDetector.GetSimilarity());

            // set image height to volume normalized
            float percent = _smileDetector.GetSimilarity();
            _soundUI.value = percent;
        }
        else
        {
            _sliderBG.color = new Color(1, 1, 1, 0);
        }
    }
}
