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

    public virtual void toggleActive()
    {
        if(remainingCooldown == 0)
        {
            if(attachedHero.activeAbility != null) {attachedHero.activeAbility.toggleAbilityHighlights();}
            attachedHero.activeAbility = this;
            toggleAbilityHighlights();
        }
    }

    public abstract void UseAbility(Tile loc);
    protected abstract void toggleAbilityHighlights();
    public abstract void init();
}
