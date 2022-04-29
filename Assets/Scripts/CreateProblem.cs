using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CreateProblem : MonoBehaviour
{
    [SerializeField] private int _maxFirstNumber = 49;
    [SerializeField] private int _minFirstNumber = 21;
    [SerializeField] private int[] _secondNumbers = new int[9] { 11, 12, 13, 14, 15, 16, 17, 18, 19 };
    [SerializeField] private string _outputPath = "output.txt";
    private int[] _firstNumbers = new int[29];
    private List<int> _problemIndexes = new List<int>();
    private void Awake()
    {
        CreateNumbersArray();
        ArrangeNumbers();
        // 出力
        using (var writer = new StreamWriter(_outputPath, true))
        {
            for (var i = 0; i < 27 * 9; i++)
            {
                var index = _problemIndexes[i];
                var firstIndex = index % 27;
                var secondIndex = index / 27;
                writer.WriteLine($"{_firstNumbers[firstIndex]} {_secondNumbers[secondIndex]}");
            }
        }
    }

    /// <summary>問題に使う数字のリストを作成</summary>
    private void CreateNumbersArray()
    {
        // 数字リスト作成
        var numbers = new List<int>();
        for (var i = _minFirstNumber; i <= _maxFirstNumber; i++)
        {
            // 10の倍数は簡単なので排除
            if (i % 10 == 0) continue;
            numbers.Add(i);
        }
        var index = 0;
        while (index < 27)
        {
            var randomIndex = UnityEngine.Random.Range(0, numbers.Count);
            _firstNumbers[index] = numbers[randomIndex];
            index++;
            Debug.Log(numbers[randomIndex]);
            numbers.RemoveAt(randomIndex);
        }
    }

    /// <summary>問題番号を並び替える</summary>
    private void ArrangeNumbers()
    {
        var numbers = new List<int>();
        for (var i = 0; i < 27 * 9; i++)
        {
            numbers.Add(i);
        }
        while (numbers.Count > 0)
        {
            var index = UnityEngine.Random.Range(0, numbers.Count);
            _problemIndexes.Add(numbers[index]);
            Debug.Log(numbers[index]);
            numbers.RemoveAt(index);
        }
    }
}
