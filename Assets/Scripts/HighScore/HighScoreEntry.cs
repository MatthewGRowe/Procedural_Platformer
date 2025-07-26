using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HighScoreEntry 
{
    //This is the datastructure for the high score entries, each high score has one of these. 
    //See HighScore.cs

    public string name;
    public int score;

    public HighScoreEntry(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}
