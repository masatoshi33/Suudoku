using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class TextBox : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Button _button;
    private int _indexX;
    private int _indexY;
    public IObservable<Unit> OnClickButton => _button.onClick.AsObservable();
    public void SetText(string text) => _text.text = text;
    public void SetColor(Color color) => _text.color = color;
    public void AddText(string text)
    {
        if (_text.text == "" && text == "0") return;
        _text.text += text;
    }
    public string GetText() => _text.text;
    public void SetIndex(int x, int y)
    {
        _indexX = x;
        _indexY = y;
    }
    public int[] GetIndex() => new[] { _indexX, _indexY };
}
