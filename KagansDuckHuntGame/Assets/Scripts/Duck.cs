using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Enum to define different types of targets with potentially different behaviors
public enum Duck_Type {
    duck = 1,
    duck_king = 2
}

public class Duck : MonoBehaviour
{
    // --- Audio and Visual Assets ---
    public AudioClip soundFall;
    public AudioClip soundBumb;
    public AudioClip soundTalk;
    public GameObject white_circle;

    // --- Target Attributes ---
    public Duck_Type type;
    public int score;
    public int hp = 1;
    public float speed = 6;
    
    // --- State Variables ---
    bool isLeft = false; // Tracks movement direction
    int status = 0;      // 0: Flying, 1: Hit, 2: Falling, 3: Destroyed
    private float scale;

    void Start()
    {
        // Store initial scale for flipping the sprite later
        scale = gameObject.transform.localScale.y;
        
        // Randomize the flight speed slightly for variety
        speed = Random.Range(speed, speed + 5); 
    }

    void Update()
    {
        Vector3 pos = gameObject.transform.localPosition;
    
        // Status 0: Target is currently flying
        if (status == 0)
        {
            float y_ = Time.deltaTime * 3;
            
            // Randomize vertical movement to avoid straight lines
            if (pos.y > 3) 
            {
                y_ = -Time.deltaTime * 3;
            }
            
            // Move Right
            if (gameObject.transform.localPosition.x < 12 && !isLeft)
            {
                gameObject.transform.localPosition = new Vector3(pos.x + Time.deltaTime * speed, pos.y + y_, pos.z);
                gameObject.transform.localScale = new Vector3(scale, scale, scale); // Face right
                isLeft = false;
            }
            // Reached right edge, spawn left
            else if (gameObject.transform.localPosition.x > 12 && !isLeft)
            {
                randomFly();
            }
            // Move Left
            else if (gameObject.transform.localPosition.x > -12 && isLeft)
            {
                gameObject.transform.localPosition = new Vector3(pos.x - Time.deltaTime * speed, pos.y + y_, pos.z);
                gameObject.transform.localScale = new Vector3(-scale, scale, scale); // Face left
                isLeft = true;
            }
            // Reached left edge, spawn right
            else if (gameObject.transform.localPosition.x < -12 && isLeft)
            {
                randomFly();
            }
        }
        // Status 2: Target has been hit and is falling
        else if (status == 2)
        {
            // Keep falling until it goes off-screen
            if (pos.y > -8) 
            {
                gameObject.transform.localPosition = new Vector3(pos.x, pos.y - Time.deltaTime * speed * 1.5f, pos.z);
            }
            else 
            {
                // Destroy the game object once it's out of view
                status = 3;
                Destroy(gameObject);
            }
        }
    }

    // --- Flight and Spawn Mechanics ---
    // Respawns the target at a random edge with a random height
    public void randomFly()
    {
        // 50% chance to play the "quack/talk" sound on spawn
        if(Random.Range(0, 10) > 5) 
        {
            GetComponent<AudioSource>().clip = soundTalk;
            GetComponent<AudioSource>().Play();
        }
        
        // 50% chance to spawn on the left side
        if (Random.Range(0, 10) > 5)
        {
            transform.localPosition = new Vector3(-15, Random.Range(-5, 2), 0);
            isLeft = false;
        }
        // 50% chance to spawn on the right side
        else
        {
            transform.localPosition = new Vector3(15, Random.Range(-5, 2), 0);
            isLeft = true;
        }
    }

    // --- Hit and Fall Mechanics ---
    // Triggers the falling animation and sound
    public void fall()
    {
        status = 2; // Set status to falling
        GetComponent<AudioSource>().clip = soundFall;
        GetComponent<AudioSource>().Play();
        GetComponent<Animator>().Play("fall");
    }

    // Called at the end of the "hit" animation
    public void endHit()
    {
        GetComponent<Animator>().Play("fall");
        
        // If Health is 0 or less, start falling
        if (hp <= 0)
        {
            fall();
        }
        // If it still has Health (e.g. boss target), resume flying
        else
        {
            status = 0;
            GetComponent<Animator>().Play("fly");
        }
    }

    // Called externally (e.g. by GameController) when the trigger is pulled and target is hit
    public void onDamge(int damage)
    {
        GetComponent<AudioSource>().clip = soundBumb;
        GetComponent<AudioSource>().Play();
        status = 1; // Set status to hit/stunned
        hp -= damage; // Reduce health
        GetComponent<Animator>().Play("hit");
    }
}