using System;
using System.Collections.Generic;
using UnityEngine;

public class LifeController : MonoBehaviour, ISubject
{
    public static event Action OnFullHealth;

    public const int MAX_LIVES = 5, MIN_LIVES = 0;

    private int lives = MIN_LIVES;

    public int Lives
    {
        get
        {
            return lives;
        }
    }

    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private LifeObserver lifesBar;
    private List<IObserver> observers = new List<IObserver>();

    private void Awake()
    {
        Attach(lifesBar);
        Attach(gameController);
        Notify();        
    }

    private void OnEnable()
    {
        LifeTimer.OnTimesUp += AddLife;
        GameController.OnRefillLives += SetLifesToMax;
        GameController.OnTakeLife += TakeLife;
    }

    private void OnDisable()
    {
        LifeTimer.OnTimesUp -= AddLife;
        GameController.OnRefillLives -= SetLifesToMax;
        GameController.OnTakeLife -= TakeLife;
    }

    private void AddLife()
    {
        if (lives < MAX_LIVES)
        {
            ++lives;
            Notify();
        }
        if(lives == MAX_LIVES)
        {
            OnFullHealth?.Invoke();
        }
    }

    private void TakeLife()
    {
        if(lives > MIN_LIVES)
        {
            --lives;
            Notify();
        }
    }

    private void SetLifesToMax()
    {
        lives = MAX_LIVES;
        Notify();
        OnFullHealth?.Invoke();
    }

    public bool IsLivesMoreThanMin()
    {
        return lives > MIN_LIVES;
    }

    public bool IsLivesLessThanMax()
    {
        return lives < MAX_LIVES;
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
        foreach (var observer in new List<IObserver>(observers))
        {
            observer.UpdateData(this);
        }
    }
}
