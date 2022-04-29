using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Data;

public class HundredCalculationPresenter : MonoBehaviour
{
    private static readonly string[] OutputTableLabels = { "date", "time", "x", "y", "answer", "isCorrect" };
    [SerializeField] private GameObject _titleView;
    [SerializeField] private Button _startButton;
    [SerializeField] private GameObject _finishText;
    [SerializeField] private Timer _timer;
    [SerializeField] private GameObject[] _fixObjects;
    [SerializeField] private string[] _problemPaths;
    [SerializeField] private SquareGridTexts _view;
    [SerializeField] private TextBox[] _horizontalTextBoxes;
    [SerializeField] private TextBox[] _verticalTextBoxes;

    private int[] _horizontalProblemNumbers = new int[10];
    private int[] _verticalProblemNumbers = new int[10];
    private int[][] _answer = new int[10][];
    private bool[,] _isLock2dArray = new bool[10, 10];
    private int _answerCount = 0;

    private DataTable _outputTable;
    [SerializeField] private string[] _outputFileName;
    [SerializeField] private int[] _problemIndexes;
    private int _problemIndex = 0;


    private async void Awake()
    {
        // タイトル画面
        _finishText.SetActive(false);
        _titleView.SetActive(true);
        _startButton.onClick.AsObservable().Subscribe(_ => _titleView.SetActive(false));

        // 全てのマスを空白にする
        var maxX = _view.GetMaxIndexes[0];
        var maxY = _view.GetMaxIndexes[1];
        for (var y = 0; y < maxY; y++)
        {
            for (var x = 0; x < maxX; x++)
            {
                _view.SetText(x, y, "");
            }
        }
        // 謎に線画非アクティブになる問題の解決用
        _view.gameObject.SetActive(false);
        _view.gameObject.SetActive(true);

        // 問題をロード
        var problemPath = $"HundredCalculation/{_problemPaths[0]}";
        LoadProblem(problemPath);
        SetAnswer();

        // 出力用CSV
        _outputTable = CSVOutput.SetUpDataTable(OutputTableLabels);

        // 解答を設置
        StartCountAsync().Forget();
    }

    private void Update()
    {
        var selectedIndexes = _view.GetSelectedIndexes;
        var selectedX = selectedIndexes[0];
        var selectedY = selectedIndexes[1];
        // 答えを確定させる
        if (Input.GetKeyDown(KeyCode.Return) && _view.GetText() != "")
        {
            // Enterを押したら、色を緑に変え、変更不能
            _isLock2dArray[selectedY, selectedX] = true;
            _answerCount++;
            var fixedColor = new Color(0, 255, 0, 255);
            _view.SetColor(fixedColor);
            var output = "";
            if (_answer[selectedY][selectedX].ToString() == _view.GetText()) output = "correct";
            else output = "wrong";
            // Debug.Log($"{output}, {_view.GetText()}, {_answer[selectedY][selectedX]}");

            // ファイル入力
            // 開始から何秒経ったか
            var time = (_timer.StartMinute - _timer.GetMinute()) * 60 + (_timer.StartSeconds - _timer.GetSeconds());
            var newData = new[] { DateTime.Now.ToString(), time.ToString(), selectedX.ToString(), selectedY.ToString(), _view.GetText(), output };
            CSVOutput.AddDataToDataTable(_outputTable, newData);
        }
        // 文字入力or削除
        if (!_isLock2dArray[selectedY, selectedX])
        {
            // 数字入力
            for (var i = 0; i < 10; i++)
            {
                if (Input.GetKeyDown(i.ToString())) _view.AddText(i.ToString());
            }
            // 削除
            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)) _view.DeleteText();
        }
        // 解き終わったら次の問題
        if (_answerCount == 100)
        {
            CSVOutput.SaveDataTable(_outputTable, _outputFileName[_problemIndex]);
            _problemIndex++;
            // 完全終了
            if (_problemIndexes.Length <= _problemIndex)
            {
                Finish();
                return;
            }
            RestartGame(_problemIndex).Forget();
        }
        // 時間切れの際
        if (_timer.IsTimeUp() && _timer.IsStopCount())
        {
            _timer.StopCount();
            Finish();
        }
    }

    ///<summary>問題の読み込み</sammary>
    public void LoadProblem(string path)
    {
        var problemTexts = LoadFile.LoadSimpleText(path);
        var horizontalProblems = new string[10];
        var verticalProblems = new string[10];
        Array.Copy(problemTexts, 0, horizontalProblems, 0, 10);
        Array.Copy(problemTexts, 10, verticalProblems, 0, 10);

        for (var i = 0; i < _horizontalTextBoxes.Length; i++)
        {
            // view
            _horizontalTextBoxes[i].SetText(horizontalProblems[i]);
            _verticalTextBoxes[i].SetText(verticalProblems[i]);
            // 数値データに変換して登録
            _horizontalProblemNumbers[i] = Int32.Parse(horizontalProblems[i]);
            _verticalProblemNumbers[i] = Int32.Parse(verticalProblems[i]);
        }
    }

    public void SetAnswer()
    {
        for (var y = 0; y < _verticalProblemNumbers.Length; y++)
        {
            _answer[y] = new int[10];
            for (var x = 0; x < _horizontalProblemNumbers.Length; x++)
            {
                _answer[y][x] = _verticalProblemNumbers[y] * _horizontalProblemNumbers[x];
                // Debug.Log($"answer({x},{y}):{_answer[y][x]}");
            }
        }
    }

    ///<summary>タイトルが非アクティブになるまでタイムカウントを待つ</sammary>
    private async UniTask StartCountAsync()
    {
        foreach (var obj in _fixObjects) obj.SetActive(false);
        await UniTask.WaitUntil(() => !_titleView.activeSelf);
        _timer.StartCount();
        foreach (var obj in _fixObjects) obj.SetActive(true);
    }

    ///<summary>ゲームをリセットしスタート</sammary>
    private async UniTask RestartGame(int index)
    {
        _outputTable = CSVOutput.SetUpDataTable(OutputTableLabels);
        await UniTask.Delay(0);
        var path = $"HundredCalculation/{_problemPaths[index]}";
        LoadProblem(path);
        SetAnswer();
    }

    private void Finish()
    {
        Debug.Log("Finish!!");
        _startButton.gameObject.SetActive(false);
        _titleView.SetActive(true);
        _finishText.SetActive(true);
    }
}