using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    public Canvas canvas;
    private bool isPaused = false;
    private PlayerControl inputActions;
    public GameObject playerContent;
    public GameObject questsContent;
    public GameObject mapContent;
    public CanvasGroup uiBlockerCanvasGroup; 
    public EventSystem eventSystem;
    public GameObject Player;
    public GameObject blockPauseGameObject;
    public MonoBehaviour scriptOne;
    public MonoBehaviour scriptTwo;
    private List<GameObject> contentObjects = new List<GameObject>();
    public TextMeshProUGUI Money_Display;
    private void Awake()
    {
        inputActions = new PlayerControl();

        inputActions.UI.Enable();

        inputActions.UI.Pause.performed += OnPausePerformed;

        InitializeContentObjects();
    }

    private void OnDestroy()
    {
        inputActions.UI.Pause.performed -= OnPausePerformed;
        inputActions.UI.Disable();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        
        if (canvas != null && Time.timeScale != 0f)
        {
            eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(Player);
            bool newCanvasState = !canvas.enabled;

            canvas.enabled = newCanvasState;

            if (newCanvasState) 
            {
                ShowPlayerContent();
                EnableUIBlocker();
            }

            int savedCoins = PlayerPrefs.GetInt("CountDollar", 0);

            var z = savedCoins.ToString();
            Money_Display.text = z;
            TogglePause();

        }
        else if (canvas.enabled && Time.timeScale ==0)

        {
            eventSystem.SetSelectedGameObject(Player);
            bool newCanvasState = !canvas.enabled;

            canvas.enabled = newCanvasState;

            if (newCanvasState) 
            {
                ShowPlayerContent();
                EnableUIBlocker();
            }
            else
            {
                SetAllContentVisible(false); 
                DisableUIBlocker(); 
            }
            DisableUIBlocker();
            TogglePause();
        }
    }
    Audio_Manager audio_manager;
    public void TogglePause()
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        audio_manager.PlaySFX(audio_manager.ClickSound);
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    void Start()
    {
        SetAllContentVisible(false); 
        ShowPlayerContent();         
        DisableUIBlocker(); 
    }

    private void InitializeContentObjects()
    {
        contentObjects.Clear(); // Clear first

        if (playerContent != null)
        {
            contentObjects.Add(playerContent);
        }
        if (questsContent != null)
        {
            contentObjects.Add(questsContent);
        }
        if (mapContent != null)
        {
            contentObjects.Add(mapContent);
        }
    }

    public void OnMapButtonClick()
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        audio_manager.PlaySFX(audio_manager.ClickSound);
        ShowMapContent();
    }

    public void OnPlayerButtonClick()
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        audio_manager.PlaySFX(audio_manager.ClickSound);
        ShowPlayerContent();
    }

    public void OnQuestsButtonClick()
    {
        ShowQuestsContent();
    }

    private void ShowMapContent()
    {
        SetAllContentVisible(false); 
        if (mapContent != null)
        {
            SetChildrenVisible(mapContent, true);
        }
    }

    private void ShowPlayerContent()
    {
        SetAllContentVisible(false); 
        if (playerContent != null)
        {
            SetChildrenVisible(playerContent, true);
        }
    }

    private void ShowQuestsContent()
    {
        SetAllContentVisible(false); 
        if (questsContent != null)
        {
            SetChildrenVisible(questsContent, true);
        }
    }

    void SetChildrenVisible(GameObject parentObject, bool isVisible)
    {
        if (parentObject == null) return; 

        foreach (Transform child in parentObject.transform)
        {
            if (child != null) 
            {
                child.gameObject.SetActive(isVisible);
            }
        }
        parentObject.SetActive(isVisible);
    }

    void SetAllContentVisible(bool isVisible)
    {
        foreach (GameObject contentObject in contentObjects)
        {
            if (contentObject != null)
            {
                contentObject.SetActive(isVisible);
                SetChildrenVisible(contentObject, isVisible);
            }
        }
    }

    private void EnableUIBlocker()
    {
        if (uiBlockerCanvasGroup != null)
        {
            uiBlockerCanvasGroup.interactable = true;
        }
    }

    private void DisableUIBlocker()
    {
        if (uiBlockerCanvasGroup != null)
        {
            uiBlockerCanvasGroup.interactable = false;
        }
    }
}
