// QuestButtonDetails.cs
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class QuestButtonDetails : MonoBehaviour, ISelectHandler
{
    private string questDifficulty;
    private string questTime;
    private string questRewardInfo;

    private Quests_Deliver questDeliverManager; 

    public void Initialize(string difficulty, string time, string reward, Quests_Deliver manager)
    {
        questDifficulty = difficulty;
        questTime = time;
        questRewardInfo = reward; 
        questDeliverManager = manager; 
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (questDeliverManager != null)
        {
            Debug.Log($"Button '{gameObject.name}' Selected. Updating quest details from QuestButtonDetails.");
            questDeliverManager.UpdateQuestDetailsText(questDifficulty, questTime, questRewardInfo);
        }
        else
        {
            Debug.LogWarning("QuestDeliverManager reference is null on QuestButtonDetails for " + gameObject.name);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        
    }
}