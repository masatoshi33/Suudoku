using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SuudokuButtonAction : MonoBehaviour
{
    [SerializeField] private TMP_Text _number;
    public void OnClick()
    {
        Debug.Log(_number.text);
    }
}
