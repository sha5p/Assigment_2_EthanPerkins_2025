using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Roulette : MonoBehaviour
{
    public float RotatePower;
    public float StopPower;

    private Rigidbody2D rbody;
    int inRotate;


    private void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    float t;
    private void FixedUpdate()
    {
        Debug.Log($"Current Angular Velocity: {rbody.angularVelocity}");
        Debug.Log($"Current Angular Velocity: {rbody.angularVelocity}");
        if (rbody.angularVelocity > 0)
        {
            rbody.angularVelocity -= StopPower * Time.deltaTime;

            rbody.angularVelocity = Mathf.Clamp(rbody.angularVelocity, 0, 1440);
        }

        if (rbody.angularVelocity == 0 && inRotate == 1)
        {
            Debug.Log("Spin");

            t += 1 * Time.deltaTime;
            if (t >= 0.5f)
            {
                Debug.Log("fiz");
                GetReward();

                inRotate = 0;
                t = 0;
            }
        }
    }


    public void Rotete()
    {
        if (inRotate == 0)
        {
            Debug.Log(rbody+""+ RotatePower);
            Debug.Log("Talk tou");
            
            rbody.AddTorque(RotatePower);
            inRotate = 1;
            
        }

    }
    void UpdateCoinDisplay()
    {
        if (coinCountText != null)
        {
            int currentDollars = PlayerPrefs.GetInt("CountDollar", 0);
            coinCountText.text = currentDollars.ToString(); 
            Debug.Log("Coin display updated to: " + currentDollars);
        }
        else
        {
            Debug.LogWarning("CoinCountText is not assigned in the Inspector! Cannot update UI.");
        }
    }
    public void bruh()
    {
        float coin = Random.Range(-30f, 20f);

        int currentDollars = PlayerPrefs.GetInt("CountDollar", 0); 

        currentDollars += Mathf.RoundToInt(coin);

        // Save the updated dollar count back to PlayerPrefs
        PlayerPrefs.SetInt("CountDollar", currentDollars);
        PlayerPrefs.Save();
        Debug.Log("Coin value generated: " + coin);
        Debug.Log("New total dollars: " + currentDollars);
        UpdateCoinDisplay();
    }
    public TextMeshProUGUI coinCountText;



    public void GetReward()
    {
        float rot = transform.eulerAngles.z;

        float offset = 22f;

        // Segment 1 (0-30 degrees)
        if (rot > (0 + offset) && rot <= (30 + offset))
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 30);
            Win(200);
        }
        // Segment 2 (30-60 degrees)
        else if (rot > (30 + offset) && rot <= (60 + offset))
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 60);
            Win(300);
        }
        // Segment 3 (60-90 degrees)
        else if (rot > (60 + offset) && rot <= (90 + offset))
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 90);
            Win(400);
        }
        // Segment 4 (90-120 degrees)
        else if (rot > (90 + offset) && rot <= (120 + offset))
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 120);
            Win(500);
        }
        // Segment 5 (120-150 degrees)
        else if (rot > (120 + offset) && rot <= (150 + offset))
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 150);
            Win(600);
        }
        // Segment 6 (150-180 degrees)
        else if (rot > (150 + offset) && rot <= (180 + offset))
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 180);
            Win(700);
        }
        // Segment 7 (180-210 degrees)
        else if (rot > (180 + offset) && rot <= (210 + offset))
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 210);
            Win(800);
        }
        // Segment 8 (210-240 degrees) - New
        else if (rot > (210 + offset) && rot <= (240 + offset))
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 240);
            Win(900); // Continued pattern
        }
        // Segment 9 (240-270 degrees) - New
        else if (rot > (240 + offset) && rot <= (270 + offset))
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 270);
            Win(1000); // Continued pattern
        }
        // Segment 10 (270-300 degrees) - New
        else if (rot > (270 + offset) && rot <= (300 + offset))
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 300);
            Win(1100); // Continued pattern
        }
        // Segment 11 (300-330 degrees) - New
        else if (rot > (300 + offset) && rot <= (330 + offset))
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 330);
            Win(1200); // Continued pattern
        }
        // Segment 12 (330-360 degrees, wraps around to 0 for visual snap)
        else if (rot > (330 + offset) && rot <= (360 + offset))
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 0); // Snaps to 0 degrees
            Win(100); 
        }
        else
        {
            Debug.LogWarning("Roulette rotation (" + rot + ") did not fall into any defined segment.");
        }
    }
    public void Win(int Score)
    {
        print(Score);
    }


}