using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbility : Ability
{
    protected override void toggleAbilityHighlights()
    {
        Tile currLoc = attachedHero.getCurrentPos();
            for(int i = -1; i < 2; ++i)
            {
                for(int j = -1; j < 2; ++j)
                {
                    if(j == 0 && i == 0) {continue;}
                    Tile toHighlight = attachedHero.getMapManager().getPos(currLoc.getX() + i, currLoc.getY() +j);
                    if(toHighlight != null) {toHighlight.toggleAbilityHighlight();}
                }
            }
    }

    public override void UseAbility(Tile loc)
    {
        toggleAbilityHighlights();
        Tile currLoc = attachedHero.getCurrentPos();
        int dX = Mathf.Abs(currLoc.getX() - loc.getX());
        int dY = Mathf.Abs(currLoc.getY() - loc.getY());
        if(((dX == 1 && dY == 0) || (dX == 0 && dY == 1) || (dX == 1 && dY == 1)) && loc.getHero() == null && loc.getType() == 1)
        {
            attachedHero.updatePosition(loc);
            attachedHero.getMapManager().advTurn(cost);
        }
        attachedHero.activeAbility = null;
    }

    public void init()
    {
        cost = 1;
        damage = 0;
        range = attachedHero.getMoveSpeed();
        cooldown = 0;
        abilityName = "Move";
    }

}
