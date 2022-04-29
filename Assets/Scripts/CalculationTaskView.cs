using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

/// <summaray>
/// 計算タスクのView
/// </summary>
public class CalculationTaskView : MonoBehaviour
{
    [SerializeField] private TitleView _titleView;
    [SerializeField] private GameObject _mainView;
    [SerializeField] private TMP_Text _pageIndex;
    [SerializeField] private TMP_Text _taskProgress;
    [SerializeField] private TMP_Text[] _problemNumbers;
    [SerializeField] private TMP_InputField _answerText;
    [SerializeField] private Button _nextButton;
    [SerializeField] private int _maxProblemNumber = 200;
    public IObservable<Unit> OnClickNextButton => _nextButton.onClick.AsObservable();

    private void Awake()
    {
        _titleView.gameObject.SetActive(true);
        _mainView.SetActive(false);
    }

    public void SetIndexNumber(int num)
    {
        _pageIndex.text = $"No.{num}";
        _taskProgress.text = $"{num}/{_maxProblemNumber}";
    }

    public void SetProblemNumbers(int[] numbers)
    {
        if (_problemNumbers.Length != numbers.Length) throw new IndexOutOfRangeException();
        for (var i = 0; i < numbers.Length; i++)
        {
            _problemNumbers[i].text = $"{numbers[i]}";
        }
    }
    public void StartGame()
    {
        _titleView.gameObject.SetActive(false);
        _mainView.SetActive(true);
    }

    public void FinishGame()
    {
        _titleView.gameObject.SetActive(true);
        _mainView.SetActive(false);
        _titleView.Finish();
    }

    public string GetAnswerText() => _answerText.text;
    public void DeleteAnswerText() => _answerText.text = "";
    public int GetMaxProblemNumber() => _maxProblemNumber;
}
