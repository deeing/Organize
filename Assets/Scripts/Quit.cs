using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        // Quit the editor play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit the standalone application
        Application.Quit();
#endif
    }
}
