using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using TMPro;
using System;


public class CookieClicker : MonoBehaviour
{
    readonly int ClickMultiplier = 1000;

    public TMP_Text txtTimer;
    float Timer;
    bool TimerOn = false;

    long Cookies;
    //private Text text;
    public TMP_Text txtCookies;

    int TimeBonus;
    float BonusLevel;
    public TMP_Text txtBonusLevel;

    long CookiesForOven;
    public TMP_Text txtCookiesForOven;

    int OvenCost;

    int Ovens = 1;
    public TMP_Text txtOvens;
    readonly string strOvens = " Cookies to buy next Oven for ";

    int Donations = 0;
    public TMP_Text txtDonations;

    public TMP_Text txtMsg;
    readonly string strMsg = "Ovens and Donations boost your Bonus Level.";
    
    public TMP_Text txtClickMe;

    Color White = Color.white;
    Color Red = Color.red;
    Color Cyan = Color.cyan;
    Color Yellow = Color.yellow;
    Color Black = Color.black;

    public AudioSource Bonus;
    public AudioSource BonusExtra;
    public AudioSource Congrats;
    public AudioSource NoNo;

    public TMP_Text txtStart;
    
    long Clicks;
    public TMP_Text txtClicks;

    long TopScore;
    public TMP_Text txtTopScore;

    int TopOvens;
    public TMP_Text txtTopOvens;

    int TopDonations;
    public TMP_Text txtTopDonations;


    void UpdateScreen()
    {
        txtTimer.text = "Timer : " + Timer.ToString("#0");

        txtTopOvens.text = "Ovens = " + TopOvens.ToString();
        txtTopDonations.text = "Donations = " + TopDonations.ToString();
        txtTopScore.text = "Top Score = " + TopScore.ToString("#,##0");
        
        txtClicks.text = "Clicks = " + Clicks.ToString("#,##0");
        txtCookies.text = "Cookies = " + Cookies.ToString("#,##0");
        txtCookiesForOven.text = CookiesForOven.ToString("#,##0") + strOvens 
            + OvenCost.ToString("#,##0");

        txtBonusLevel.text = "Bonus Level : " + BonusLevel.ToString("#,##0.00");
        txtMsg.text = strMsg;

        txtOvens.text = "Ovens = " + Ovens.ToString("#,###0");
        txtDonations.text = "Donations = " + Donations.ToString("#0");
    }

    void Start()
    {
        ReadData();
        //if (TopScore < Cookies) TopScore = Cookies;

        Timer = 60f;
        Clicks = 0;
        
        Cookies = 0;
        CookiesForOven = 100 * ClickMultiplier;
        OvenCost = 30 * ClickMultiplier;

        TimeBonus = 1;
        Ovens = 1;
        Donations = 0;
        BonusLevel = 1f;

        txtCookiesForOven.color = Yellow;
        txtMsg.color = Yellow;

        UpdateScreen();
    }

    void Update()
    {

        if (TimerOn == false)
        {
            txtStart.color = Lerp(White, Red, 8);   //Start Button enabled
            txtClickMe.color = White;               //Cookie Button disabled
            return;
        }

        txtStart.color = Black;                     //Start Button disabled
        txtClickMe.color = Lerp(White, Red, 8);     //Cookie Button enabled
        txtCookiesForOven.color = Lerp(White, Yellow, 10);

        Timer -= Time.deltaTime;                    //Timer enabled
        if (Timer <= 0f)                            //Timer disabled 
        {
            Timer = 0f;
            TimerOn = false;
            SaveData();
        }
        
    }

    Color Lerp(Color Color1, Color Color2, float speed)
    {
        return Color.Lerp(Color1, Color2, Mathf.Sin(Time.time * speed));
    }


    public void StartButtonClick()
    {
        //if (TimerOn) return;    //Don't re-start in the middle
        
        TimerOn = true;
        Start();
    }


