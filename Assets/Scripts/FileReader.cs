using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FileReader : MonoBehaviour
{
    [SerializeField]
    private string fileName = "data.txt";

    public string fileContents { get; private set; }

    public void Read(Action onReadFinish)
    {
        string filePath = Path.Combine(Application.dataPath, "..", fileName);

        if (File.Exists(filePath))
        {
            fileContents = File.ReadAllText(filePath);
            Debug.Log("File contents: " + fileContents);
            onReadFinish.Invoke();
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }
}
