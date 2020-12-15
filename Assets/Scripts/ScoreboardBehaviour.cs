using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardBehaviour : MonoBehaviour
{
    public int score;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}
