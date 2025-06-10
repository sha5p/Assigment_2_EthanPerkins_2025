using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Quests_Deliver : MonoBehaviour
{
    public Transform questArea;
    public Transform playerTransform;
    public GameObject npcTextPrefab;
    public GameObject PlayerBut;
    public EventSystem eventSystem; 

    [Header("Quest Detail Texts")]
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI rewardInfoText;

    [Header("Quest State Management")]
    [SerializeField] private bool questIsActive = false;
    private GameObject activeQuestNpc = null;
    public CanvasGroup abandonQuestCanvasGroup;

    [Header("Active Quest Display")]
    public TextMeshProUGUI activeQuestDisplayText;

    public SelectionManager selectionManager;

    private List<Button> spawnedNpcButtons = new List<Button>();

    private int activeQuestReward = 0;
    [SerializeField] Timer time1;

    public GameObject parentObjectForGroupA; 
    public GameObject parentObjectForGroupB;
    Audio_Manager audio_manager;

    public void ActivateGroupA()
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        audio_manager.PlaySFX(audio_manager.ClickSound);
        SetChildrenActiveState(parentObjectForGroupA, true);  
        SetChildrenActiveState(parentObjectForGroupB, false); 
    }

    public void ActivateGroupB()
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        audio_manager.PlaySFX(audio_manager.ClickSound);
        SetChildrenActiveState(parentObjectForGroupB, true); 
        SetChildrenActiveState(parentObjectForGroupA, false);
    }
    private void SetChildrenActiveState(GameObject parentObject, bool activate)
    {
        if (parentObject == null)
        {
            Debug.LogWarning($"GroupActivator: A parent object for state change is not assigned!");
            return;
        }

        foreach (Transform child in parentObject.transform)
        {
            child.gameObject.SetActive(activate);

            CanvasGroup cg = child.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = activate ? 1 : 0;
                cg.interactable = activate;
                cg.blocksRaycasts = activate;
            }
        }
    }



    public void OnQuestUIVisible()
    {
        if (questArea == null)
        {
            Debug.LogError("Quest Area Transform not assigned!");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned!");
            return;
        }

        foreach (Transform child in questArea)
        {
            Destroy(child.gameObject);
        }
        spawnedNpcButtons.Clear();

        GameObject[] allNpcs = GameObject.FindGameObjectsWithTag("NPC");

        if (allNpcs.Length == 0)
        {
            Debug.LogWarning("No NPCs found with the 'NPC' tag.");
            UpdateQuestDetailsText("N/A", "N/A", "N/A");
            if (activeQuestDisplayText != null)
            {
                activeQuestDisplayText.text = questIsActive && activeQuestNpc != null ? $"Active Quest: Deliver to {activeQuestNpc.name}" : "No Active Quest";
            }
            return;
        }

        List<KeyValuePair<GameObject, float>> npcDistances = new List<KeyValuePair<GameObject, float>>();
        foreach (GameObject npc in allNpcs)
        {
            float distance = Vector3.Distance(playerTransform.position, npc.transform.position);
            npcDistances.Add(new KeyValuePair<GameObject, float>(npc, distance));
        }

        npcDistances = npcDistances.OrderBy(pair => pair.Value).ToList();

        GameObject closestNpc = null;
        GameObject farthestNpc = null;
        GameObject midRangeNpc = null;

        if (npcDistances.Count > 0)
        {
            closestNpc = npcDistances[0].Key;
        }

        if (npcDistances.Count > 1)
        {
            farthestNpc = npcDistances[^1].Key;
        }

        if (npcDistances.Count > 2)
        {
            midRangeNpc = npcDistances[npcDistances.Count / 2].Key;
        }
        else if (npcDistances.Count == 2)
        {
            midRangeNpc = npcDistances[1].Key;
        }

        CreateNpcText(closestNpc, "Closest NPC", "Easy", "2 minutes", "11 Gold", 11, 120); 
        CreateNpcText(midRangeNpc, "Mid-Range NPC", "Medium", "10 minutes", "25 Gold", 25, 600); 
        CreateNpcText(farthestNpc, "Farthest NPC", "Hard", "25 minutes", "50 Gold", 50, 1200);

        if (PlayerBut != null && PlayerBut.activeInHierarchy)
        {
            if (eventSystem != null && eventSystem.currentSelectedGameObject == null)
            {
                eventSystem.SetSelectedGameObject(PlayerBut);
            }
        }
        else if (PlayerBut == null)
        {
            Debug.LogWarning("PlayerBut is not assigned in Quests_Deliver. Cannot set initial selected object.");
        }
        else if (!PlayerBut.activeInHierarchy)
        {
            Debug.LogWarning("PlayerBut is inactive in the hierarchy. Cannot set as initial selected object.");
        }

        SetNpcButtonsInteractable(!questIsActive);

        if (abandonQuestCanvasGroup != null)
        {
            abandonQuestCanvasGroup.alpha = questIsActive ? 1 : 0;
            abandonQuestCanvasGroup.interactable = questIsActive;
            abandonQuestCanvasGroup.blocksRaycasts = questIsActive;
        }

        if (activeQuestDisplayText != null)
        {
            activeQuestDisplayText.text = questIsActive && activeQuestNpc != null ? $"Active Quest: Deliver to {activeQuestNpc.name}" : "No Active Quest";
        }
    }

    void CreateNpcText(GameObject npc, string label, string difficulty, string time, string rewardInfo, int rewardValue, int timeInSeconds)
    {
        if (npc != null)
        {
            GameObject buttonGO = Instantiate(npcTextPrefab, questArea);
            Button button = buttonGO.GetComponent<Button>();
            TextMeshProUGUI tmpText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            QuestButtonDetails questButtonDetails = buttonGO.GetComponent<QuestButtonDetails>();

            if (tmpText != null)
            {
                tmpText.text = label + ": " + npc.name;
                tmpText.fontSize = 16;
                tmpText.alignment = TextAlignmentOptions.Center;
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on the Button Prefab! Check your npcTextPrefab.");
                Destroy(buttonGO);
                return;
            }

            if (questButtonDetails != null)
            {
                questButtonDetails.Initialize(difficulty, time, rewardInfo, this);
            }
            else
            {
                Debug.LogError("QuestButtonDetails script not found on the Button Prefab! Did you attach it to npcTextPrefab?");
                Destroy(buttonGO);
                return;
            }

            button.onClick.AddListener(() => OnNpcButtonClicked(npc, rewardValue, timeInSeconds));

            spawnedNpcButtons.Add(button);

            button.interactable = !questIsActive;
        }
        else
        {
            Debug.Log("No " + label + " NPC found to create a button for.");
        }
    }
    
    public void OnNpcButtonClicked(GameObject clickedNpc, int questReward, int questDurationInSeconds)
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        audio_manager.PlaySFX(audio_manager.ClickSound);
        if (questIsActive)
        {
            Debug.Log($"Cannot accept quest from {clickedNpc.name}. A quest is already active from {activeQuestNpc?.name}.");
            return;
        }

        Debug.Log("Separate function called! Button clicked for NPC: " + clickedNpc.name);

        InteractableObject dialogueScript = clickedNpc.GetComponent<InteractableObject>();
        
        NPC_Movment npc_script =clickedNpc.GetComponent<NPC_Movment>();
        if (dialogueScript != null)
        {
            dialogueScript.enabled = true;
            npc_script.enabled = false;
            questIsActive = true;
            activeQuestNpc = clickedNpc;
            // NEW: Store the active quest's reward
            activeQuestReward = questReward;
            Debug.Log($"Quest from {clickedNpc.name} is now active with reward: {activeQuestReward} Gold!");

            SetNpcButtonsInteractable(false);

            if (abandonQuestCanvasGroup != null)
            {
                abandonQuestCanvasGroup.alpha = 1;
                abandonQuestCanvasGroup.interactable = true;
                abandonQuestCanvasGroup.blocksRaycasts = true;
            }

            if (activeQuestDisplayText != null)
            {
                activeQuestDisplayText.text = $"Active Quest: Deliver to {clickedNpc.name}";
            }
        }
        else
        {
            Debug.LogWarning($"Script (e.g., InteractableObject) not found on {clickedNpc.name}.");
        }

        eventSystem.SetSelectedGameObject(PlayerBut);
        time1.SetDuration(questDurationInSeconds).Begin();
    }

    public void AbandonActiveQuest()
    {
        if (selectionManager != null)
        {
            selectionManager.ResumeGameInteraction();
            selectionManager.TogglePause();
            Debug.Log("Game re-paused after abandoning quest.");
        }
        else
        {
            Debug.LogWarning("SelectionManager reference is missing in Quests_Deliver. Cannot resume game interaction.");
        }
        time1.End();
        
    }


    public int GetActiveQuestReward()
    {
        return activeQuestReward;
    }

    public void CompleteActiveQuest()
    {
        if (!questIsActive)
        {
            Debug.LogWarning("No quest is currently active to complete.");
            return;
        }

        Debug.Log($"Quest from {activeQuestNpc?.name} has been completed!");

        questIsActive = false;
        activeQuestNpc = null;
        activeQuestReward = 0;

        SetNpcButtonsInteractable(true);

        if (abandonQuestCanvasGroup != null)
        {
            abandonQuestCanvasGroup.alpha = 0;
            abandonQuestCanvasGroup.interactable = false;
            abandonQuestCanvasGroup.blocksRaycasts = false;
        }

        if (activeQuestDisplayText != null)
        {
            activeQuestDisplayText.text = "No Active Quest";
        }

        if (activeQuestNpc != null)
        {
            InteractableObject dialogueScript = activeQuestNpc.GetComponent<InteractableObject>();
            if (dialogueScript != null && dialogueScript.enabled)
            {
                dialogueScript.enabled = false;
                Debug.Log($"Dialogue for {activeQuestNpc.name} disabled upon completion.");
            }
        }

        UpdateQuestDetailsText("N/A", "N/A", "N/A");

        if (selectionManager != null)
        {
            selectionManager.ResumeGameInteraction();
        }
        else
        {
            Debug.LogWarning("SelectionManager reference is missing in Quests_Deliver. Cannot resume game interaction.");
        }
    }

    private void SetNpcButtonsInteractable(bool interactable)
    {
        foreach (Button button in spawnedNpcButtons)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
        }
    }

    public void SetQuestUIActive(bool isVisible)
    {
        gameObject.SetActive(isVisible);

        if (isVisible)
        {
            OnQuestUIVisible();
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                if (PlayerBut != null && PlayerBut.activeInHierarchy)
                {
                    EventSystem.current.SetSelectedGameObject(PlayerBut);
                }
            }
        }
        else
        {
            foreach (Transform child in questArea)
            {
                Destroy(child.gameObject);
            }
            spawnedNpcButtons.Clear();

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

            if (abandonQuestCanvasGroup != null)
            {
                abandonQuestCanvasGroup.alpha = 0;
                abandonQuestCanvasGroup.interactable = false;
                abandonQuestCanvasGroup.blocksRaycasts = false;
            }

            if (activeQuestDisplayText != null)
            {
                if (!questIsActive)
                {
                    activeQuestDisplayText.text = "No Active Quest";
                }
            }

            UpdateQuestDetailsText("N/A", "N/A", "N/A");
        }
    }

    public void UpdateQuestDetailsText(string difficulty, string time, string rewardInfo)
    {
        if (difficultyText != null)
        {
            difficultyText.text = "Difficulty: " + difficulty;
        }
        else
        {
            Debug.LogWarning("Difficulty Text not assigned in Quests_Deliver.");
        }

        if (timeText != null)
        {
            timeText.text = "Time: " + time;
        }
        else
        {
            Debug.LogWarning("Time Text not assigned in Quests_Deliver.");
        }

        if (rewardInfoText != null)
        {
            rewardInfoText.text = "Reward: " + rewardInfo;
        }
        else
        {
            Debug.LogWarning("Reward Info Text not assigned in Quests_Deliver.");
        }
    }





    public void TimerEND()
    {
        if (!questIsActive)
        {
            Debug.LogWarning("No quest is currently active to abandon.");
            return;
        }

        Debug.Log($"Quest from {activeQuestNpc?.name} has been abandoned.");

        questIsActive = false;
        activeQuestNpc = null;
        activeQuestReward = 0;

        SetNpcButtonsInteractable(true);

        if (abandonQuestCanvasGroup != null)
        {
            abandonQuestCanvasGroup.alpha = 0;
            abandonQuestCanvasGroup.interactable = false;
            abandonQuestCanvasGroup.blocksRaycasts = false;
        }

        if (activeQuestDisplayText != null)
        {
            activeQuestDisplayText.text = "No Active Quest";
        }

        if (activeQuestNpc != null)
        {
            InteractableObject dialogueScript = activeQuestNpc.GetComponent<InteractableObject>();
            if (dialogueScript != null && dialogueScript.enabled)
            {
                dialogueScript.enabled = false;
                Debug.Log($"Dialogue for {activeQuestNpc.name} disabled.");
            }
        }

        UpdateQuestDetailsText("N/A", "N/A", "N/A");

       

        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(PlayerBut);
        }
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");

        foreach (GameObject npc in npcs)
        {
            InteractableObject[] interactableObjects = npc.GetComponentsInChildren<InteractableObject>();
            NPC_Movment[] npc_script = npc.GetComponentsInChildren<NPC_Movment>();
            foreach (NPC_Movment z in npc_script)
            {

                z.enabled = true;
                Debug.Log($"Disabled InteractableObject script on: {npc.name}");
            }
            foreach (InteractableObject interactable in interactableObjects)
            {
                interactable.enabled = false;
                Debug.Log($"Disabled InteractableObject script on: {npc.name}");
            }
        }

        GameObject arrow_Pare = GameObject.FindWithTag("arrowBoi");
        arrow_Pare.SetActive(true);
        Transform myGameObject = transform;
        foreach (Transform child in arrow_Pare.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}