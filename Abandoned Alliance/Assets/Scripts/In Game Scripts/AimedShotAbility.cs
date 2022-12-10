using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimedShotAbility : Ability
{
    protected override void toggleAbilityHighlights()
    {
        Tile currLoc = AttachedHero.GetCurrentPos();
            for(int i = -_range; i < _range+1; ++i)
            {
                for(int j = -_range; j < _range+1; ++j)
                {
                    if(j == 0 && i == 0)
                    {
                        continue;
                    }
                    Tile toHighlight = AttachedHero.GetMapManager().GetPos(currLoc.GetX() + i, currLoc.GetY() +j);
                    if(toHighlight != null) 
                    {
                        if(toHighlight.GetHero() != null && toHighlight.GetTileType() == 1)
                        {
                            toHighlight.ToggleAbilityHighlight();
                        }
                        else
                        {
                            toHighlight.ToggleInvalidAbilityHighlight();
                        }
                    }
                }
            }
    }


    public override bool UseAbility(Tile loc)
    {
        bool used = false;
        toggleAbilityHighlights();
        Tile currLoc = AttachedHero.GetCurrentPos();
        float dX = Mathf.Abs(currLoc.GetX() - loc.GetX());
        float dY = Mathf.Abs(currLoc.GetY() - loc.GetY());
        if(loc.GetHero() != null && Mathf.Sqrt(Mathf.Pow(dX, 2) + Mathf.Pow(dY, 2)) <= _range)
        {
            loc.GetHero().ChangeHealth(-1*_damage);
            _remainingCooldown = _cooldown;
            AttachedHero.GetMapManager().AdvTurn(_cost);
            used = true;
        }
        AttachedHero.ActiveAbility = null;
        return used;
    }


    public override void Init()
    {
        _cost = 1;
        _damage = (int)(2*AttachedHero.GetDamage());
        _range = 12;
        _cooldown = 4;
        AbilityName = "Aimed Shot";
        transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{AbilityName}- {_cost} AP to use.";
    }
}
