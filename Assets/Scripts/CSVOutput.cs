using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;       // DateTimeを使うために必要
using System.IO;    // CSV保存をするために必要
using System.Data;
using UnityEngine.Assertions;

/// <summary>string配列をcsvファイルとして出力する。SetUpDataTableでラベルを設定してから、
/// AddDataでどんどん追加していき、最後にSaveする。
/// </summary>
public static class CSVOutput
{
    private enum DataType
    {
        String = 1,
        Int = 2,
        Float = 3,
    };

    // stringの二次元配列をDataTableに変換するための関数
    private static DataTable ToDataTable(string[] titles, int dataTypes, string[][] data)
    {
        // 各列のタイトルを設定
        var table = new DataTable();
        foreach (var title in titles) table.Columns.Add(title, typeof(String));

        // 行
        for (var i = 0; i < data.Length; i++)
        {
            var newRow = table.NewRow();
            // 列
            for (var j = 0; j < titles.Length; j++)
            {
                // データ型で分類
                switch (dataTypes)
                {
                    case (int)DataType.String:
                        newRow[titles[j]] = data[i][j];
                        break;
                    case (int)DataType.Int:
                        newRow[titles[j]] = data[i][j];
                        break;
                    case (int)DataType.Float:
                        newRow[titles[j]] = data[i][j];
                        break;
                }
            }
            table.Rows.Add(newRow);
        }
        return table;
    }

    ///<summary>ラベルからDataTableを作成</summary>
    ///<param="labels">列名</param>
    public static DataTable SetUpDataTable(string[] labels)
    {
        var table = new DataTable();
        foreach (var label in labels) table.Columns.Add(label, typeof(String));
        return table;
    }

    public static DataTable AddDataToDataTable(DataTable table, string[] data)
    {
        // Debug.Log($"table:{table.Columns.Count}");
        // Debug.Log($"data:{data.Length}");
        Assert.IsTrue(table.Columns.Count == data.Length, "データの数がラベル数と一致しません");
        var newRow = table.NewRow();
        for (var i = 0; i < data.Length; i++) newRow[i] = data[i];
        table.Rows.Add(newRow);
        return table;
    }
    public static void SaveDataTable(DataTable table, string fileName)
    {
        var filePath = $"{Application.dataPath}/Outputs/{fileName}.csv";
        var writer = new StreamWriter(filePath, false, System.Text.Encoding.GetEncoding("shift_jis"));
        // 最上部のラベルを出力
        foreach (var column in table.Columns)
        {
            var colIndex = 0;
            writer.Write(column);
            // Debug.Log(column);
            if (colIndex >= table.Columns.Count - 1) continue;
            colIndex++;
            writer.Write(", ");
        }
        writer.Write("\r\n");
        // 各行を出力
        for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
        {
            for (var colIndex = 0; colIndex < table.Columns.Count; colIndex++)
            {
                writer.Write(table.Rows[rowIndex][colIndex]);
                // Debug.Log(table.Rows[rowIndex][colIndex]);
                if (colIndex >= table.Columns.Count - 1) continue;
                writer.Write(", ");
            }
            writer.Write("\r\n");
        }
        writer.Close();
    }
}
