using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbility : Ability
{
    public override void UseAbility(int X, int Y)
    {
        //placeholder
        attachedHero.updatePosition(X, Y);
    }

    public MoveAbility()
    {
        cost = 1;
        damage = 0;
        range = attachedHero.getMoveSpeed();
        cooldown = 0;
        abilityName = "Move";
    }

}
