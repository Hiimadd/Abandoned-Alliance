using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAbility : Ability
{

    public override void UseAbility(int X, int Y)
    {
        
    }

    public SlashAbility()
    {
        cost = 1;
        damage = attachedHero.getDamage();
        range = 1;
        cooldown = 0;
        abilityName = "Slash";
    }
}
