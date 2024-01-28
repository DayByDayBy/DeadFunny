using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceUI : MonoBehaviour
{
    void Start()
    {
        RawImage image = GetComponent<RawImage>();
        Visualizer.Instance._faceUI = image;
    }
}
