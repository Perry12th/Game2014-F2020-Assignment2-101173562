using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Score;
    ScoreboardBehaviour scoreboardBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        scoreboardBehaviour = FindObjectOfType<ScoreboardBehaviour>();
        Score.SetText("Score: " + scoreboardBehaviour.score);
    }


    void OnDestroy()
    {
        Destroy(scoreboardBehaviour);
    }

}
