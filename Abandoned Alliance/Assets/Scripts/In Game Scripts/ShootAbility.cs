using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootAbility : Ability
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
            loc.GetHero().ChangeHealth(-1*AttachedHero.GetDamage());
            AttachedHero.GetMapManager().AdvTurn(_cost);
            used = true;
        }
        AttachedHero.ActiveAbility = null;
        return used;
    }


    public override void Init()
    {
        _cost = 1;
        _damage = AttachedHero.GetDamage();
        _range = 8;
        _cooldown = 0;
        AbilityName = "Shoot Bow";
        transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{AbilityName}- {_cost} AP to use.";
    }
}
