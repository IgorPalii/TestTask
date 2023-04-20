using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyBonusController : MonoBehaviour
{
    private const int FIRST_DAY_REWARD = 2, SECOND_DAY_REWARD = 3;
    [SerializeField]
    private TextMeshProUGUI rewardText;
    [SerializeField]
    private Button claimButton;

    void Start()
    {
        int dayInMonth = DateTime.UtcNow.Day;
        int month = DateTime.UtcNow.Month;
        int year = DateTime.UtcNow.Year;

        claimButton.onClick.AddListener(ClaimReward);
        try
        {
            rewardText.text = rewardText.text + CalculateReward(DayInSeason(dayInMonth, month, year)).ToString();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            gameObject.SetActive(false);
        }
    }

    private double CalculateReward(int dayInSeason)
    {
        if(dayInSeason < 1)
        {
            throw new ArgumentException("Day must not be negative or zero!");
        }

        if (dayInSeason == 1)
        {
            return FIRST_DAY_REWARD;
        }
        else if (dayInSeason == 2)
        {
            return SECOND_DAY_REWARD;
        }

        ulong result = 0;
        ulong predidushiy = SECOND_DAY_REWARD, predpredidushiy = FIRST_DAY_REWARD;

        for (int i = 3; i <= dayInSeason; i++)
        {
            result = (ulong)(Math.Round(predidushiy * 0.6)) + predpredidushiy;
            predpredidushiy = predidushiy;
            predidushiy = result;            
        }

        return result;
    }

    private void ClaimReward()
    {
        gameObject.SetActive(false);
    }

    private int DayInSeason(int dayInMonth, int month, int year)
    {        
        int monthInSeason = (month % 3) + 1;
        int daysInPrevMonthes = 0;
        int prevMonth;

        for (int i = 1; i < monthInSeason; i++)
        {
            prevMonth = (month - i) == 0 ? 12 : month - i;
            daysInPrevMonthes += DateTime.DaysInMonth(year, prevMonth);
        }

        return dayInMonth + daysInPrevMonthes;
    }
}
