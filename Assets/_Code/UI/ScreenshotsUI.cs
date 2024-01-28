using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotsUI : MonoBehaviour
{
    public List<RawImage> _screenshots;

    void Start()
    {
        List<Texture2D> screenshots = Visualizer.Instance._screenshots; // 6 of these
        for (int i = 0; i < 6; i++)
        {
            _screenshots[i].texture = screenshots[i];
        }
    }
}
