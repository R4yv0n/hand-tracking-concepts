using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RockPaperScissorScript : MonoBehaviour
{
    [Serializable]
    public enum Symbol
    {
        Rock = 0,
        Paper = 1,
        Scissor = 2,
        None = 3
    }

    public OVRHand hand;
    public Text gameStateText;
    public Text instructionsText;
    public Text countDownTimer;
    public SpriteRenderer rpsImage;
    public Sprite[] sprites;

    private Symbol foundGesture;
    private Symbol npcSymbol;
    
    private Boolean timerIsRunning = false;
    private float timeRemaining = 6f;
    
    private Boolean gameReseted = true;

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                countDownTimer.text = Mathf.FloorToInt(timeRemaining % 60).ToString();
                timeRemaining -= Time.deltaTime;
                gameReseted = false;
            }
            else
            {
                CheckForWin();
                timerIsRunning = false;
            }
        }
    }

    public void CheckForWin()
    {
        npcSymbol = (Symbol) Random.Range(0, 2);
        gameStateText.text = "Npc's choice:" + npcSymbol;
        rpsImage.sprite = sprites[(int)npcSymbol];
        
        if (foundGesture == Symbol.None)
        {
            instructionsText.text = "No symbol detected/ selected";
            return;
        }
        if (foundGesture == npcSymbol)
        {
            instructionsText.text = "Draw.";
            return;
        }
        var winText = "You Win!";
        var looseText = "You loose...";
        switch (foundGesture)
        {
            case Symbol.Paper:
                instructionsText.text = npcSymbol == Symbol.Rock ? winText : looseText;
                break;
            case Symbol.Scissor:
                instructionsText.text = npcSymbol == Symbol.Paper ? winText : looseText;
                break;
            case Symbol.Rock:
                instructionsText.text = npcSymbol == Symbol.Scissor ? winText : looseText;
                break;
        }
    }

    public void SetGesture(int gesture)
    {
        foundGesture = (Symbol) gesture;
    }
    
    public void ResetGame()
    {
        timeRemaining = 6f;
        countDownTimer.text = "5";
        timerIsRunning = false;
        foundGesture = Symbol.None;
        instructionsText.text = "Press Start-button";
        gameReseted = true;
        rpsImage.sprite = sprites[3];
    }

    public void StartGame()
    {
        if (gameReseted)
        {
            timerIsRunning = true;
        }
        else
        {
            instructionsText.text = "Reset the Game before starting a new round";
        }
    }
}
