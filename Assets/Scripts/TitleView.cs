using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TitleView : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private GameObject _finishText;

    void Awake()
    {
        this.gameObject.SetActive(true);
        _finishText.SetActive(false);
        _startButton.onClick.AsObservable()
        .Select(_ => 1)
        .Subscribe(_ =>
        {
            this.gameObject.SetActive(false);
            _startButton.gameObject.SetActive(false);
        })
        .AddTo(this);
    }

    public void Finish()
    {
        _finishText.SetActive(true);
        _startButton.gameObject.SetActive(false);
    }
}
