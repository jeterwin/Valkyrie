using UnityEngine;
using UnityEngine.Events;
public class MainMenuScript : MonoBehaviour
{
    public Animator MainCanvasAnimator;
    public UnityEvent @Event;
    public UnityEvent ClickedBackFromPlay;
    public UnityEvent ClickedBackFromOptions;
    public UnityEvent ClickedPlay;
    public UnityEvent ClickedOptions;

    private bool canClickButton = true;
    private bool invokedAnyKey = false;
    private void Update()
    {
        if (Input.anyKeyDown && invokedAnyKey == false)
        {
            invokedAnyKey = true;
            Event.Invoke();
        }
    }
    public void DisableMainCanvasAnimator()
    {
        MainCanvasAnimator.enabled = false;
    }
    public void EnableMainCanvasAnimator()
    {
        MainCanvasAnimator.enabled = true;
    }
    public void ClickPlay()
    {
        if(!CheckIfCanClick()) { return; }
        ClickedPlay.Invoke();
    }
    public void ClickOptions()
    {
        if(!CheckIfCanClick()) { return; }
        ClickedOptions.Invoke();
    }
    public void ClickQuit()
    {
        Application.Quit();
    }
    public void ClickedBackFromplay()
    {
        if(!CheckIfCanClick()) { return; }
        ClickedBackFromPlay.Invoke();
    }
    public void ClickedBackFromoptions()
    {
        if(!CheckIfCanClick()) { return; }
        ClickedBackFromPlay.Invoke();
    }
    private bool CheckIfCanClick()
    {
        return canClickButton;
    }

    public void CanClick()
    {
        canClickButton = true;
    }
    public void CannotClick()
    {
        canClickButton = false;
    }
}
