using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float floodTimer = 60.0f;    // Time for one cycle
    public TextMeshProUGUI timerText; // Drag the UI Text here

    // --- Flood Rise Variables ---
    [Header("Flood Settings")]
    public GameObject waterPlane;       // NEW: Drag the WaterPlane here
    public float floodTargetHeight = 5f;  // NEW: How high the water will rise
    public float floodResetHeight = -10f; // NEW: Where the water will reset to
    public float floodSpeed = 1f;         // NEW: Speed of the water rising
    
    private float currentTimer;
    private bool isFlooding = false;    // NEW: State to control the flood

    void Start()
    {
        currentTimer = floodTimer;
        isFlooding = false;
        
        // NEW: Ensure water is at its reset position at the start
        waterPlane.transform.position = new Vector3(
            waterPlane.transform.position.x,
            floodResetHeight,
            waterPlane.transform.position.z
        );
    }

    void Update()
    {
        // If in the flooding state
        if (isFlooding)
        {
            RiseWater(); // NEW: Call the water rising function
        }
        // Otherwise (normal state)
        else
        {
            RunTimer(); // NEW: Call the timer function
        }
    }

    // NEW: Separated the timer logic
    void RunTimer()
    {
        currentTimer -= Time.deltaTime;

        if (currentTimer > 0)
        {
            // Update the timer
            timerText.text = "Flood in: " + Mathf.Ceil(currentTimer).ToString();
        }
        else
        {
            // Time's up! Start the flood
            StartFlood();
        }
    }

    // NEW: Function to start the flood
    void StartFlood()
    {
        isFlooding = true;
        timerText.text = "FLOODING!"; // Or "FLOOD!!!"

        // Find ALL objects with the tag "Plant"
        GameObject[] allPlants = GameObject.FindGameObjectsWithTag("Plant");
        
        // Destroy them one by one
        foreach (GameObject plant in allPlants)
        {
            Destroy(plant);
        }
    }

    // NEW: Function to make the water rise
    void RiseWater()
    {
        // If the water hasn't reached the target height
        if (waterPlane.transform.position.y < floodTargetHeight)
        {
            // Move the water up a bit (Vector3.up)
            waterPlane.transform.Translate(Vector3.up * floodSpeed * Time.deltaTime);
        }
        else
        {
            // Water has reached max height, start a new cycle
            ResetCycle();
        }
    }

    // NEW: Function to reset the cycle
    void ResetCycle()
    {
        isFlooding = false;
        currentTimer = floodTimer; // Reset the timer

        // Reset the water position back down
        waterPlane.transform.position = new Vector3(
            waterPlane.transform.position.x,
            floodResetHeight,
            waterPlane.transform.position.z
        );
    }
}