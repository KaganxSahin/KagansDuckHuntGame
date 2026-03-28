using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreObject : MonoBehaviour
{
    // --- UI Elements ---
    public TMP_Text lbText;
    private Canvas cv;

    // --- Core Functions ---
    
    // Spawns the floating score text at the target's position
    public void ShowWithPosition(Vector3 pos, int score)
    {
        // Find the TextMeshPro component inside the Canvas child object
        lbText = gameObject.transform.Find("Canvas/lb").GetComponent<TMP_Text>();
        
        SetScore(score);
        gameObject.transform.localPosition = pos;      
        
        // Destroy this floating text object after 1 second
        SetTimeOut(1, () => {           
            Destroy(gameObject);
        });
    }

    // Sets the text value and scales the UI element based on the score
    public void SetScore(int score)
    {
        if (lbText != null)
        {
            lbText.text = "+" + score;
            
            // Larger scores will appear physically bigger on the screen
            float scale_ = score / 100.0f;
            gameObject.transform.localScale = new Vector3(scale_, scale_, scale_);
        }
    }

    // --- Helper Coroutines ---
    
    // Set a timeout to execute an action after a delay
    public Coroutine SetTimeOut(float delay, System.Action action)
    {
        return StartCoroutine(WaitAndDo(delay, action));
    }

    // Coroutine to wait for a specific time before firing the action
    private IEnumerator WaitAndDo(float time, System.Action action = null)
    {
        yield return new WaitForSeconds(time);
        if (action != null)
            action();
    }
}