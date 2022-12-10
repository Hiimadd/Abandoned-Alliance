using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpreadShotAbility : Ability
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
        float dX = currLoc.GetX() - loc.GetX();
        float dY = currLoc.GetY() - loc.GetY();
        if(Mathf.Sqrt(Mathf.Pow(dX, 2) + Mathf.Pow(dY, 2)) <= _range && Mathf.Sqrt(Mathf.Pow(dX, 2) + Mathf.Pow(dY, 2)) != 0)
        {
            if(Mathf.Abs(dX) == Mathf.Abs(dY)) //on a corner, the hit tiles should be a corner
            {
                Tile t1;
                if(dX > 0)
                {
                    t1 = AttachedHero.GetMapManager().GetPos(loc.GetX()+1, loc.GetY());
                }
                else
                {
                    t1 = AttachedHero.GetMapManager().GetPos(loc.GetX()-1, loc.GetY());
                }

                if(t1 != null)
                {
                    t1.ToggleMouseHighlight(addHighlight);
                }

                Tile t2;
                if(dY > 0)
                {
                    t2 = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()+1);
                }
                else
                {
                    t2 = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()-1);
                }

                if(t2 != null)
                {
                    t2.ToggleMouseHighlight(addHighlight);   
                }
            }
            else //not on a corner, the hit tiles should be in a line
            {
                for(int i = -1; i < 2; ++i)
                {
                    Tile t;
                    if(Mathf.Abs(dX) > Mathf.Abs(dY)) //attack should be a vertical line
                    {
                        t = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()+i);
                    }
                    else //dX < dY; attack should be a horizontal line
                    {
                        t = AttachedHero.GetMapManager().GetPos(loc.GetX()+i, loc.GetY());
                    }

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
        float dX = currLoc.GetX() - loc.GetX();
        float dY = currLoc.GetY() - loc.GetY();
        if(Mathf.Sqrt(Mathf.Pow(dX, 2) + Mathf.Pow(dY, 2)) <= _range && Mathf.Sqrt(Mathf.Pow(dX, 2) + Mathf.Pow(dY, 2)) != 0)
        {
            int validEnemies = 0;
            if(Mathf.Abs(dX) == Mathf.Abs(dY)) //on a corner, the hit tiles should be a corner
            {
                if(loc.GetHero() != null)
                {
                    ++validEnemies;
                    loc.GetHero().ChangeHealth(-1*_damage);
                }

                Tile t1;
                if(dX > 0)
                {
                    t1 = AttachedHero.GetMapManager().GetPos(loc.GetX()+1, loc.GetY());
                }
                else
                {
                    t1 = AttachedHero.GetMapManager().GetPos(loc.GetX()-1, loc.GetY());
                }

                if(t1 != null)
                {
                    Hero h1 = t1.GetHero();
                    if(h1 != null)
                    {
                        ++validEnemies;
                        h1.ChangeHealth(-1*_damage);
                    }
                }

                Tile t2;
                if(dY > 0)
                {
                    t2 = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()+1);
                }
                else
                {
                    t2 = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()-1);
                }

                if(t2 != null)
                {
                    Hero h2 = t2.GetHero();
                    if(h2 != null)
                    {
                        ++validEnemies;
                        h2.ChangeHealth(-1*_damage);
                    }
                }
            }
            else //not on a corner, the hit tiles should be in a line
            {
                for(int i = -1; i < 2; ++i)
                {
                    Tile t;
                    if(Mathf.Abs(dX) > Mathf.Abs(dY)) //attack should be a vertical line
                    {
                        t = AttachedHero.GetMapManager().GetPos(loc.GetX(), loc.GetY()+i);
                    }
                    else //dX < dY; attack should be a horizontal line
                    {
                        t = AttachedHero.GetMapManager().GetPos(loc.GetX()+i, loc.GetY());
                    }

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


    public override void Init()
    {
        _cost = 1;
        _damage = (int)(1.5*AttachedHero.GetDamage());
        _range = 4;
        _cooldown = 4;
        AbilityName = "Spread Shot";
        transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{AbilityName}- {_cost} AP to use.";
    }
}
