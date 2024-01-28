using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicatorUI : MonoBehaviour
{
    public Transform _textUI;
    private GameController _gc;

    void Start()
    {
        _gc = GameController.Instance;
    }

    void Update()
    {
        if (_gc._currentTarget && _gc._currentTarget._alive)
        {
            _textUI.gameObject.SetActive(true);
            _textUI.position = _gc._currentTarget.transform.position + Vector3.up * 8f;
        }
        else
        {
            _textUI.gameObject.SetActive(false);
        }
    }
}
