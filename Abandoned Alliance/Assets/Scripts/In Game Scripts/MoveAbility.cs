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
        Tile currLoc = attachedHero.getCurrentPos();
            for(int i = -1; i < 2; ++i)
            {
                for(int j = -1; j < 2; ++j)
                {
                    if(j == 0 && i == 0) {continue;}
                    Tile toHighlight = attachedHero.getMapManager().getPos(currLoc.getX() + i, currLoc.getY() +j);
                    if(toHighlight != null)
                    {
                        if(toHighlight.getHero() == null && toHighlight.getType() == 1)
                        {
                            toHighlight.toggleAbilityHighlight();
                        }
                        else
                        {
                            toHighlight.toggleInvalidAbilityHighlight();
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
        Tile currLoc = attachedHero.getCurrentPos();
        int dX = Mathf.Abs(currLoc.getX() - loc.getX());
        int dY = Mathf.Abs(currLoc.getY() - loc.getY());
        if(((dX == 1 && dY == 0) || (dX == 0 && dY == 1) || (dX == 1 && dY == 1)) && loc.getHero() == null && loc.getType() == 1)
        {
            attachedHero.UpdatePosition(loc);
            attachedHero.getMapManager().advTurn(cost);
            used = true;
        }
        attachedHero.activeAbility = null;
        return used;
    }

    //Implementation of init for MoveAbility.
    //Just initializes the variables of the ability, not much to explain here.
    //Worth pointing out that range is currently unused for MoveAbility.
    public override void init()
    {
        cost = 1;
        damage = 0;
        range = attachedHero.getMoveSpeed();
        cooldown = 0;
        abilityName = "Move";
        transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{abilityName}- {cost} AP to use.";
    }

}
