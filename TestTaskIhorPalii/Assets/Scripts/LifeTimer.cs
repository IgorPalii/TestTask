using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LifeTimer : MonoBehaviour, ISubject
{
    public static event Action OnTimesUp;

    public const string TIMER_FORMAT = "{0:00}:{1:00}";
    private const string LIFE_FULL_TEXT = "FULL";   
    private const int MAX_TIMER_SECONDS = 20;

    private int counter = MAX_TIMER_SECONDS;
    
    public float minutes { get; private set; }
    public float seconds { get; private set; }

    public bool isTimerWorking = false;

    private DateTime pauseDateTime;  
    [SerializeField]
    private TextMeshProUGUI mainTimerText;

    private List<IObserver> observers = new List<IObserver>();

    private void Start()
    {
        StartTimer();
    }

    private void OnEnable()
    {
        LifeController.OnFullHealth += StopTimer;
        GameController.OnStartTimer += StartTimer;
    }

    private void OnDisable()
    {
        LifeController.OnFullHealth -= StopTimer;
        GameController.OnStartTimer -= StartTimer;
    }

    private void StartTimer()
    {
       isTimerWorking = true;
       StartCoroutine(UpdateTime());
    }

    private void StopTimer()
    {
        isTimerWorking = false;
        StopAllCoroutines();
        counter = MAX_TIMER_SECONDS;
        mainTimerText.text = LIFE_FULL_TEXT;
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (!isTimerWorking) return;

        if (isPaused)
        {
            BackupTimer();
        }
        else
        {
            RestoreTimer();
        }
    }

    private void RestoreTimer()
    {
        int elapsedSeconds = (int)(DateTime.Now - pauseDateTime).TotalSeconds;
        counter -= elapsedSeconds;

        if (counter < 0)
        {
            int elapsedTimerLoopsNumber = Mathf.Clamp(counter / MAX_TIMER_SECONDS, 1, LifeController.MAX_LIVES);

            for (int i = 0; i < elapsedTimerLoopsNumber; i++)
            {
                counter += MAX_TIMER_SECONDS; //rewind timer for one loop
                OnTimesUp?.Invoke();
            }
        }
    }

    private void BackupTimer()
    {
        pauseDateTime = DateTime.Now;
    }

    private IEnumerator UpdateTime()
    {
        while (isTimerWorking)
        {
            minutes = Mathf.FloorToInt(counter / 60);
            seconds = Mathf.FloorToInt(counter % 60);
            mainTimerText.text = string.Format(TIMER_FORMAT, minutes, seconds);
            Notify();
            yield return new WaitForSecondsRealtime(1);
            counter--;

            if (counter < 0)
            {
                OnTimesUp?.Invoke();
                counter = MAX_TIMER_SECONDS;
            }
        }
    }

    public void Attach(IObserver observer)
    {
        observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void Notify()
    {
        foreach (var observer in observers)
        {
            observer.UpdateData(this);
        }
    }
}
