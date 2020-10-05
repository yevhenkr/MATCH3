using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText;

    public void ResetCount()
    {
        scoreText.text = "0";
    }

    public void AddPoint(int i)
    {
        int n = 0;
        if (scoreText.text != "")
        {
            n = int.Parse(scoreText.text);
        }

        scoreText.text = (n += i).ToString();
    }
}