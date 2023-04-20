using TMPro;
using UnityEngine;

public class TimerObserver : MonoBehaviour, IObserver
{
    [SerializeField]
    private TextMeshProUGUI timerText;

    public void UpdateData(ISubject subject)
    {
        var timer = (subject as LifeTimer);
        timerText.text = string.Format(LifeTimer.TIMER_FORMAT, timer.minutes, timer.seconds);
    }
}
