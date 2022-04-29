using UnityEngine;
using System;


///<summary>
/// ファイル読み込みを行う関数を定義するクラス
///</summary>
public static class LoadFile
{
    ///<summary>
    /// 第一引数で指定したファイル名のテキストファイルを読み込み、第二引数で指定した区切り位置、及び改行コードで分けた
    /// 配列を返す(csvファイルの場合は、区切り文字は',')
    ///</summary>
    ///<param name="fileName">読み込むテキストファイル名</param>
    ///<param name="splitMarker">文字列の区切り文字(デフォルトは空白' ')</param>
    public static string[][] Load(string fileName, char splitMarker = ' ')
    {
        // Debug.Log(fileName);
        var textasset = new TextAsset(); //テキストファイルのデータを取得するインスタンスを作成
        textasset = Resources.Load(fileName, typeof(TextAsset)) as TextAsset; //Resourcesフォルダから対象テキストを取得
        var textLines = textasset.text; //テキスト全体をstring型で入れる変数を用意して入れる

        //Splitで一行づつを代入した1次配列を作成
        var textMessage = textLines.Split('\n'); //

        //行数と列数を取得
        var columnLength = textMessage[0].Split(splitMarker).Length;
        var rowLength = textMessage.Length;

        //2次配列を定義
        var textWords = new string[rowLength][];

        for (int i = 0; i < rowLength; i++)
        {
            var tempWords = textMessage[i].Split(splitMarker); //textMessageをカンマごとに分けたものを一時的にtempWordsに代入
            textWords[i] = new string[columnLength];
            for (int j = 0; j < columnLength; j++)
            {
                textWords[i][j] = tempWords[j]; //2次配列textWordsにカンマごとに分けたtempWordsを代入していく
                // Debug.Log(textWords[i][j]);
            }
        }
        return textWords;
    }

    ///<summary>
    /// 改行のないテキストファイルを読み込む関数。第一引数で指定したファイル名のテキストファイルを読み込み、
    /// 第二引数で指定した区切り位置、及び改行コードで分けた配列を返す
    ///</summary>
    ///<param name="fileName">読み込むテキストファイル名</param>
    ///<param name="splitMarker">文字列の区切り文字(デフォルトは空白' ')</param>
    public static string[] LoadSimpleText(string fileName, char splitMarker = ' ')
    {
        // Debug.Log(fileName);
        var textasset = new TextAsset(); //テキストファイルのデータを取得するインスタンスを作成
        textasset = Resources.Load(fileName, typeof(TextAsset)) as TextAsset; //Resourcesフォルダから対象テキストを取得
        var text = textasset.text; //テキスト全体をstring型で入れる変数を用意して入れる

        // テキストを区切り位置で分けた配列を取得
        Debug.Log(text);
        var textWords = text.Split(splitMarker);
        // Debug.Log($"input text:{textWords}");
        return textWords;
    }
}