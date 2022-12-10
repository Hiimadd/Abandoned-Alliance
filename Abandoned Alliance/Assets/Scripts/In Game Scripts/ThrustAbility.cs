using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrustAbility : Ability
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
        float dX = currLoc.GetX() - loc.GetX();
        float dY = currLoc.GetY() - loc.GetY();
        if((Mathf.Abs(dX) == 1 && dY == 0) || (dX == 0 && Mathf.Abs(dY) == 1) || (Mathf.Abs(dX) == 1 && Mathf.Abs(dY) == 1)) //is this a place the player could attack?
        {
            if(loc.GetHero() != null)
            {
                used = true;
                loc.GetHero().ChangeHealth(-1*_damage);
            }

            if(Mathf.Abs(dX) == 0) //either left or right of the hero, centered
            {
                Tile reach = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()-(int)dY);
                if(reach != null && reach.GetHero() != null)
                {
                    reach.GetHero().ChangeHealth(-1*_damage);
                    used = true;
                }

            }
            else if(Mathf.Abs(dY) == 0) //either above or below the hero, centered
            {
                Tile reach = AttachedHero.GetMapManager().GetPos(loc.GetX()-(int)dX, loc.GetY());
                if(reach != null && reach.GetHero() != null)
                {
                    reach.GetHero().ChangeHealth(-1*_damage);
                    used = true;
                }

            }
            else //one of the four corners
            {
                Tile reach = AttachedHero.GetMapManager().GetPos(loc.GetX()-(int)dX, loc.GetY()-(int)dY);
                if(reach != null && reach.GetHero() != null)
                {
                    reach.GetHero().ChangeHealth(-1*_damage);
                    used = true;
                }

            }
        }
        if(used)
        {
            AttachedHero.GetMapManager().AdvTurn(_cost);
            _remainingCooldown = _cooldown;
        }
        AttachedHero.ActiveAbility = null;
        return used;
    }

    public override void MouseOver(Tile loc, bool addHighlight)
    {
        loc.ToggleMouseHighlight(addHighlight);
        Tile currLoc = AttachedHero.GetCurrentPos();
        float dX = currLoc.GetX() - loc.GetX();
        float dY = currLoc.GetY() - loc.GetY();
        if((Mathf.Abs(dX) == 1 && dY == 0) || (dX == 0 && Mathf.Abs(dY) == 1) || (Mathf.Abs(dX) == 1 && Mathf.Abs(dY) == 1)) //is this a place the player could attack?
        {
            if(Mathf.Abs(dX) == 0) //either left or right of the hero, centered
            {
                Tile reach = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()-(int)dY);
                if(reach != null)
                {
                    reach.ToggleMouseHighlight(addHighlight);
                }

            }
            else if(Mathf.Abs(dY) == 0) //either above or below the hero, centered
            {
                Tile reach = AttachedHero.GetMapManager().GetPos(loc.GetX()-(int)dX, loc.GetY());
                if(reach != null)
                {
                    reach.ToggleMouseHighlight(addHighlight);
                }

            }
            else //one of the four corners
            {
                Tile reach = AttachedHero.GetMapManager().GetPos(loc.GetX()-(int)dX, loc.GetY()-(int)dY);
                if(reach != null)
                {
                    reach.ToggleMouseHighlight(addHighlight);
                }

            }
        }
    }

    public override void Init()
    {
        _cost = 1;
        _damage = (int)(1.5 * AttachedHero.GetDamage());
        _range = 2;
        _cooldown = 3;
        AbilityName = "Thrust";
        transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{AbilityName}- {_cost} AP to use.";
    }
}
