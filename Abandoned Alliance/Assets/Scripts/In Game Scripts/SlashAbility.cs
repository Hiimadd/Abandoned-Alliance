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

    //Implementation of useAbility for SlashAbility.
    //Checks to make sure the attack is in range (distance 1 from the Hero)
    //And that there is actually a unit at the clicked tile to make sure the ability can't attack empty space.
    public override bool UseAbility(Tile loc)
    {
        bool used = false;
        toggleAbilityHighlights();
        Tile currLoc = attachedHero.getCurrentPos();
        float dX = Mathf.Abs(currLoc.getX() - loc.getX());
        float dY = Mathf.Abs(currLoc.getY() - loc.getY());
        if(loc.getHero() != null && ((dX == 1 && dY == 0) || (dX == 0 && dY == 1) || (dX == 1 && dY == 1)))
        {
            loc.getHero().changeHealth(-1*attachedHero.getDamage());
            attachedHero.getMapManager().advTurn(cost);
            used = true;
        }
        attachedHero.activeAbility = null;
        return used;
    }

    //Implementation of init for SlashAbility.
    //Slash, as a basic attack, has a cost of 1 and a range of 1, to differentiate it from more specialized abilities
    //that may have greater range or damage at the expense of a higher AP cost or a longer cooldown.
    public override void init()
    {
        cost = 1;
        damage = attachedHero.getDamage();
        range = 1;
        cooldown = 0;
        abilityName = "Slash";
        transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{abilityName}- {cost} AP to use.";
    }
}
