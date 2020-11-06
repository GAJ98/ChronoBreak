﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


// TODO Change controls and stuff 
// TODO Add time features
// TODO Split the code into files perhaps

public class Timer : MonoBehaviour
{
    [SerializeField] GameObject timerPanel; // Panel showing any UI elements related to the timer 
    [SerializeField] Text timerText; // Timer label shown in UI

    [SerializeField] GameObject eventPanel; // Pop-up panel showing the timer interaction from an in-game event
    [SerializeField] Text eventText; // Label to display the event

    [SerializeField] GameObject timeSummaryPanel; // Panel shown right after the level completion
    [SerializeField] Text summaryText; // Label containing a summary of the events in level

    [SerializeField] GameObject levelFinishedPanel; // Panel shown when level is finished (after showing the time summary panel)
    [SerializeField] Text finalTimeText; // Timer label shown after the level has ended

    [SerializeField] GameObject dashIcon; // UI element for dash (cooldown)

    [SerializeField] GameObject pausePanel; // Panel shown when menu is paused

    // Time bonus and penalties
    // TODO add more fields
    [SerializeField] float dashTimePenalty = 3f;
    [SerializeField] float enemy1KilledTimeBonus = -2f; // Enemies that are standing still
    [SerializeField] float enemy2KilledTimeBonus = -2f; // Enemies that are throw projectiles
    [SerializeField] float enemy3KilledTimeBonus = -2f; // Enemies that chases the player in a pre-defined radius 
    [SerializeField] float enemy1TouchedTimePenalty = 5f;
    [SerializeField] float enemy2TouchedTimePenalty = 5f;
    [SerializeField] float enemy3TouchedTimePenalty = 5f;

    int dashCounter = 0;
    int enemy1KilledCounter = 0;
    int enemy1TouchedCounter = 0;
    int enemy2KilledCounter = 0;
    int enemy2TouchedCounter = 0; 
    int enemy3KilledCounter = 0;
    int enemy3TouchedCounter = 0;

    float timeElapsed; // Actual time elapsed since the level has started
    float totalTime; // Cumulative time since level has started
    int ms = 0; // Centiseconds
    int sec = 0; // Seconds
    int min = 0; // Minutes

    // Booleans for checking current state of the game
    bool isCompleted = false;
    bool hasStarted = false;
    bool isPaused = false;
    bool hasStopped = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        timeElapsed = 0; // Warning: scene may start with a cutscene of some sort so might need to adjust from actual starting time
        hasStarted = true;
        timerPanel.SetActive(true);
        eventPanel.SetActive(false);
        levelFinishedPanel.SetActive(false);
        pausePanel.SetActive(false);
        timeSummaryPanel.SetActive(false);
        dashIcon.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

