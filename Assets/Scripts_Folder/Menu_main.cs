using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu_main : MonoBehaviour
{
    Audio_Manager audio_manager;
    public GameObject button;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EventSystem.current.SetSelectedGameObject(button);
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EventSystem.current.SetSelectedGameObject(button);
            
        }
    }
    // Function to open a specific scene
    public void OpenScene(string sceneName)
    {
        audio_manager.PlaySFX(audio_manager.ClickSound);
        SceneManager.LoadScene(sceneName);
    }

    // Function to close the game
    public void QuitGame()
    {
        audio_manager.PlaySFX(audio_manager.ClickSound);
        Application.Quit();


    }
    public void OnMyButtonClicked()
    {
        EventSystem.current.SetSelectedGameObject(button);
    }
}