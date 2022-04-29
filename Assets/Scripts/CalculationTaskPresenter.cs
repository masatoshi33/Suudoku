using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

public class CalculationTaskPresenter : MonoBehaviour
{
    private static readonly string[] OutputTableLabels = { "date", "time", "answer", "isCorrect" };
    [SerializeField] private string _problemPath;
    [SerializeField] private string _outputFileName = "calculationTask_result1";
    [SerializeField] CalculationTaskView _view;
    [SerializeField] private GameObject _titleView;
    [SerializeField] Timer _timer;
    private int _answerCount = 1;
    private int[] _problemNumbers;
    private int[][] _questionNumbers2dArray = new int[100][];
    private DataTable _outputTable;
    private CalculationTask[] _calculationTasks = new CalculationTask[79 * 9];
    public int MaxProblemNumber = 200;

    private class CalculationTask
    {
        public int FirstNumber;
        public int SecondNumber;
        public int CorrectAnswer;
        public CalculationTask(int first, int second)
        {
            FirstNumber = first;
            SecondNumber = second;
            CorrectAnswer = first * second;
        }
    }

    private void Awake()
    {
        // 問題を読み込み
        var problemPath = $"CalculationTask/{_problemPath}";
        LoadProblem(problemPath);
        _view.OnClickNextButton.Subscribe(_ =>
        {
            var task = _calculationTasks[_answerCount];
            CheckAnswer(task);
            // 最後の問題の場合はCSVを保存し、修了処理
            if (_answerCount > _view.GetMaxProblemNumber())
            {
                FinishTask();
            }
            // 問題番号を更新し、最後の問題でなければ次の問題をセット
            else
            {
                _answerCount++;
                SetNextProblem(_answerCount);
            }
        });
        // 問題をセットし、カウントスタート
        SetNextProblem(_answerCount);
        StartCountAsync().Forget();
        // 出力用CSV
        _outputTable = CSVOutput.SetUpDataTable(OutputTableLabels);
    }

    ///<summary>タイトルが非アクティブになるまでタイムカウントを待つ</sammary>
    private async UniTask StartCountAsync()
    {
        await UniTask.WaitUntil(() => !_titleView.activeSelf);
        _view.StartGame();
        _timer.StartCount();
    }
    private void Update()
    {
        // 時間切れの際
        if (_timer.IsTimeUp() && !_timer.IsStopCount())
        {
            _timer.StopCount();
            // 終了処理
            Debug.Log("Finish!!");
            FinishTask();
            CSVOutput.SaveDataTable(_outputTable, $"{_outputFileName}");
        }

        if (Input.GetKeyDown(KeyCode.Return) && _view.GetAnswerText() != "")
        {
            var task = _calculationTasks[_answerCount];
            CheckAnswer(task);
            // 最後の問題までは行かせない想定
            // 最後の問題の場合はCSVを保存し、修了処理
            if (_answerCount > _view.GetMaxProblemNumber())
            {
                FinishTask();
            }
            // 問題番号を更新し、最後の問題でなければ次の問題をセット
            else
            {
                _answerCount++;
                SetNextProblem(_answerCount);
            }
        }
    }

    ///<summary>問題の読み込み</sammary>
    public void LoadProblem(string path)
    {
        var problemTexts = LoadFile.Load(path);
        for (var i = 0; i < MaxProblemNumber; i++)
        {
            var problem = problemTexts[i];
            var first = int.Parse(problem[0]);
            var second = int.Parse(problem[1]);
            _calculationTasks[i] = new CalculationTask(first, second);
        }
    }

    private void CheckAnswer(CalculationTask task)
    {
        // 解答を取得、送信
        var answer = _view.GetAnswerText();
        var isCorrect = task.CorrectAnswer == int.Parse(answer);
        // ファイル出力
        // 開始から何秒経ったか
        var time = (_timer.StartMinute - _timer.GetMinute()) * 60 + (_timer.StartSeconds - _timer.GetSeconds());
        var newData = new[] { DateTime.Now.ToString(), time.ToString(), answer, isCorrect ? "correct" : "wrong" };
        CSVOutput.AddDataToDataTable(_outputTable, newData);
    }

    /// <summary>次の問題をセット</summary>
    private void SetNextProblem(int index)
    {
        _view.SetIndexNumber(index);
        _view.SetProblemNumbers(new int[] { _calculationTasks[index].FirstNumber, _calculationTasks[index].SecondNumber });
        _view.DeleteAnswerText();
    }

    /// <summary>ゲーム終了時の処理</summary>
    private void FinishTask()
    {
        _view.FinishGame();
        // CSVを保存
        var nowTime = DateTime.Now.ToString().Replace("/", "_");
        CSVOutput.SaveDataTable(_outputTable, $"{nowTime}_{_outputFileName}");
    }
}
