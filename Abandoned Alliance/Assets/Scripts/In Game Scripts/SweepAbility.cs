using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//NOTE: This entire ability is WIP and not implemented yet.
//It is the first AOE-style attack and is not fully functional.
public class SweepAbility : Ability
{
    //functions identically to SlashAbility, as it has the same range and target parameters.
    protected override void toggleAbilityHighlights()
    {
        Tile currLoc = AttachedHero.GetCurrentPos();
            for(int i = -1; i < 2; ++i)
            {
                for(int j = -1; j < 2; ++j)
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

    //Attacks grouped targets around the hero, in a line if next to the hero or around a corner if on an edge.
    public override bool UseAbility(Tile loc)
    {
        bool used = false;
        toggleAbilityHighlights();
        Tile currLoc = AttachedHero.GetCurrentPos();
        float dX = currLoc.GetX() - loc.GetX();
        float dY = currLoc.GetY() - loc.GetY();
        if((Mathf.Abs(dX) == 1 && dY == 0) || (dX == 0 && Mathf.Abs(dY) == 1) || (Mathf.Abs(dX) == 1 && Mathf.Abs(dY) == 1)) //is this a place the player could attack?
        {
            int validEnemies = 0;
            if(loc.GetHero() != null)
            {
                ++validEnemies;
                loc.GetHero().ChangeHealth(-1*_damage);
            } 

            if(Mathf.Abs(dX) == 0) //either above or below the hero, centered
            {
                Tile left = AttachedHero.GetMapManager().GetPos(loc.GetX()-1, loc.GetY());
                if(left != null && left.GetHero() != null)
                {
                    ++validEnemies;
                    left.GetHero().ChangeHealth(-1*_damage);
                }

                Tile right = AttachedHero.GetMapManager().GetPos(loc.GetX()+1, loc.GetY());
                if(right != null && right.GetHero() != null)
                {
                    ++validEnemies;
                    right.GetHero().ChangeHealth(-1*_damage);
                }
            }
            else if(Mathf.Abs(dY) == 0) //either left or right of the hero, centered
            {
                Tile down = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()-1);
                if(down != null && down.GetHero() != null)
                {
                    ++validEnemies;
                    down.GetHero().ChangeHealth(-1*_damage);
                }

                Tile up = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()+1);
                if(up != null && up.GetHero() != null)
                {
                    ++validEnemies;
                    up.GetHero().ChangeHealth(-1*_damage);
                }
            }
            else //one of the four corners
            {
                Tile s1 = AttachedHero.GetMapManager().GetPos(loc.GetX()+(int)dX, loc.GetY());
                if(s1 != null && s1.GetHero() != null)
                {
                    ++validEnemies;
                    s1.GetHero().ChangeHealth(-1*_damage);
                }

                Tile s2 = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()+(int)dY);
                if(s2 != null && s2.GetHero() != null)
                {
                    ++validEnemies;
                    s2.GetHero().ChangeHealth(-1*_damage);
                }
            }

            if(validEnemies != 0)
            {
                AttachedHero.GetMapManager().AdvTurn(_cost);
                _remainingCooldown = _cooldown;
                used = true;
            }
        }
        AttachedHero.ActiveAbility = null;
        return used;
    }

    //mouseOver logic to enable highlighting area-of-effect for selecting a certain tile.
    //In this case, it's the tile clicked and the two tiles in range touching it.
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
                Tile down = AttachedHero.GetMapManager().GetPos(loc.GetX()-1, loc.GetY());
                if(down != null)
                {
                    down.ToggleMouseHighlight(addHighlight);
                }

                Tile up = AttachedHero.GetMapManager().GetPos(loc.GetX()+1, loc.GetY());
                if(up != null)
                {
                    up.ToggleMouseHighlight(addHighlight);
                }
            }
            else if(Mathf.Abs(dY) == 0) //either above or below the hero, centered
            {
                Tile left = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()-1);
                if(left != null)
                {
                    left.ToggleMouseHighlight(addHighlight);
                }

                Tile right = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()+1);
                if(right != null)
                {
                    right.ToggleMouseHighlight(addHighlight);
                }
            }
            else //one of the four corners
            {
                Tile s1 = AttachedHero.GetMapManager().GetPos(loc.GetX()+(int)dX, loc.GetY());
                if(s1 != null)
                {
                    s1.ToggleMouseHighlight(addHighlight);
                }

                Tile s2 = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()+(int)dY);
                if(s2 != null)
                {
                    s2.ToggleMouseHighlight(addHighlight);
                }
            }
        }
    }

    //Paramaters still need to be tuned,
    //Should have a cooldown and higher AP cost than slash.
    public override void Init()
    {
        _cost = 2;
        _damage = AttachedHero.GetDamage();
        _range = 1;
        _cooldown = 3;
        AbilityName = "Sweep";
        transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{AbilityName}- {_cost} AP to use.";
    }
}
