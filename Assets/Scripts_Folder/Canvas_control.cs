using UnityEngine;

public class Canvas_control : MonoBehaviour
{
    public CanvasGroup canvasGroupToControl;

    public void EnableInputOnCanvas()
    {
        canvasGroupToControl.blocksRaycasts = true;
        canvasGroupToControl.interactable = true; 
    }

    public void DisableInputOnCanvas()
    {
        canvasGroupToControl.blocksRaycasts = false;
        canvasGroupToControl.interactable = false; 
    }

    public GameObject myMenu;
    public void ShowMenu()
    {
        myMenu.SetActive(true);
        EnableInputOnCanvas(); 
    }

    public void HideMenu()
    {
        myMenu.SetActive(false);
        DisableInputOnCanvas(); 
    }
}