using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using TMPro;
using System;
using System.Data;

public class SuudokuMainView : MonoBehaviour
{
    private static readonly string[] OutputTableLabels = { "date", "time", "x", "y", "number", "isCorrect" };
    [SerializeField] private GameObject _titleView;
    [SerializeField] private TMPTextList[] _numberTextsRoot;
    [SerializeField] private RectTransform _selectedCursor;
    [SerializeField] private TMP_Text _correctCountText;
    [SerializeField] private TMP_Text _wrongCountText;
    [SerializeField] private GameObject _wrong;
    [SerializeField] private Timer _timer;
    public string BasePath = "Suudoku/";
    private bool[][] _isLock2dArray = new bool[9][];
    private string[][] _answer2dArray = new string[9][];
    private int _wrongCount = 0;
    private int _correctCount = 0;
    private DataTable _outputTable;
    [SerializeField] private string[] _outputFileName;

    public int SelectedNumberX;
    public int SelectedNumberY;
    [SerializeField] private int[] _problemIndexes;
    private int _problemIndex = 0;
    public string[] ProblemPaths = new[] { "test" };
    public string[] AnswerPaths = new[] { "answer" };

    [System.Serializable]
    public class TMPTextList
    {
        [SerializeField] public TMP_Text[] NumberTexts;
    }

    private void Awake()
    {
        _selectedCursor.gameObject.SetActive(false);
        _problemIndex = 0;
        var index = _problemIndexes[_problemIndex];
        Debug.Log($"Answer Path:{AnswerPaths[index]}");
        LoadAnswer(LoadFile.Load($"{BasePath}{AnswerPaths[index]}"));
        Debug.Log($"Problem Path:{ProblemPaths[index]}");
        LoadProblem(LoadFile.Load($"{BasePath}{ProblemPaths[index]}"));
        _outputTable = CSVOutput.SetUpDataTable(OutputTableLabels);
        StartCountAsync().Forget();
    }

    ///<summary>タイトルが非アクティブになるまでタイムカウントを待つ</sammary>
    private async UniTask StartCountAsync()
    {
        await UniTask.WaitUntil(() => !_titleView.activeSelf);
        _timer.StartCount();
        _selectedCursor.gameObject.SetActive(true);
    }

    ///<summary>問題の読み込み</sammary>
    public void LoadProblem(string[][] numbers)
    {
        for (var y = 0; y < 9; y++)
        {
            _isLock2dArray[y] = new bool[9];
            for (var x = 0; x < 9; x++)
            {
                // 文字色を黒に戻す
                _numberTextsRoot[y].NumberTexts[x].color = new Color(0, 0, 0, 255);
                // Debug.Log(numbers[y][x]);
                // NOTE:改行コードがCRLFの時に空白が含まれてしまう問題への対応
                if (numbers[y][x][0] == '0')
                {
                    _isLock2dArray[y][x] = false;
                    _numberTextsRoot[y].NumberTexts[x].text = "";
                }
                else
                {
                    // Debug.Log($"problem is not 0 : {numbers[y][x]}");
                    // 正解チェック
                    if (_answer2dArray[y][x] != numbers[y][x]) throw new System.Exception($"位置({x},{y})の解答({_answer2dArray[y][x]})と問題({numbers[y][x]})が一致していません。");
                    // Debug.Log($"位置({x},{y})の解答({_answer2dArray[y][x]})と問題({numbers[y][x]})が一致している");
                    _numberTextsRoot[y].NumberTexts[x].text = numbers[y][x];
                    _isLock2dArray[y][x] = true;
                }   
            }
        }
    }

    ///<summary>解答の読み込み</sammary>
    public void LoadAnswer(string[][] numbers) => _answer2dArray = numbers;

    public void SetText(string text)
    {
        var isValidText = false;
        for (var i = 0; i < 9; i++) if (text == (i + 1).ToString()) isValidText = true;
        if (!isValidText)
        {
            Debug.Log("Wrong Text");
            return;
        }
        if (_isLock2dArray[SelectedNumberY][SelectedNumberX]) return;
        _numberTextsRoot[SelectedNumberY].NumberTexts[SelectedNumberX].text = text;

        var isCorrect = _answer2dArray[SelectedNumberY][SelectedNumberX] == text;
        if (isCorrect)
        {
            _numberTextsRoot[SelectedNumberY].NumberTexts[SelectedNumberX].color = new Color(0, 255, 0, 255);
            _correctCount++;
            // Debug.Log($"correct count:{_correctCount}");
            // 正解したら変えられなくする
            _isLock2dArray[SelectedNumberY][SelectedNumberX] = true;
        }
        else
        {
            _numberTextsRoot[SelectedNumberY].NumberTexts[SelectedNumberX].color = new Color(255, 0, 0, 255);
            _wrongCount++;
            Debug.Log($"{_answer2dArray[SelectedNumberX][SelectedNumberY]},{text}");
            // Debug.Log($"wrong count:{_wrongCount}");
            ShowWrongAnswer().Forget();
        }

        // ファイル出力
        // 開始から何秒経ったか
        var time = (_timer.StartMinute - _timer.GetMinute()) * 60 + (_timer.StartSeconds - _timer.GetSeconds());
        var newData = new[] { DateTime.Now.ToString(), time.ToString(), SelectedNumberX.ToString(), SelectedNumberY.ToString(), text, isCorrect ? "correct" : "wrong" };
        CSVOutput.AddDataToDataTable(_outputTable, newData);
    }

    public void DeleteText()
    {
        if (_isLock2dArray[SelectedNumberY][SelectedNumberX]) return;
        _numberTextsRoot[SelectedNumberY].NumberTexts[SelectedNumberX].text = "";
    }

    public void Update()
    {
        // カーソル移動
        _selectedCursor.transform.position = _numberTextsRoot[SelectedNumberY].NumberTexts[SelectedNumberX].transform.position;
        // 正解数、不正解数表示更新
        _correctCountText.text = _correctCount.ToString();
        _wrongCountText.text = _wrongCount.ToString();
        // 時間切れの際
        if (_timer.IsTimeUp() && !_timer.IsStopCount())
        {
            _timer.StopCount();
            // 完全終了
            if (_problemIndexes.Length <= _problemIndex)
            {
                Debug.Log("Finish!!");
                return;
            }
            CSVOutput.SaveDataTable(_outputTable, _outputFileName[_problemIndex]);
            _problemIndex++;
            // var index = _problemIndexes[_problemIndex];
            RestartGame(_problemIndex).Forget();
        }
    }

    ///<summary>ゲームをリセットしスタート</sammary>
    private async UniTask RestartGame(int index)
    {
        _outputTable = CSVOutput.SetUpDataTable(OutputTableLabels);
        await UniTask.Delay(0);
        _correctCount = 0;
        _wrongCount = 0;
        _correctCountText.text = "0";
        _wrongCountText.text = "0";
        LoadAnswer(LoadFile.Load(AnswerPaths[index]));
        LoadProblem(LoadFile.Load(ProblemPaths[index]));
        _timer.SetTime(10, 0);
        _timer.StartCount();
    }

    public async UniTask ShowWrongAnswer()
    {
        _wrong.SetActive(true);
        await UniTask.Delay(3000);
        _wrong.SetActive(false);
    }

    public void UpdateXIndex(int x) => SelectedNumberX = x;
    public void UpdateYIndex(int y) => SelectedNumberY = y;
}

