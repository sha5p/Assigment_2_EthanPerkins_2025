using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class arrow_Pare : MonoBehaviour
{
    public Transform target;
    public float arrowspeed;
    public EventSystem eventSystem;
    public Canvas canvas;
    public CanvasGroup uiBlockerCanvasGroup;
    public GameObject Reset;
    public TextMeshProUGUI textMeshProUGUI;
    public GameObject selection;
    void Start()
    {

    }

    void Update()
    {
        if (target == null || target == selection.transform)
        {
            canvas.enabled = true;
            eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(Reset);
            EnableUIBlocker();
            // If null, print 
            Debug.Log("you killed npce you lsoe");
            GameObject[] npcsInScene = GameObject.FindGameObjectsWithTag("NPC");

            if (npcsInScene.Length == 0)
            {
                Debug.Log("No NPCs left in the scene!");
                
            }
            Time.timeScale = 0f;

            return; 

        }
        else
        {
            Vector3 relativePos = target.position - transform.position;

            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = rotation;
        }
        
    }
    private void EnableUIBlocker()
    {
        if (uiBlockerCanvasGroup != null)
        {
            uiBlockerCanvasGroup.interactable = true;
        }
    }
    public  void resetGame()
    {
        Time.timeScale = 1f;

        PlayerPrefs.DeleteAll();
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
