using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    protected int cost;
    protected int damage;
    protected int range;
    protected int cooldown;
    protected int remainingCooldown;
    public string abilityName;

    public Hero attachedHero;

    public void toggleActive()
    {
        if(remainingCooldown == 0)
        {
            attachedHero.activeAbility = this;
            Debug.Log("Ability Activated: " + abilityName + ".");
        }
    }

    public abstract void UseAbility(int X, int Y);
}
