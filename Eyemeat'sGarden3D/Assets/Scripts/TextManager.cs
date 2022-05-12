using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour {
    public Text clock;
    public Text money;
    public Text difficulty;
    public Text health;
    private float timeSoFar = 0;
    public World myWorld;

    private bool dif = true;

    void Start () {
		
	}
	
	void Update () {
        System.DateTime time = System.DateTime.Now;
        timeSoFar += Time.deltaTime;

        updateClock(timeSoFar);
    }

    public void updateClock(float t)
    {
        int total = (int)t;

        if (total % 60 == 0 &&  dif)
        {
            myWorld.increaseDifficulty(1);
            dif = false;
        }else if(total % 60 != 0 && !dif)
        {
            dif = true;
        }

        int seconds = (int)t % 60;
        int minutes = (int)t / 60 % 60;
        int hours = minutes / 60;

        string hoursS = ""+hours;
        string minutesS = ""+minutes;
        string secondsS = ""+seconds;

        if(hours < 10)
        {
            hoursS = "0" + hoursS;
        }
        if(minutes < 10)
        {
            minutesS = "0" + minutesS;
        }
        if(seconds < 10)
        {
            secondsS = "0" + secondsS;
        }

        clock.text = "["+hoursS+":"+minutesS+":"+secondsS+"]";
    }

    public void updateMoney(int amount)
    {
        money.text = "Money: "+ amount;
    }

    public void updateDifficulty(int d)
    {
        difficulty.text = "Difficulty: " + d;
    }

    public void updateHealth(int h)
    {
        health.text = "Health: " + h;
    }

    public float getTimeSoFar()
    {
        return timeSoFar;
    }
}

