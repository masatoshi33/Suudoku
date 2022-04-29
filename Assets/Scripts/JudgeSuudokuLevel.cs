using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 指定したファイルを一気に読み込んで、数独の問題の難易度判定を行う
/// </summary>
public class JudgeSuudokuLevel : MonoBehaviour
{
    [SerializeField] private string[] _problemPaths;
    [SerializeField] private string _outputFileName;
    private int[][] _suudokuProblem = new int[9][];

    private void Awake()
    {
        OutputLevel();
    }
    private List<double> OutputLevel()
    {
        var output = new List<double>();
        foreach (var path in _problemPaths)
        {
            var problemNumbersText = LoadFile.Load(path);
            var problemNumbers = new int[9][];
            for (var i = 0; i < problemNumbersText.Length; i++)
            {
                problemNumbers[i] = new int[9];
                for (var j = 0; j < problemNumbersText[0].Length; j++)
                {
                    problemNumbers[i][j] = Int32.Parse(problemNumbersText[i][j]);
                }
            }
            var candidate2dArray = new int[9][]; // 2dにする必要ないかも
            // それぞれのマスに対して
            for (var y = 0; y < 9; y++)
            {
                candidate2dArray[y] = new int[9];
                for (var x = 0; x < 9; x++)
                {
                    // すでに数字が入っていればスルー
                    if (problemNumbers[y][x] != 0) continue;
                    // 入力不可能な数字のリスト(keyには9進数を用いる)
                    var unavailableList = new HashSet<int>();
                    // 区画を判定
                    var blockIndexX = x / 3;
                    var blockIndexY = y / 3;
                    // 同一区画のものを追加
                    for (int i = 0; i < 3; i++)
                    {
                        for (var j = 0; j < 3; j++)
                        {
                            var num = problemNumbers[i + blockIndexY * 3][j + blockIndexX * 3];
                            if (num == 0) continue;
                            if (x == j + blockIndexX && y == i + blockIndexY) continue;
                            unavailableList.Add(num);
                        }
                    }
                    // 同一x座標のものを追加
                    for (int i = 0; i < 9; i++)
                    {
                        var num = problemNumbers[i][x];
                        if (num == 0 || i == y) continue;
                        unavailableList.Add(num);
                    }
                    // 同一y座標のものを追加
                    for (int i = 0; i < 9; i++)
                    {
                        var num = problemNumbers[y][i];
                        if (num == 0 || i == x) continue;
                        unavailableList.Add(num);
                    }
                    // 入力可能な数字の候補数
                    candidate2dArray[y][x] = 9 - unavailableList.Count;
                }
            }
            var entropy = 1d;
            for (var y = 0; y < 9; y++)
            {
                for (var x = 0; x < 9; x++)
                {
                    if (problemNumbers[y][x] != 0) continue;
                    if (candidate2dArray[y][x] == 0) Debug.Log($"0 is x:{x},y:{y}");
                    entropy *= candidate2dArray[y][x];
                }
            }
            entropy = Math.Log(entropy) / 81;
            Debug.Log($"FilePath:{path},entropy:{entropy}");
            output.Add(entropy);
        }
        OutputCSV(output);
        return output;
    }

    private void OutputCSV(List<double> list)
    {
        var array = list.ToArray();
        var outputTable = CSVOutput.SetUpDataTable(new string[] { "FilePath", "entropy" });
        for (var i = 0; i < array.Length; i++)
        {
            var newData = new[] { _problemPaths[i], array[i].ToString() };
            CSVOutput.AddDataToDataTable(outputTable, newData);
        }
        CSVOutput.SaveDataTable(outputTable, _outputFileName);
    }
}