using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Threading;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public GameObject interactionInfoUI;
    public TextMeshProUGUI interactionText;
    public GameObject mainUI;

    public PlayerControl inputActions;
    public InputActionReference actionReference;

    private InputAction action;
    private bool isPaused = false;
    private Transform selectedObject;
    private bool mainUIVisible = true;
    public Quests_Deliver questsDeliverScript;

    private List<CanvasGroup> interactableCanvasGroups = new List<CanvasGroup>();

    [SerializeField] private GameObject pileOfCoins;
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private Vector2[] initialPos;
    [SerializeField] private Quaternion[] initialRotation;
    [SerializeField] private int coinsAmount; // This will now be set dynamically

    private readonly Vector2 desiredInitialCoinPosition = new Vector2(50f, -50f);

    void Start()
    {
        InitializeCoinPositions();

        int savedCoins = PlayerPrefs.GetInt("CountDollar", 0);
        if (counter != null)
        {
            counter.text = savedCoins.ToString();
            Debug.Log($"Loaded {savedCoins} coins from PlayerPrefs at game start.");
        }
        else
        {
            Debug.LogWarning("Counter TextMeshProUGUI is not assigned. Cannot display saved coins at game start.");
        }
    }

    private void InitializeCoinPositions()
    {
        if (pileOfCoins != null && pileOfCoins.transform.childCount > 0)
        {
            initialPos = new Vector2[pileOfCoins.transform.childCount];
            initialRotation = new Quaternion[pileOfCoins.transform.childCount];

            for (int i = 0; i < pileOfCoins.transform.childCount; i++)
            {
                RectTransform coinRectTransform = pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>();
                if (coinRectTransform != null)
                {
                    initialPos[i] = coinRectTransform.anchoredPosition;
                    initialRotation[i] = coinRectTransform.rotation;
                }
            }
        }
        else
        {
            Debug.LogWarning("PileOfCoins GameObject is null or has no children. Cannot initialize coin positions.");
        }
    }


    private void Reset()
    {
        if (pileOfCoins == null || initialPos == null || initialRotation == null)
        {
            Debug.LogWarning("Reset cannot proceed: pileOfCoins or initial position/rotation arrays are not initialized.");
            return;
        }

        for (int i = 0; i < pileOfCoins.transform.childCount; i++)
        {
            RectTransform coinRectTransform = pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>();
            if (coinRectTransform != null && i < initialPos.Length && i < initialRotation.Length) // Add bounds check
            {
                coinRectTransform.anchoredPosition = initialPos[i];
                coinRectTransform.rotation = initialRotation[i];
                coinRectTransform.DOScale(0f, 0f);
                coinRectTransform.gameObject.SetActive(true);
            }
        }
        pileOfCoins.SetActive(true);
    }
    [SerializeField] Timer time1;
    public void RewardPileOfCoin(int noCoin, float numCoins)
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        audio_manager.PlaySFX(audio_manager.coin);
        Reset();
        time1.End();
        var delay = 0f;
        pileOfCoins.SetActive(true);

        Vector2 targetCoinTopLeft = new Vector2(-400f, 150);

        int coinsToAnimate = Mathf.Min(noCoin, pileOfCoins.transform.childCount);

        for (int i = 0; i < coinsToAnimate; i++)
        {
            RectTransform coinRectTransform = pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>();

            if (coinRectTransform != null)
            {
                coinRectTransform.DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);
                coinRectTransform.DOAnchorPos(targetCoinTopLeft, 1f).SetDelay(delay).SetEase(Ease.OutQuad);
                coinRectTransform.DOScale(0f, 0.3f).SetDelay(delay + 1f).SetEase(Ease.InBack);

                delay += 0.1f;
            }
        }
        var currentMulti = PlayerPrefs.GetFloat("Multi", 1.0f);

        numCoins = currentMulti * numCoins;
        
        StartCoroutine(CountDollars(Mathf.RoundToInt(numCoins)));
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");

        

        GameObject arrow_Pare = GameObject.FindWithTag("arrowBoi");
        arrow_Pare.SetActive(true);
        Transform myGameObject = transform;
        foreach (Transform child in arrow_Pare.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    IEnumerator CountDollars(int coinAmount)
    {
        yield return new WaitForSecondsRealtime(0.5f);

        PlayerPrefs.SetInt("CountDollar", PlayerPrefs.GetInt("CountDollar") + coinAmount);

        if (counter != null)
        {
            counter.text = PlayerPrefs.GetInt("CountDollar").ToString();
        }

    }

    private void Awake()
    {
        inputActions = new PlayerControl();
        action = actionReference.action;

        if (action == null)
        {
            Debug.LogError($"Action is null. Please ensure an InputActionReference is assigned and that it's a valid action in the Inspector.");
            this.enabled = false;
            return;
        }

        action.Enable();
        action.performed += OnActionPerformed;
    }

    private void OnEnable()
    {
        if (action != null)
        {
            action.Enable();
            action.performed += OnActionPerformed;
        }
    }

    private void OnDisable()
    {
        if (action != null)
        {
            action.Disable();
            action.performed -= OnActionPerformed;
        }
    }

    private void OnDestroy()
    {
        if (action != null)
        {
            action.performed -= OnActionPerformed;
        }
    }
    public LayerMask mylayermask;
    void Update() // Or FixedUpdate, depending on your needs
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, mylayermask))
        {
            if (hit.transform != selectedObject) // Check if a new object is being hovered over
            {
                selectedObject = hit.transform;

                InteractableObject[] interactables = selectedObject.GetComponents<InteractableObject>();

                InteractableObject foundInteractable = null;

                foreach (InteractableObject interactable in interactables)
                {
                    if (interactable != null && interactable.enabled)
                    {
                        foundInteractable = interactable;
                        break; 
                    }
                }

                if (foundInteractable != null)
                {
                    interactionInfoUI.SetActive(true);
                    if (interactionText != null)
                    {
                        interactionText.text = foundInteractable.GetItemName();
                    }
                    else
                    {
                        Debug.LogWarning("Interaction Text UI component is not assigned in the Inspector. Cannot display item name.");
                    }
                }
                else // No enabled InteractableObject found on the selectedObject
                {
                    interactionInfoUI.SetActive(false);
                }
            }
        }
        else
        {
            if (selectedObject != null) 
            {
                selectedObject = null;
                interactionInfoUI.SetActive(false); 
            }
        }
    }
    Audio_Manager audio_manager;
    private void OnActionPerformed(InputAction.CallbackContext context)
    {
        InteractableObject interactable = null;
        if (selectedObject != null)
        {
            interactable = selectedObject.GetComponent<InteractableObject>();
        }

        if (interactable != null && interactable.enabled && selectedObject.CompareTag("Quest"))
        {
            if (interactionInfoUI != null && mainUI != null)
            {
                bool newState = !interactionInfoUI.activeSelf;

                ToggleCanvasGroups(newState);

                interactionInfoUI.SetActive(newState);

                interactable.OnInteract();

                if (questsDeliverScript != null)
                {
                    questsDeliverScript.SetQuestUIActive(!newState);
                }
                else
                {
                    Debug.LogWarning("Quests_Deliver script is not assigned. Cannot toggle quest UI.");
                }

                TogglePause();

                Debug.Log($"Canvas '{interactionInfoUI.name}' visibility toggled to {newState}. Game paused: {isPaused}");
            }
            else
            {
                if (interactionInfoUI == null)
                    Debug.LogWarning("interactionInfoUI is not assigned. Please assign a Canvas GameObject in the Inspector.");
                if (mainUI == null)
                    Debug.LogWarning("mainUI is not assigned. Please assign a Canvas GameObject in the Inspector.");
            }
        }
        else if (interactable != null && interactable.enabled && selectedObject.CompareTag("NPC"))
        {
            if (questsDeliverScript != null)
            {
                int rewardAmount = questsDeliverScript.GetActiveQuestReward();
                float z = questsDeliverScript.GetActiveQuestReward();
                RewardPileOfCoin(rewardAmount,z);
                questsDeliverScript.CompleteActiveQuest();
                ResumeGameInteraction();
            }
            else
            {
                Debug.LogWarning("QuestsDeliverManager not assigned in SelectionManager. Cannot complete quest.");
            }
        }
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        audio_manager.PlaySFX(audio_manager.ClickSound);
    }

    public void ResumeGameInteraction()
    {
        Debug.Log("SelectionManager: Resuming game interaction.");
        isPaused = false;
        Time.timeScale = 1f;

        if (interactionInfoUI != null)
        {
            interactionInfoUI.SetActive(false);
        }
        ToggleCanvasGroups(false);

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void TogglePause()
    {
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

    private void SetCanvasGroupInteractable(GameObject root, bool isInteractable)
    {
        CanvasGroup rootCanvasGroup = root.GetComponent<CanvasGroup>();
        if (rootCanvasGroup != null)
        {
            rootCanvasGroup.interactable = isInteractable;
            rootCanvasGroup.blocksRaycasts = isInteractable;
        }

        foreach (Transform child in root.transform)
        {
            SetCanvasGroupInteractable(child.gameObject, isInteractable);
        }
    }

    private void ToggleCanvasGroups(bool enableInteractionUI)
    {
        if (mainUI != null)
        {
            SetCanvasGroupInteractable(mainUI, !enableInteractionUI);
        }
        if (interactionInfoUI != null)
        {
            SetCanvasGroupInteractable(interactionInfoUI, enableInteractionUI);
        }
    }
}