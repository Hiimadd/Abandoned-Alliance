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

    //Attacks grouped targets around the hero, in a line if next to the hero or around a corner if on an edge.
    public override bool UseAbility(Tile loc)
    {
        bool used = false;
        toggleAbilityHighlights();
        Tile currLoc = attachedHero.getCurrentPos();
        float dX = currLoc.getX() - loc.getX();
        float dY = currLoc.getY() - loc.getY();
        if((Mathf.Abs(dX) == 1 && dY == 0) || (dX == 0 && Mathf.Abs(dY) == 1) || (Mathf.Abs(dX) == 1 && Mathf.Abs(dY) == 1)) //is this a place the player could attack?
        {
            int validEnemies = 0;
            if(loc.getHero() != null) {++validEnemies; loc.getHero().changeHealth(-1*damage);} 
            if(Mathf.Abs(dX) == 0) //either above or below the hero, centered
            {
                Tile left = attachedHero.getMapManager().getPos(loc.getX()-1, loc.getY());
                if(left != null && left.getHero() != null) {++validEnemies; left.getHero().changeHealth(-1*damage);}

                Tile right = attachedHero.getMapManager().getPos(loc.getX()+1, loc.getY());
                if(right != null && right.getHero() != null) {++validEnemies; right.getHero().changeHealth(-1*damage);}
            }
            else if(Mathf.Abs(dY) == 0) //either left or right of the hero, centered
            {
                Tile down = attachedHero.getMapManager().getPos(loc.getX(), loc.getY()-1);
                if(down != null && down.getHero() != null) {++validEnemies; down.getHero().changeHealth(-1*damage);}

                Tile up = attachedHero.getMapManager().getPos(loc.getX(), loc.getY()+1);
                if(up != null && up.getHero() != null) {++validEnemies; up.getHero().changeHealth(-1*damage);}
            }
            else //one of the four corners
            {
                Tile s1 = attachedHero.getMapManager().getPos(loc.getX()+(int)dX, loc.getY());
                if(s1 != null && s1.getHero() != null) {++validEnemies; s1.getHero().changeHealth(-1*damage);}

                Tile s2 = attachedHero.getMapManager().getPos(loc.getX(), loc.getY()+(int)dY);
                if(s2 != null && s2.getHero() != null) {++validEnemies; s2.getHero().changeHealth(-1*damage);}
            }

            if(validEnemies != 0) {attachedHero.getMapManager().advTurn(cost); remainingCooldown = cooldown; used = true;}
        }
        attachedHero.activeAbility = null;
        return used;
    }

    //mouseOver logic to enable highlighting area-of-effect for selecting a certain tile.
    //In this case, it's the tile clicked and the two tiles in range touching it.
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
                Tile down = attachedHero.getMapManager().getPos(loc.getX()-1, loc.getY());
                if(down != null) {down.toggleMouseHighlight(addHighlight);}

                Tile up = attachedHero.getMapManager().getPos(loc.getX()+1, loc.getY());
                if(up != null) {up.toggleMouseHighlight(addHighlight);}
            }
            else if(Mathf.Abs(dY) == 0) //either above or below the hero, centered
            {
                Tile left = attachedHero.getMapManager().getPos(loc.getX(), loc.getY()-1);
                if(left != null) {left.toggleMouseHighlight(addHighlight);}

                Tile right = attachedHero.getMapManager().getPos(loc.getX(), loc.getY()+1);
                if(right != null) {right.toggleMouseHighlight(addHighlight);}
            }
            else //one of the four corners
            {
                Tile s1 = attachedHero.getMapManager().getPos(loc.getX()+(int)dX, loc.getY());
                if(s1 != null) {s1.toggleMouseHighlight(addHighlight);}

                Tile s2 = attachedHero.getMapManager().getPos(loc.getX(), loc.getY()+(int)dY);
                if(s2 != null) {s2.toggleMouseHighlight(addHighlight);}
            }
        }
    }

    //Paramaters still need to be tuned,
    //Should have a cooldown and higher AP cost than slash.
    public override void init()
    {
        cost = 2;
        damage = attachedHero.getDamage();
        range = 1;
        cooldown = 3;
        abilityName = "Sweep";
        transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{abilityName}- {cost} AP to use.";
    }
}
