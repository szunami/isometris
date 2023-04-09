using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Score : ScriptableObject
{
    [SerializeField]
    int linesCleared;

    void OnEnable() {
        linesCleared = 0;
    }

    public void ClearLine() {
        linesCleared++;
    }

    public int LinesCleared() {
        return linesCleared;
    }
}
