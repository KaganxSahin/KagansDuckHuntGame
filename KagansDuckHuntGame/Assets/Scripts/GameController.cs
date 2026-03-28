using System.Collections;
using System.Collections.Generic;
using System.Media;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // --- UI Layers & Canvases ---
    public GameObject beginLayer;
    public GameObject blackLayer;
    public GameObject gameLayer;
    public GameObject gameOverLayer;
    public GameObject topMenuLayer;
    
    // --- UI Text Elements ---
    public GameObject lbScore;
    public GameObject lbTime;
    public GameObject lbOverScore;
    public GameObject ScoreTextPrefap;

    // --- Game Objects & Audio ---
    public List<GameObject> duckInstanceArr;
    List<Duck> duckArr;
    public List<AudioClip> audioArr;

    // --- Game State Variables ---
    private int gameScore;
    private int gameTime;
    public int gameStatus; // -1: Stopped/Over, 0: Playing
    public int gunDamage = 1;
    
    // --- Hardware Communication Variables ---
    bool isReadSensor = false;
    ComController comController;

    void Start()
    {
        duckArr = new List<Duck>();
        
        // Initialize UI states
        HideBlackScreen();
        gameOverLayer.SetActive(false);
        beginLayer.SetActive(true);
        topMenuLayer.SetActive(false);
        gameLayer.SetActive(false);
        
        gameScore = 0;
        gameStatus = -1;
    }

    // Connects to the Arduino COM port and starts the game on success
    public void StartComConnect()
    {
        comController = GetComponent<ComController>();
        comController.CreatePortWithCallback(() => {
            StartGame(); // Start game when COM is ready
        });
    }

    // Initializes the game loop, timer, and spawns the first targets
    public void StartGame()
    {
        gameStatus = 0;
        gameTime = 90; // 90 seconds gameplay
        
        beginLayer.SetActive(false);
        gameLayer.SetActive(true);
        gameOverLayer.SetActive(false);
        topMenuLayer.SetActive(true);
        
        AddNewBird();
        SetTimeOut(2, () =>
        {
            AddNewBird(); // Spawn second target after 2 seconds
        });
        
        setScore(0);
        updateTime(gameTime);
    }

    // Instantiates a new target from the prefab list
    public void AddNewBird()
    {
        int k = Random.Range(0, duckInstanceArr.Count - 1);
        Duck duck = Instantiate(duckInstanceArr[k], gameLayer.transform).GetComponent<Duck>();
        duckArr.Add(duck);
        duck.randomFly();
    }

    // --- Classic Light Gun Mechanics (Black Screen / White Target) ---
    
    public void ShowBlackScreen()
    {
        blackLayer.SetActive(true);
    }

    public void ShowWhiteCircle(bool val, int index)
    {
        duckArr[index].white_circle.SetActive(val);
    }

    public void HideBlackScreen()
    {
        blackLayer.SetActive(false);
        foreach (Duck d in duckArr)
        {
            d.white_circle.SetActive(false);
        }
    }

    // Triggered when the physical gun switch is pressed
    public void ShootBegin()
    {
        ShowBlackScreen();
        GetComponent<AudioSource>().clip = audioArr[0];
        GetComponent<AudioSource>().Play();
    }

    // Applies damage, updates score, and handles target elimination
    public void OnDamage(int duckIndex)
    {
        if (gameStatus != 0) return;
        
        // Note: This version currently supports detecting one duck at a time effectively
        Duck duck = duckArr[duckIndex];
        duck.onDamge(gunDamage);
        
        if (duck.hp == 0)
        {
            // Show floating score text
            ScoreObject lb = Instantiate(ScoreTextPrefap, gameLayer.transform).GetComponent<ScoreObject>();
            lb.ShowWithPosition(new Vector3(duck.transform.localPosition.x, duck.transform.localPosition.y + 1, duck.transform.localPosition.z), duck.score);         
            
            setScore(gameScore + duck.score);        
            duckArr.RemoveAt(duckIndex);
            AddNewBird(); // Replace the eliminated duck
        }
        
        // Play hit sound
        GetComponent<AudioSource>().clip = audioArr[1];
        GetComponent<AudioSource>().Play();
    }

    // --- Main Game Loop ---
    float eTime = 0;
    void Update()
    {
        if (gameStatus < 0) return;
        
        // Time countdown logic
        eTime += Time.deltaTime;
        if (eTime > 1)
        {
            eTime = 0;
            updateTime(gameTime - 1);
        }
        if (gameTime == 0)
        {
            GameOver();
        }
        
        // Listen to hardware inputs
        UpdateDuck();
    }

    // --- Hardware Input & Light Detection Logic ---
    float delayTime = 0.2f;
    float sensorTime = 0;
    int fpsState = 0;
    int fpsTotal = 0;
    int trigerState = 0;
    int sensorState = 0;
    float delayShoot = 0.0f;
    int duckSelect = 0;

    void UpdateDuck()
    {
        string s;
        if (gameStatus < 0) return;
        
        if (ComController.spCom.IsOpen && gameStatus == 0)
        {
            try
            {
                // Parse serial data from Arduino format: "trigger,sensor"
                s = ComController.spCom.ReadLine();
                string[] proArr = s.Split(","); 
                trigerState = int.Parse(proArr[0]);
                sensorState = int.Parse(proArr[1]);

                delayShoot -= Time.deltaTime;
                
                // If trigger is pulled, start the detection sequence
                if (trigerState == 1 && !isReadSensor && delayShoot < 0)
                {
                    isReadSensor = true;
                    duckSelect = 0;
                    fpsTotal = 0;
                    ShootBegin(); // Flash screen black
                    delayShoot = 0.5f; // Cooldown for shooting
                }
                
                // Read LDR sensor during the flash sequence
                if (isReadSensor)
                {
                    sensorTime += Time.deltaTime;
                  
                    if (duckSelect < duckArr.Count && duckArr.Count > 0)
                    {
                        if (fpsTotal == 0)
                        {                        
                            ShowBlackScreen();
                            ShowWhiteCircle(true, duckSelect); // Flash white circle on current target
                            sensorTime = 0;
                            fpsState = 0;
                        }
                        
                        Duck duck = duckArr[duckSelect];
                        fpsTotal += 1;
                        fpsState += sensorState;
                        
                        // Sequence timing logic for displaying targets and reading light
                        if (sensorTime > 0.2f * delayTime)
                        {
                            HideBlackScreen();
                        }
                        if (sensorTime > 0.3f * delayTime)
                        {
                            ShowWhiteCircle(false, duckSelect);
                        }
                        if (sensorTime > delayTime)
                        {
                            sensorTime = 0;
                            // Check if the sensor detected the white light on screen
                            if (fpsState > 0 && fpsState < fpsTotal) 
                            {
                                isReadSensor = false;
                                OnDamage(duckSelect);                              
                            }
                            else
                            {
                                duckSelect++; // Move to next target if missed
                            }                           
                            fpsTotal = 0;
                        }
                    }
                    else
                    {
                        isReadSensor = false; // Detection sequence ended
                    }
                }
            }
            catch
            {
                // Ignore parsing errors or serial disconnections smoothly
                return;
            }
        }
    }

    // --- UI Updaters ---
    
    public void updateTime(int gameTime_)
    {
        gameTime = gameTime_;
        int m = gameTime / 60;
        int s = gameTime % 60;
        string ss = s.ToString();
        if (s < 10) ss = "0" + s;
        string str = "0" + m + ":" + ss;
        lbTime.GetComponent<Text>().text = str;
    }

    public void setScore(int score_)
    {
        gameScore = score_;
        string str = score_.ToString();
        
        // Format score string with leading zeros
        if (score_ >= 100 && score_ < 1000)
        {
            str = "00" + score_;
        }
        else if (score_ >= 1000 && score_ < 10000)
        {
            str = "0" + score_;
        }
        else if (score_ < 10)
        {
            str = "0000" + score_;
        }
        lbScore.GetComponent<Text>().text = str;
    }

    // --- Game Control Methods ---

    public void GameOver()
    {
        gameOverLayer.SetActive(true);
        lbOverScore.GetComponent<TMPro.TMP_Text>().text = gameScore.ToString();
        
        GetComponent<AudioSource>().clip = audioArr[3];
        GetComponent<AudioSource>().Play();
        
        gameStatus = -1;
        duckArr.Clear();
        
        // Destroy all remaining targets
        foreach (Transform child in gameLayer.transform)
        {
            Destroy(child.gameObject);
        }        
    }

    public void PlayAgain()
    {
        duckArr.Clear();
        StartGame();
    }

    // --- Helper Coroutines ---
    
    // Set a timeout to execute an action after a delay
    public Coroutine SetTimeOut(float delay, System.Action action)
    {
        return StartCoroutine(WaitAndDo(delay, action));
    }

    private IEnumerator WaitAndDo(float time, System.Action action = null)
    {
        yield return new WaitForSeconds(time);
        if (action != null)
            action();
    }
}