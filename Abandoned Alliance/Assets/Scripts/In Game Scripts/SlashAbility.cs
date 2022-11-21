using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAbility : Ability
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
                    if(toHighlight != null) 
                    {
                        if(toHighlight.getHero() != null && toHighlight.getType() == 1)
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

    public override void UseAbility(Tile loc)
    {
        toggleAbilityHighlights();
        Tile currLoc = attachedHero.getCurrentPos();
        float dX = Mathf.Abs(currLoc.getX() - loc.getX());
        float dY = Mathf.Abs(currLoc.getY() - loc.getY());
        if(loc.getHero() != null && ((dX == 1 && dY == 0) || (dX == 0 && dY == 1) || (dX == 1 && dY == 1)))
        {
            loc.getHero().changeHealth(-1*attachedHero.getDamage());
            attachedHero.getMapManager().advTurn(cost);
        }
        attachedHero.activeAbility = null;
    }

    public override void init()
    {
        cost = 1;
        damage = attachedHero.getDamage();
        range = 1;
        cooldown = 0;
        abilityName = "Slash";
    }
}
