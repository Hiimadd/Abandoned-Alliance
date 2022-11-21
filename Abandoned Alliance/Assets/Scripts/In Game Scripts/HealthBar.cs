using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider healthBar;
    [SerializeField] private Hero attachedHero;


    private void Start()
    {
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = attachedHero.getMaxHealth();
        healthBar.value = attachedHero.getMaxHealth();
    }

    public void Update()
    {
        healthBar.value = attachedHero.getHealth();
    }
}

//Code language: C# (cs)