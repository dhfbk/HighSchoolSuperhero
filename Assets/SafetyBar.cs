using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataUtilities;
public class SafetyBar : MonoBehaviour
{
    [SerializeField]
    public float speed;
    public float Speed { get => speed; set => speed = value; }

    public static bool timeAttack;
    public static bool TimeAttack { get => timeAttack; set => timeAttack = value; }

    public static float SafetyMax { get; set; } = 1000.0f;

    [SerializeField]
    private static float currentSafety;
    public static float CurrentSafety
    {
        get => currentSafety;
        set
        {
            if (value > SafetyMax)
                currentSafety = SafetyMax;
            else if (value <= 0)
            {
                currentSafety = 0;
                
            }
            else
                currentSafety = value;
        }
    }
    
    private void Start()
    {
        Speed = Speed == 0 ? 0.2f : Speed;
    }
    private void Update()
    {
        if (TimeAttack)
        {
            CurrentSafety -= Time.deltaTime*Speed;
            GetComponent<Image>().fillAmount = CurrentSafety / SafetyMax;
        }
        if (!Player.gameOverCalled)
        {
            if (LoadUtility.AllLoaded == true && CurrentSafety <= 0)
            {
                FindObjectOfType<Player>().GameOver();
                Player.gameOverCalled = true;
            }
        }
    }

    public static void AddSafety(float score)
    {
        CurrentSafety += score;
    }
}