    public void MakeCookie()
    {
        if (TimerOn == false) return;               // Start button needs to be clicked.
        
        if ((Cookies < CookiesForOven) && (Cookies + BonusLevel*ClickMultiplier >= CookiesForOven))
        {
            //Can buy next oven sound+colour alerts
            BonusExtra.Play();
            txtCookies.color = Cyan;
        }
        else
        {
            txtCookies.color = White;
        };

        Cookies += Convert.ToInt64(BonusLevel * ClickMultiplier);
        Clicks += 1;
        if (TopScore < Cookies) TopScore = Cookies;

        txtMsg.text = strMsg;
        txtMsg.color = Yellow;
        txtOvens.color = White;

        SetBonusLevel();
    }


    private void SetBonusLevel()
    {
        float OldBonusLevel = BonusLevel;
        float TimeElapsed = 60f - Timer;

        if (TimeElapsed <= 10f) TimeBonus = 1; else TimeBonus = 2;

        BonusLevel = (float)TimeBonus + (float)Ovens-1f + (float)Donations*0.3f;

        if (BonusLevel > OldBonusLevel)
        {
            Bonus.Play();
            txtBonusLevel.color = Cyan;
        }
        else
        {
            txtBonusLevel.color = White;
        }
        UpdateScreen();
    }


    public void BuyOven() 
    {
        if (TimerOn == false) return;

        if (Cookies < CookiesForOven)
        {
            NoNo.Play();    
            txtMsg.text = "Sorry, You need more cookies to buy the next oven.";
            txtMsg.color = Red;
            txtOvens.color = White;
        }
        else
        {
            Congrats.Play();
            Cookies -= OvenCost;
            
            Ovens ++;
            txtOvens.color = Cyan;

            txtMsg.text = "Congratulations! You have another oven.";
            txtMsg.color = Cyan;

            // Make the next oven 10% dearer
            OvenCost = Convert.ToInt32(OvenCost * 1.1);

            //Reset Cookies For Oven
            if (Ovens >= 2) CookiesForOven = (Ovens-1) * 400 * ClickMultiplier;

            SetBonusLevel();
        }
    }

    public void Donate()
    {
        if (TimerOn == false) return;

        if (Cookies < 50*ClickMultiplier)
        {
            NoNo.Play();
            txtMsg.text = "Sorry, You need more cookies to donate.";
            txtMsg.color = Red;
            txtDonations.color = White;
        }
        else
        {
            Congrats.Play();
            //Debug.Log("Before Donation - " + Cookies.ToString("#,##0"));
            Cookies -= Convert.ToInt64(Cookies * 0.02f);
            //Debug.Log(" After Donation - " + Cookies.ToString("#,##0"));

            Donations++;
            txtDonations.color = Cyan;

            txtMsg.text = "Congratulations! Your community thanks you!";
            txtMsg.color = Cyan;

            SetBonusLevel();
            //Debug.Log("   Bonus Level - " + BonusLevel.ToString("#,##0.00")
            //    + " = " + TimeBonus.ToString("#,##0")
            //    + " ; " + Ovens.ToString("#,##0")
            //    + " ; " + Donations.ToString())
            //    ;
        }
    }


    public void Exit() 
    {
        SaveData();
        Application.Quit();
    }


    void SaveData() 
    {
        if (TopScore > Cookies) return;

        PlayerPrefs.SetInt("TopScore", (int)TopScore);
        PlayerPrefs.SetInt("TopOvens", TopOvens);
        PlayerPrefs.SetInt("TopDonations", TopDonations);

        

        //PlayerPrefs.Save();
    }


    void ReadData() 
    {
        /*
        PlayerPrefs.SetInt("TopScore", 2419990);
        PlayerPrefs.SetInt("TopOvens", 7);
        PlayerPrefs.SetInt("TopDonations", 36);
        */

        TopScore = PlayerPrefs.GetInt("TopScore",0);
        TopOvens = PlayerPrefs.GetInt("TopOvens", 0);
        TopDonations = PlayerPrefs.GetInt("TopDonations", 0);

        Debug.Log("ReadData");
        Debug.Log(TopScore.ToString());
        Debug.Log(TopOvens.ToString());
        Debug.Log(TopDonations.ToString());
    }
}
