using TMPro;
using UnityEngine;

public class LifeObserver : MonoBehaviour, IObserver
{
    [SerializeField]
    private TextMeshProUGUI lifeText;

    public void UpdateData(ISubject subject)
    {
        var lifeController = (subject as LifeController);
        lifeText.text = lifeController.Lives.ToString();
    }
}
