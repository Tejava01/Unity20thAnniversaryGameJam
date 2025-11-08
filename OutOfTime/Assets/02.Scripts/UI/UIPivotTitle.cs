using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPivotTitle : MonoBehaviour
{
    [SerializeField] private bool _ignoreMouseClick = true;
    [SerializeField] private TextMeshProUGUI _promptText;
    [SerializeField] private string _promptMessage = "- Press any key to start -";
    [SerializeField] private bool _enableBlink = true;
    [SerializeField] private float _blinkSpeed = 0.5f;
    

    private bool _isInputDetected = false;

    private void Start()
    {
        _promptText.text = _promptMessage;

        if (_enableBlink)
        {
            StartCoroutine(BlinkText());
        }
    }

    void Update()
    {
        if (_isInputDetected) return;

        if (Input.anyKeyDown)
        {
            if (_ignoreMouseClick && IsMouseButton())
            {
                return;
            }

            OnAnyKeyPressed();
        }
    }

    //------------------------------------------

    private bool IsMouseButton()
    {
        for (int i = 0; i < 7; i++)
        {
            if (Input.GetMouseButtonDown(i) || Input.mouseScrollDelta.x != 0 || Input.mouseScrollDelta.y != 0)
            {
                return true;
            }
        }

        return false;
    }

    private void OnAnyKeyPressed()
    {
        _isInputDetected = true;

        StopAllCoroutines();
        if (_promptText.enabled == false)
        {
            _promptText.enabled = true;
        }

        LoadScene(SceneName.SceneBoss);
    }

    private void LoadScene(SceneName sceneName)
    {
        ManagerScene.Instance.LoadScene(sceneName);
    }

    private IEnumerator BlinkText()
    {
        while (true)
        {
            _promptText.enabled = !_promptText.enabled;
            yield return new WaitForSeconds(_blinkSpeed);
        }
    }
}
