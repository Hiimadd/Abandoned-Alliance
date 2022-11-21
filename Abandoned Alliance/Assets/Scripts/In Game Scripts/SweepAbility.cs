using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepAbility : Ability
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

    public override void mouseOver(Tile loc)
    {
        loc.toggleMouseHighlight();
        Tile currLoc = attachedHero.getCurrentPos();
        float dX = currLoc.getX() - loc.getX();
        float dY = currLoc.getY() - loc.getY();
        if((Mathf.Abs(dX) == 1 && dY == 0) || (dX == 0 && Mathf.Abs(dY) == 1) || (Mathf.Abs(dX) == 1 && Mathf.Abs(dY) == 1)) //is this a place the player could attack?
        {
            if(Mathf.Abs(dX) == 0) //either left or right of the hero, centered
            {
                Tile down = attachedHero.getMapManager().getPos(loc.getX(), loc.getY()-1);
                if(down != null) {down.toggleMouseHighlight();}

                Tile up = attachedHero.getMapManager().getPos(loc.getX(), loc.getY()+1);
                if(up != null) {up.toggleMouseHighlight();}
            }
            else if(Mathf.Abs(dY) == 0) //either above or below the hero, centered
            {
                Tile left = attachedHero.getMapManager().getPos(loc.getX()-1, loc.getY());
                if(left != null) {left.toggleMouseHighlight();}

                Tile right = attachedHero.getMapManager().getPos(loc.getX()+1, loc.getY());
                if(right != null) {right.toggleMouseHighlight();}
            }
            else //one of the four corners
            {
                Tile s1 = attachedHero.getMapManager().getPos(loc.getX()-(int)dX, loc.getY());
                if(s1 != null) {s1.toggleMouseHighlight();}

                Tile s2 = attachedHero.getMapManager().getPos(loc.getX(), loc.getY()-(int)dY);
                if(s2 != null) {s2.toggleMouseHighlight();}
            }
        }
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
