using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public Text scoreboard;
    private string myName;

    private string highScoreName;
    private int highScoreNum;
    // Use this for initialization
    void Awake () {
        Cursor.visible = true;
        setUpScoreboard();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void startGame()
    {
        if(myName != null)
            PlayerPrefs.SetString("lastName", myName);
        SceneManager.LoadScene(sceneName: "MainGame");
    }

    public void setMyName(string name)
    {
        myName = name;
    }

    public void setUpScoreboard()
    {
        string score = "Highscore\n";

        highScoreName = PlayerPrefs.GetString("topName");
        highScoreNum = PlayerPrefs.GetInt("topNum");

        if(PlayerPrefs.GetString("lastName") != null && PlayerPrefs.GetInt("lastNum") > highScoreNum)
        {
            highScoreName = PlayerPrefs.GetString("lastName");
            highScoreNum = PlayerPrefs.GetInt("lastNum");

            PlayerPrefs.SetString("topName", highScoreName);
            PlayerPrefs.SetInt("topScore", highScoreNum);
        }

        score += highScoreName + " : " + highScoreNum;

        scoreboard.text = score;
    }
}
