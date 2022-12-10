using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireballAbility : Ability
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

    public override void MouseOver(Tile loc, bool addHighlight)
    {
        loc.ToggleMouseHighlight(addHighlight);
        Tile currLoc = AttachedHero.GetCurrentPos();
        float dX = Mathf.Abs(currLoc.GetX() - loc.GetX());
        float dY = Mathf.Abs(currLoc.GetY() - loc.GetY());
        if(Mathf.Sqrt(Mathf.Pow(dX, 2) + Mathf.Pow(dY, 2)) <= _range) //is this a place the player could attack?
        {
            for(int i = -2; i < 3; ++i)
            {
                for(int j = -2; j < 3; ++j)
                {
                    if(i == 0 && j == 0)
                    {
                        continue;
                    }

                    Tile t = AttachedHero.GetMapManager().GetPos(loc.GetX()+i, loc.GetY()+j);
                    if(t != null)
                    {
                        t.ToggleMouseHighlight(addHighlight);
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
        if(Mathf.Sqrt(Mathf.Pow(dX, 2) + Mathf.Pow(dY, 2)) <= _range)
        {
            int validEnemies = 0;
            for(int i = -2; i < 3; ++i)
            {
                for(int j = -2; j < 3; ++j)
                {
                    Tile t = AttachedHero.GetMapManager().GetPos(loc.GetX()+i, loc.GetY()+j);
                    if(t != null)
                    {
                        Hero h = t.GetHero();
                        if(h != null)
                        {
                            ++validEnemies;
                            h.ChangeHealth(-1*_damage);
                        }
                    }
                }
            }

            if(validEnemies > 0)
            {
                AttachedHero.GetMapManager().AdvTurn(_cost);
                _remainingCooldown = _cooldown;
                used = true;
            }
        }
        AttachedHero.ActiveAbility = null;
        return used;
    }


    public override void Init()
    {
        _cost = 3;
        _damage = 2*AttachedHero.GetDamage();
        _range = 8;
        _cooldown = 6;
        AbilityName = "Fireball";
        transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{AbilityName}- {_cost} AP to use.";
    }
}
