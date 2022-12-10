using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider _healthBar;
    [SerializeField] private Hero _attachedHero;


    private void Start()
    {
        _healthBar = GetComponent<Slider>();
        _healthBar.maxValue = _attachedHero.GetMaxHealth();
        _healthBar.value = _attachedHero.GetMaxHealth();
    }

    public void Update()
    {
        _healthBar.value = _attachedHero.GetHealth();
    }
}

//Code language: C# (cs)