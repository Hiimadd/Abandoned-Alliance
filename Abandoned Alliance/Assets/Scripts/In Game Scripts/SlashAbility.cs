using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlashAbility : Ability
{
    //Implementation of toggleAbilityHighlights for SlashAbility.
    //Checks to make sure that the tile is occupied and walkable, as a unit shouldn't be able to strike at a target they can't be on the same level as.
    //(For example, there could be archers on unwalkable tiles to simulate being on elevated terrain, in which case a sword wouldn't be able to reach them.)
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

    //Implementation of useAbility for SlashAbility.
    //Checks to make sure the attack is in range (distance 1 from the Hero)
    //And that there is actually a unit at the clicked tile to make sure the ability can't attack empty space.
    public override bool UseAbility(Tile loc)
    {
        bool used = false;
        toggleAbilityHighlights();
        Tile currLoc = AttachedHero.GetCurrentPos();
        float dX = Mathf.Abs(currLoc.GetX() - loc.GetX());
        float dY = Mathf.Abs(currLoc.GetY() - loc.GetY());
        if(loc.GetHero() != null && ((dX == 1 && dY == 0) || (dX == 0 && dY == 1) || (dX == 1 && dY == 1)))
        {
            loc.GetHero().ChangeHealth(-1*AttachedHero.GetDamage());
            AttachedHero.GetMapManager().AdvTurn(_cost);
            used = true;
        }
        AttachedHero.ActiveAbility = null;
        return used;
    }

    //Implementation of init for SlashAbility.
    //Slash, as a basic attack, has a cost of 1 and a range of 1, to differentiate it from more specialized abilities
    //that may have greater range or damage at the expense of a higher AP cost or a longer cooldown.
    public override void Init()
    {
        _cost = 1;
        _damage = AttachedHero.GetDamage();
        _range = 1;
        _cooldown = 0;
        AbilityName = "Slash";
        transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{AbilityName}- {_cost} AP to use.";
    }
}
