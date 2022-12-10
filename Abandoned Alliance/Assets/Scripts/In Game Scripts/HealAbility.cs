using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealAbility : Ability
{
    protected override void toggleAbilityHighlights()
    {
        Tile currLoc = AttachedHero.GetCurrentPos();
            for(int i = -_range; i < _range+1; ++i)
            {
                for(int j = -_range; j < _range+1; ++j)
                {
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
        int dX = Mathf.Abs(currLoc.GetX() - loc.GetX());
        int dY = Mathf.Abs(currLoc.GetY() - loc.GetY());
        if(Mathf.Sqrt(Mathf.Pow(dX, 2) + Mathf.Pow(dY, 2)) <= _range && loc.GetHero() != null)
        {
            loc.GetHero().ChangeHealth(_damage);
            AttachedHero.GetMapManager().AdvTurn(_cost);
            _remainingCooldown = _cooldown;
            used = true;
        }
        AttachedHero.ActiveAbility = null;
        return used;
    }

    public override void Init()
    {
        _cost = 1;
        _damage = 5;
        _range = 2;
        _cooldown = 6;
        AbilityName = "Move";
        transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{AbilityName}- {_cost} AP to use.";
    }

}
