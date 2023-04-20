using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour, IObserver
{
    private const string IS_ACTIVE_ANIMATOR_PARAM = "isActive";

    public static event Action OnRefillLives, OnTakeLife, OnStartTimer;

    [SerializeField]
    private Content contentFull, contentNotFull, contentNoLives;
    private Content currentContent;
    [SerializeField]
    private Image popupBackground;
    [SerializeField]
    private Animator popupAnimator;
    [SerializeField]
    private LifeTimer timer;
    [SerializeField]
    private LifeController lifeController;

    private void Awake()
    {
        currentContent = contentNoLives;
    }

    public void UpdateData(ISubject subject)
    {
        TryUpdateCurrentContent();
    }

    public void TogglePopup()
    {
        if (popupBackground.gameObject.activeSelf) 
        { 
            StartCoroutine(ClosePopup()); 
        } 
        else 
        { 
            StartCoroutine(OpenPopup()); 
        }
    }

    private void UseLife()
    {
        if (lifeController.IsLivesMoreThanMin()) OnTakeLife?.Invoke();
        TryUpdateCurrentContent();
        if (lifeController.Lives == (LifeController.MAX_LIVES - 1)) OnStartTimer?.Invoke();
    }

    private void RefillLives()
    {
        OnRefillLives?.Invoke();
        SetCurrentContent(contentFull, true);
    }

    private Content CalculateCurrentContent()
    {
        switch (lifeController.Lives)
        {
            case LifeController.MAX_LIVES:
                return contentFull;
            case LifeController.MIN_LIVES:
                return contentNoLives;
            default:
                return contentNotFull;
        }
    }

    private void TryUpdateCurrentContent()
    {
        Content content = CalculateCurrentContent();
        if (!currentContent.Equals(content))
        {
            SetCurrentContent(content, currentContent.gameObject.activeSelf);
        }
    }

    private void SetCurrentContent(Content content, bool skipAnimation = false)
    {
        currentContent.DeinitContent(lifeController, timer);        
        currentContent = content;
        currentContent.InitContent(lifeController, timer);
        currentContent.refillLives?.onClick.AddListener(RefillLives);
        currentContent.useLife?.onClick.AddListener(UseLife);
    }

    private IEnumerator OpenPopup()
    {
        popupBackground.gameObject.SetActive(true);
        popupAnimator.SetBool(IS_ACTIVE_ANIMATOR_PARAM, true);
        SetCurrentContent(CalculateCurrentContent());
        for (float i = 0; i < 0.5f; i += 0.01f)
        {
            popupBackground.color = new Color(popupBackground.color.r, popupBackground.color.g, popupBackground.color.b, i);
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator ClosePopup()
    {
        popupAnimator.SetBool(IS_ACTIVE_ANIMATOR_PARAM, false);
        for (float i = 0.5f; i >= 0f; i -= 0.01f)
        {
            popupBackground.color = new Color(popupBackground.color.r, popupBackground.color.g, popupBackground.color.b, i);
            yield return new WaitForFixedUpdate();
        }
        currentContent.DeinitContent(lifeController, timer);
        popupBackground.gameObject.SetActive(false);
    }
}
