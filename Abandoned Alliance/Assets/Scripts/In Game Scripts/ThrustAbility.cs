using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrustAbility : Ability
{
    protected override void toggleAbilityHighlights()
    {
        Tile currLoc = attachedHero.getCurrentPos();
            for(int i = -range; i < range+1; ++i)
            {
                for(int j = -range; j < range+1; ++j)
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

    public override bool UseAbility(Tile loc)
    {
        bool used = false;
        toggleAbilityHighlights();
        Tile currLoc = attachedHero.getCurrentPos();
        float dX = currLoc.getX() - loc.getX();
        float dY = currLoc.getY() - loc.getY();
        if((Mathf.Abs(dX) == 1 && dY == 0) || (dX == 0 && Mathf.Abs(dY) == 1) || (Mathf.Abs(dX) == 1 && Mathf.Abs(dY) == 1)) //is this a place the player could attack?
        {
            if(loc.getHero() != null) {used = true; loc.getHero().changeHealth(-1*damage);}

            if(Mathf.Abs(dX) == 0) //either left or right of the hero, centered
            {
                Tile reach = attachedHero.getMapManager().getPos(loc.getX(), loc.getY()-(int)dY);
                if(reach != null && reach.getHero() != null) {reach.getHero().changeHealth(-1*damage); used = true;}

            }
            else if(Mathf.Abs(dY) == 0) //either above or below the hero, centered
            {
                Tile reach = attachedHero.getMapManager().getPos(loc.getX()-(int)dX, loc.getY());
                if(reach != null && reach.getHero() != null) {reach.getHero().changeHealth(-1*damage); used = true;}

            }
            else //one of the four corners
            {
                Tile reach = attachedHero.getMapManager().getPos(loc.getX()-(int)dX, loc.getY()-(int)dY);
                if(reach != null && reach.getHero() != null) {reach.getHero().changeHealth(-1*damage); used = true;}

            }
        }
        if(used) {attachedHero.getMapManager().advTurn(cost); remainingCooldown = cooldown;}
        attachedHero.activeAbility = null;
        return used;
    }

    public override void mouseOver(Tile loc, bool addHighlight)
    {
        loc.toggleMouseHighlight(addHighlight);
        Tile currLoc = attachedHero.getCurrentPos();
        float dX = currLoc.getX() - loc.getX();
        float dY = currLoc.getY() - loc.getY();
        if((Mathf.Abs(dX) == 1 && dY == 0) || (dX == 0 && Mathf.Abs(dY) == 1) || (Mathf.Abs(dX) == 1 && Mathf.Abs(dY) == 1)) //is this a place the player could attack?
        {
            if(Mathf.Abs(dX) == 0) //either left or right of the hero, centered
            {
                Tile reach = attachedHero.getMapManager().getPos(loc.getX(), loc.getY()-(int)dY);
                if(reach != null) {reach.toggleMouseHighlight(addHighlight);}

            }
            else if(Mathf.Abs(dY) == 0) //either above or below the hero, centered
            {
                Tile reach = attachedHero.getMapManager().getPos(loc.getX()-(int)dX, loc.getY());
                if(reach != null) {reach.toggleMouseHighlight(addHighlight);}

            }
            else //one of the four corners
            {
                Tile reach = attachedHero.getMapManager().getPos(loc.getX()-(int)dX, loc.getY()-(int)dY);
                if(reach != null) {reach.toggleMouseHighlight(addHighlight);}

            }
        }
    }

    public override void init()
    {
        cost = 1;
        damage = (int)(1.5 * attachedHero.getDamage());
        range = 2;
        cooldown = 3;
        abilityName = "Thrust";
        transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{abilityName}- {cost} AP to use.";
    }
}