        // Pause/resume controls
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!isPaused)
                Pause();
            else
                Resume();
        }

        // Timer update (will update while player still has not completed the level)
        if (!isCompleted && hasStarted)
        {
            timeElapsed += Time.deltaTime;
            ComputeTimer();
            SetTimerText();
        }

        // Action to do if level is completed
        if (isCompleted && !hasStopped)
        {
            // Change the action
            Time.timeScale = 0.05f;
            Invoke("LevelCompleted", 0.05f * 3);
            hasStopped = true;
        }

        // Restart level if restart button is pressed
        // TODO Change controls
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     ResetLevel();
        // }

    }

    // Compute total time and transform in minutes, seconds and centiseconds 
    void ComputeTimer()
    {
        // TODO Take into account time penalties and reductions
        totalTime = timeElapsed;
        // totalTime += ... (bunch of times)
        totalTime += dashCounter * dashTimePenalty;

        totalTime += enemy1KilledCounter * enemy1KilledTimeBonus;
        totalTime += enemy1TouchedCounter * enemy1TouchedTimePenalty;

        totalTime += enemy2KilledCounter * enemy2KilledTimeBonus;
        totalTime += enemy2TouchedCounter * enemy2TouchedTimePenalty;

        totalTime += enemy3KilledCounter * enemy3KilledTimeBonus;
        totalTime += enemy3TouchedCounter * enemy3TouchedTimePenalty;

        // Transform total time in to minutes, seconds and ms (2 digits)
        min = (int) (totalTime / 60);
        sec = (int) (totalTime - min * 60);
        ms = (int) ((totalTime - min * 60 - sec) * 100);
    }

    // Formats the time as "MM:SS.CC" and updates the timer label in UI
    void SetTimerText()
    {
        // Formatting
        string minutesText = min < 10 ? minutesText = "0" + min.ToString() : min.ToString();
        string secondsText = sec < 10 ? secondsText = "0" + sec.ToString() : sec.ToString();
        string msText = ms < 10 ? msText = "0" + ms.ToString() : ms.ToString();

        // Update timer text label
        timerText.text = minutesText + ":" + secondsText + "." + msText;
    }

    // Pause the game
    void Pause()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        pausePanel.SetActive(true);
    }

    // Resume the game
    void Resume()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        pausePanel.SetActive(false);
    }

    // Set game state to completed
    public void SetCompleted(bool completed)
    {
        isCompleted = completed;
    }

    // Reset level i.e reload the scene
    void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Show time summary when player reached goal
    void LevelCompleted()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        timerPanel.SetActive(false);
        pausePanel.SetActive(false);
        eventPanel.SetActive(false);
        dashIcon.SetActive(false);
        ShowTimeSummaryPanel();
    }

    // Shows a summary of the time penalties and bonus awarded during the level
    void ShowTimeSummaryPanel()
    {
        string summary;

        // Show elapsed time
        int minutes = (int)(timeElapsed / 60);
        int seconds = (int)(timeElapsed - minutes * 60);
        int centiseconds = (int)((timeElapsed - minutes * 60 - seconds) * 100);

        string minutesText = minutes < 10 ? minutesText = "0" + minutes.ToString() : minutes.ToString();
        string secondsText = seconds < 10 ? secondsText = "0" + seconds.ToString() : seconds.ToString();
        string msText = centiseconds < 10 ? msText = "0" + centiseconds.ToString() : centiseconds.ToString();

        summary = "Elapsed time -----> " + minutesText + ":" + secondsText + '.' + msText + "\n\n";

        // Show dash stats
        summary += "Dash: " + dashCounter.ToString() + " x " + dashTimePenalty.ToString() + " sec -----> " + (dashCounter * dashTimePenalty).ToString() + " sec\n\n";

        // Show standing enemy stats
        summary += "Standing enemies killed: " + enemy1KilledCounter.ToString() + " x " + enemy1KilledTimeBonus.ToString() + " sec -----> " + (enemy1KilledCounter * enemy1KilledTimeBonus).ToString() + " sec\n";
        summary += "Standing enemies touched: " + enemy1TouchedCounter.ToString() + " x " + enemy1TouchedTimePenalty.ToString() + " sec -----> " + (enemy1TouchedCounter * enemy1TouchedTimePenalty).ToString() + " sec\n\n";

        // Show shooting enemy stats
        summary += "Shooting enemies killed: " + enemy2KilledCounter.ToString() + " x " + enemy2KilledTimeBonus.ToString() + " sec -----> " + (enemy2KilledCounter * enemy2KilledTimeBonus).ToString() + " sec\n";
        summary += "Shot by enemies: " + enemy2TouchedCounter.ToString() + " x " + enemy2TouchedTimePenalty.ToString() + " sec -----> " + (enemy2TouchedCounter * enemy2TouchedTimePenalty).ToString() + " sec\n\n";

        // Show chasing enemy stats
        summary += "Chasing enemies killed: " + enemy3KilledCounter.ToString() + " x " + enemy3KilledTimeBonus.ToString() + " sec -----> " + (enemy3KilledCounter * enemy3KilledTimeBonus).ToString() + " sec\n";
        summary += "Chasing enemies touched: " + enemy3TouchedCounter.ToString() + " x " + enemy3TouchedTimePenalty.ToString() + " sec -----> " + (enemy3TouchedCounter * enemy3TouchedTimePenalty).ToString() + " sec\n\n";


        // Show final time
        summary += "Total time -----> " + timerText.text;

        // Display in UI
        timeSummaryPanel.SetActive(true);
        summaryText.text = summary;
    }

    // Increments the counter of a specified/passed event
    public void CountEvent(string ev)
    {
        switch (ev)
        {
            //TODO add falling off stage event
            case "Dash": 
                dashCounter++;
                PopUpEventPanel("Dash", dashTimePenalty);
                break;
            case "standing enemy kill":
                enemy1KilledCounter++;
                PopUpEventPanel("Standing enemy killed", enemy1KilledTimeBonus);
                break;
            case "standing enemy touched":
                enemy1TouchedCounter++;
                PopUpEventPanel("Standing enemy touched", enemy1TouchedTimePenalty);
                break;
            case "shooting enemy kill":
                enemy2KilledCounter++;
                PopUpEventPanel("Shotting enemy killed", enemy2KilledTimeBonus);
                break;
            case "shot by enemy":
                enemy2TouchedCounter++;
                PopUpEventPanel("Shot by enemy: ", enemy2TouchedTimePenalty);
                break;
            case "chasing enemy kill":
                enemy3KilledCounter++;
                PopUpEventPanel("Chasing enemy killed", enemy3KilledTimeBonus);
                break;
            case "chasing enemy touched":
                enemy3TouchedCounter++;
                PopUpEventPanel("Chasing enemy touched", enemy3TouchedTimePenalty);
                break;

        }
    }

    // Display the in-game event in UI and resulting timer interaction for 0.5 seconds
    // green: time reduction
    // red: time penalty
    void PopUpEventPanel(string ev, float timeIncrement)
    {
        eventText.text = ev + ": " + timeIncrement.ToString() + " sec.";
        eventText.color = timeIncrement > 0 ? Color.red : Color.green;
        eventPanel.SetActive(true);
        Invoke("DisableEventPanel", 0.5f);
    }

    // Helper function for diabling the even panel (useful for invoke function)
    private void DisableEventPanel()
    {
        eventPanel.SetActive(false);
    }

    // Button methods
    public void RestartLevel()
    {
        ResetLevel();
    }

    public void ResumeGame()
    {
        Resume();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ShowLevelEndMenu()
    {
        timeSummaryPanel.SetActive(false);
        levelFinishedPanel.SetActive(true);
        finalTimeText.text = "Final Time: " + timerText.text;
    }
}