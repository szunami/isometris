using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreReporter : MonoBehaviour
{
    [SerializeField]
    private Score score;


    [SerializeField]
    TextMeshProUGUI text;

    void Update()
    {
        text.SetText($"{score.LinesCleared()} lines cleared");
    }
}
