using UnityEngine.UI;
using UnityEngine;

public class UITutorialPopUp : UIWindow
{
    public override WINDOW_NAME WindowName => WINDOW_NAME.TUTORIAL_POPUP;

    public Image currentPageImage;
    public Button exit;
    public Button previous;
    public Button next;

    private int currentPage = 0;

    private void OnEnable()
    {
        SetTutorial();
        //Debug.Log(currentPage);
    }

    private void SetTutorial()
    {
        previous.interactable = false;
        next.interactable = true;
        currentPage = 0;
        currentPageImage.sprite = GameManager.uiManager.tutorialPages[currentPage];
    }

    public void OnButtonExit()
    {
        GameManager.PlayButtonSFX();
        Close();
    }

    public void OnButtonPrevious()
    {
        GameManager.PlayButtonSFX();
        if (currentPage <= 0)
        {
            CheckPage();
            //Debug.Log(currentPage);
            currentPageImage.sprite = GameManager.uiManager.tutorialPages[currentPage];
        }
        else
        {
            --currentPage;
            //Debug.Log(currentPage);
            currentPageImage.sprite = GameManager.uiManager.tutorialPages[currentPage];
            CheckPage();
        }
    }

    public void OnButtonNext()
    {
        GameManager.PlayButtonSFX();
        if (currentPage >= GameManager.uiManager.tutorialPages.Length - 1)
        {
            CheckPage();
            //Debug.Log(currentPage);
            currentPageImage.sprite = GameManager.uiManager.tutorialPages[currentPage];
        }
        else
        {
            ++currentPage;
            //Debug.Log(currentPage);
            currentPageImage.sprite = GameManager.uiManager.tutorialPages[currentPage];
            CheckPage();
        }
    }

    private void CheckPage()
    {
        if(currentPage == 0)
        {
            previous.interactable = false;
            next.interactable = true;
        }
        else if(currentPage == GameManager.uiManager.tutorialPages.Length - 1)
        {
            previous.interactable = true;
            next.interactable = false;
            currentPage = GameManager.uiManager.tutorialPages.Length - 1;
        }
        else
        {
            previous.interactable = true;
            next.interactable = true;
        }
    }
}