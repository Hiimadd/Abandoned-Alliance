using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveAbility : Ability
{
    //Implementation of toggleAbilityHighlights for MoveAbility.
    //Checks to make sure the tiles are on the grid, of type 1 (walkable), and aren't already occupied.
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
                        if(toHighlight.GetHero() == null && toHighlight.GetTileType() == 1)
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

    //Implementation of useAbility for MoveAbility.
    //Checks distance of clicked tile from the Hero's current position, and moves
    //if the tile has a distance of 1, is open, and is walkable.
    public override bool UseAbility(Tile loc)
    {
        bool used = false;
        toggleAbilityHighlights();
        Tile currLoc = AttachedHero.GetCurrentPos();
        int dX = Mathf.Abs(currLoc.GetX() - loc.GetX());
        int dY = Mathf.Abs(currLoc.GetY() - loc.GetY());
        if(((dX == 1 && dY == 0) || (dX == 0 && dY == 1) || (dX == 1 && dY == 1)) && loc.GetHero() == null && loc.GetTileType() == 1)
        {
            AttachedHero.UpdatePosition(loc);
            AttachedHero.GetMapManager().AdvTurn(_cost);
            used = true;
        }
        AttachedHero.ActiveAbility = null;
        return used;
    }

    //Implementation of init for MoveAbility.
    //Just initializes the variables of the ability, not much to explain here.
    //Worth pointing out that range is currently unused for MoveAbility.
    public override void Init()
    {
        _cost = 1;
        _damage = 0;
        _range = AttachedHero.GetMoveSpeed();
        _cooldown = 0;
        AbilityName = "Move";
        transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{AbilityName}- {_cost} AP to use.";
    }

}
