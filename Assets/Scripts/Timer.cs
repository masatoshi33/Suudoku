using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private int _minute;
    [SerializeField] private float _seconds;
    //　前のUpdateの時の秒数
    private float _oldSeconds;
    //　タイマー表示用テキスト
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private bool IsCountUp;
    public int StartMinute;
    public float StartSeconds;
    [SerializeField] private bool _isCountStart;

    private DateTime _startTime;

    void Start()
    {
        _startTime = DateTime.Now;
        _minute = StartMinute;
        _seconds = StartSeconds;
        _oldSeconds = StartSeconds;
    }

    void Update()
    {
        if (!_isCountStart) return;
        if (IsCountUp) _seconds += Time.deltaTime;
        else _seconds -= Time.deltaTime;
        // カウントアップの場合
        if (_seconds >= 60f)
        {
            _minute++;
            _seconds = _seconds - 60f;
        }
        // カウントダウンの場合
        if (_seconds < 0f)
        {
            if (_minute > 0)
            {
                _minute--;
                _seconds = _seconds + 60f;
            }
        }
        //　値が変わった時だけテキストUIを更新
        if ((int)_seconds != (int)_oldSeconds)
        {
            _timerText.text = _minute.ToString("00") + ":" + ((int)_seconds).ToString("00");
        }
        _oldSeconds = _seconds;
    }

    public int GetMinute() => _minute;
    public float GetSeconds() => _seconds;
    public void SetTime(int minute, float seconds)
    {
        _minute = minute;
        _seconds = seconds;
    }
    public bool IsTimeUp() => _minute <= 0 && _seconds <= 0f;

    public bool StartCount() => _isCountStart = true;
    public bool StopCount() => _isCountStart = false;
    public bool IsStopCount() => !_isCountStart;
    public DateTime GetStartTime() => _startTime;
}