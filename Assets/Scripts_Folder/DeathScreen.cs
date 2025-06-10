using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {

    }


    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Trigger entered by: " + other.gameObject.name);
        if (other.gameObject.name == "PizzaMan")
        {
            Debug.Log("The pizza man entered the trigger! Resetting the scene...");
            // Reload   scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}