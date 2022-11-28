using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Ability : MonoBehaviour
{
    protected int cost;
    protected int damage;
    protected int range;
    protected int cooldown;
    protected int remainingCooldown;
    public string abilityName;

    public Hero attachedHero;

    //Method to be called when ability is told to activate through a button or shortcut. Can be overriden if a specific ability needs to change this implementation.
    public virtual void toggleActive()
    {
        if(remainingCooldown == 0 && cost <= attachedHero.getAP())
        {
            if(attachedHero.activeAbility != null) {attachedHero.activeAbility.toggleAbilityHighlights();}
            if(attachedHero.activeAbility != this) {attachedHero.activeAbility = this;}
            else {attachedHero.activeAbility = null;}
            toggleAbilityHighlights();
        }
    }

    //Default behavior for Tile mouseovers, should be overriden for abilities with an area of effect.
    public virtual void mouseOver(Tile loc, bool addHighlight)
    {
        loc.toggleMouseHighlight(addHighlight);
    }

    //Method that is triggered when an ability is already active and the user clicks a tile. No implementation in the Abstract class
    //Because this script is entirely dependent on what the ability should be doing.
    public abstract bool UseAbility(Tile loc);

    //Private method used in toggleActive to indicate valid locations for an ability to be used.
    //No implementation in the Abstract as this is dependent on ability requirements and range.
    protected abstract void toggleAbilityHighlights();

    //Roughly equivelent to a constructor, this is called on Ability creation to set the properties of the generated Ability.
    //Some elements are generalizable, so a virtual method might be valid here, but it's fairly likely that each class will want to
    //Hard-code some values statically anyway (for example, Move should always deal 0 damage, as it can't even be used on an occupied tile.)
    public abstract void init();

    public virtual int getCost() {return cost;}
    public virtual void advCooldown(int amount)
    {
        if(remainingCooldown-amount <= 0)
        {
            transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{abilityName}- {cost} AP to use.";
            remainingCooldown = 0;
        } 
        else
        {
            remainingCooldown -= amount;
            transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{abilityName} cooling down: {remainingCooldown} AP until recovered.";
        }
    }
}
