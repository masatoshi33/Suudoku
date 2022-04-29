using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class SquareGridTexts : MonoBehaviour
{
    [SerializeField] private int _maxIndexX = 10;
    [SerializeField] private int _maxIndexY = 10;
    [SerializeField] private RectTransform _selectedCursor;
    [SerializeField] private ReactiveProperty<int> _selectedIndexX = new ReactiveProperty<int>(0);
    [SerializeField] private ReactiveProperty<int> _selectedIndexY = new ReactiveProperty<int>(0);

    [SerializeField] private TextBoxList[] _numberTextsRoot;

    [System.Serializable]
    public class TextBoxList
    {
        [SerializeField] public TextBox[] NumberTexts;
    }

    public void SetText(int x, int y, string text) => _numberTextsRoot[y].NumberTexts[x].SetText(text);
    public void SetColor(int x, int y, Color color) => _numberTextsRoot[y].NumberTexts[x].SetColor(color);
    public void AddText(int x, int y, string text) => _numberTextsRoot[y].NumberTexts[x].AddText(text);
    public void SetText(string text) => SetText(_selectedIndexX.Value, _selectedIndexY.Value, text);
    public void SetColor(Color color) => SetColor(_selectedIndexX.Value, _selectedIndexY.Value, color);
    public void AddText(string text) => AddText(_selectedIndexX.Value, _selectedIndexY.Value, text);
    public void DeleteText() => SetText(_selectedIndexX.Value, _selectedIndexY.Value, "");
    public int[] GetMaxIndexes => new[] { _maxIndexX, _maxIndexY };
    public int[] GetSelectedIndexes => new[] { _selectedIndexX.Value, _selectedIndexY.Value };
    public string GetText() => _numberTextsRoot[_selectedIndexY.Value].NumberTexts[_selectedIndexX.Value].GetText();

    private void Awake()
    {
        _selectedIndexX.Subscribe(x =>
        {
            // Debug.Log($"x:{x}");
            _selectedCursor.position = _numberTextsRoot[_selectedIndexY.Value].NumberTexts[x].transform.position;
        }).AddTo(this);
        _selectedIndexY.Subscribe(y =>
        {
            // Debug.Log($"y:{y}");
            _selectedCursor.position = _numberTextsRoot[y].NumberTexts[_selectedIndexX.Value].transform.position;
        }).AddTo(this);
        for (var y = 0; y < _maxIndexY; y++)
        {
            for (var x = 0; x < _maxIndexX; x++)
            {
                var num = _numberTextsRoot[y].NumberTexts[x];
                num.SetIndex(x, y);
                num.OnClickButton.Subscribe(_ =>
                {
                    SetSubscribe(num.GetIndex()[0], num.GetIndex()[1]);
                }
                ).AddTo(this);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_selectedIndexY.Value <= 0) _selectedIndexY.Value = _maxIndexY - 1;
            else _selectedIndexY.Value--;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_selectedIndexY.Value >= _maxIndexY - 1) _selectedIndexY.Value = 0;
            else _selectedIndexY.Value++;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_selectedIndexX.Value >= _maxIndexX - 1) _selectedIndexX.Value = 0;
            else _selectedIndexX.Value++;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_selectedIndexX.Value <= 0) _selectedIndexX.Value = _maxIndexX - 1;
            else _selectedIndexX.Value--;
        }
        // エンターキー押下時の処理
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (_selectedIndexY.Value >= _maxIndexY - 1)
            {
                // 一番右下からだと左上に戻る
                if (_selectedIndexX.Value >= _maxIndexX - 1) _selectedIndexX.Value = 0;
                // 一番下まで行くと、右隣の一番上に移動
                else _selectedIndexX.Value++;
                _selectedIndexY.Value = 0;
            }
            else _selectedIndexY.Value++;
        }
        // Tabキー押下時の処理
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_selectedIndexX.Value >= _maxIndexX - 1)
            {
                // 一番右下からだと左上に戻る
                if (_selectedIndexY.Value >= _maxIndexX - 1) _selectedIndexY.Value = 0;
                // 一番下まで行くと、右隣の一番上に移動
                else _selectedIndexY.Value++;
                _selectedIndexX.Value = 0;
            }
            else _selectedIndexX.Value++;
        }
    }

    private void SetSubscribe(int x, int y)
    {
        // Debug.Log($"click x:{x},y:{y}");
        _selectedIndexX.Value = x;
        _selectedIndexY.Value = y;
    }
}
