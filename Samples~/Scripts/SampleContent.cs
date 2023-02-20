using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyOutline;

public class SampleContent : MonoBehaviour
{
    [SerializeField] private Transform _stage;
    [SerializeField] private OutlineTag[] _outlineTags;
    
    private OutlineManager _outlineManager;
    private int _index = 0;
    
    private void Start()
    {
        _outlineManager = gameObject.GetComponent<OutlineManager>();
        if (_outlineTags.Length > 0)
        {
            _outlineManager.EnableOutline(_outlineTags[_index]);    
        }
    }

    private void Update()
    {
        var q = new Quaternion();
        q.eulerAngles = new Vector3(0.0f, 360.0f * Mathf.Repeat(Time.time * 0.13f, 1.0f), 0.0f);
        _stage.transform.rotation = q;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_outlineTags.Length == 0)
                return;
            
            _outlineManager.DisableOutline(_outlineTags[_index]);
            _index++;
            if (_index == _outlineTags.Length)
            {
                _index = 0;
            }
            _outlineManager.EnableOutline(_outlineTags[_index]);
        }
#endif
    }
    
#if UNITY_EDITOR
    private GUIStyle guiStyle = new GUIStyle();
    
    void OnGUI()
    {
        guiStyle.fontSize = 50;
        GUI.Label(new Rect(30, 30, 100, 50), "Press [Tab] To Next", guiStyle);
    }
#endif
}
