using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperiencePoints : MonoBehaviour
{
    public Image image;
    private float currentExp;
    public int Level { get; set; }
    public float MaxExp { get; set; }
    public float CurrentExp 
    { 
        get => currentExp; 
        set
        {
            if (value > MaxExp)
            {
                currentExp = 0F;
                Level += 1;
                MaxExp += 100F * Level;
                UpdateLevel();
            }
            else
            {
                currentExp = value;
            }
        }
    }
    private void Start()
    {
        //UpdateBar();
    }

    public void AddPoints(int points)
    {
        CurrentExp += points;
        //UpdateBar();
    }

    public void UpdateLevel()
    {
        GetComponent<Player>().cameraInterface.level.text = $"Lv {Level}";
    }

    public void UpdateBar()
    {
        image.fillAmount = CurrentExp / MaxExp;
    }
}