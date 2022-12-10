using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Ability : MonoBehaviour
{
    protected int _cost;
    protected int _damage;
    protected int _range;
    protected int _cooldown;
    protected int _remainingCooldown;
    public string AbilityName;

    public Hero AttachedHero;

    //Method to be called when ability is told to activate through a button or shortcut. Can be overriden if a specific ability needs to change this implementation.
    public virtual void ToggleActive()
    {
        if(_remainingCooldown == 0 && _cost <= AttachedHero.GetAP())
        {
            if(AttachedHero.ActiveAbility != null)
            {
                AttachedHero.ActiveAbility.toggleAbilityHighlights();
            }
            if(AttachedHero.ActiveAbility != this)
            {
                AttachedHero.ActiveAbility = this;
            }
            else
            {
                AttachedHero.ActiveAbility = null;
            }
            toggleAbilityHighlights();
        }
    }

    //Default behavior for Tile mouseovers, should be overriden for abilities with an area of effect.
    public virtual void MouseOver(Tile loc, bool addHighlight)
    {
        loc.ToggleMouseHighlight(addHighlight);
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
    public abstract void Init();

    public virtual int GetCost() {return _cost;}
    public virtual void AdvCooldown(int amount)
    {
        if(_remainingCooldown-amount <= 0)
        {
            transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{AbilityName}- {_cost} AP to use.";
            _remainingCooldown = 0;
        } 
        else
        {
            _remainingCooldown -= amount;
            transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{AbilityName} cooling down: {_remainingCooldown} AP until recovered.";
        }
    }
}
