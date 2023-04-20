using UnityEngine;
using UnityEngine.UI;

public class Content : MonoBehaviour
{
    private enum ContentStatus
    {
        FULL = 1,
        NOT_FULL = 2,
        NO_LIVES = 3
    }

    [SerializeField]
    private ContentStatus contentStatus;
    [SerializeField]
    private LifeObserver lifeObserver;
    [SerializeField]
    private TimerObserver timerObserver;
    public Button useLife, refillLives; 

    public void InitContent(LifeController lifeController, LifeTimer timer)
    {
        gameObject.SetActive(true);
        switch (contentStatus)
        {
            case ContentStatus.NOT_FULL:
            case ContentStatus.NO_LIVES:
                timer.Attach(timerObserver);
                break;
        }
        lifeController.Attach(lifeObserver);
        lifeController.Notify();
    }

    public void DeinitContent(LifeController lifeController, LifeTimer timer)
    {
        switch (contentStatus)
        {
            case ContentStatus.NOT_FULL:
            case ContentStatus.NO_LIVES:
                lifeController.Detach(lifeObserver);
                timer.Detach(timerObserver);
                break;
        }
        lifeController.Detach(lifeObserver);
        useLife?.onClick.RemoveAllListeners();
        refillLives?.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }
}
