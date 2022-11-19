using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public Ability activeAbility;
    private MapManager mapManager;
    public bool isDummy;
    private int maxHealth;
    private int currHealth;
    private int defense;
    private int moveSpeed;
    private int maxActionPoints;
    private int currActionPoints;
    private int damage;
    private int sightRange;
    private Tile currLoc;

    //Pass in the amount the health should change by, positive = increase in health, negative = decrease in health

    public void updatePosition(Tile loc)
    {
        if(currLoc != null) {currLoc.setHero(null);}
        transform.position = loc.transform.position;
        currLoc = loc;
        loc.setHero(this);
    }

    public Tile getCurrentPos() {return currLoc;}

    public int getHealth() {return currHealth;}

    public void changeHealth(int change)
    {
        currHealth += change;
        if(currHealth > maxHealth) {currHealth = maxHealth;}
        if(currHealth < 1) {currLoc.setHero(null); mapManager.killHero(this);}
    }

    public int getAP() {return currActionPoints;}

    public void useAP(int used) {currActionPoints -= used;}

    public void resetAP() {currActionPoints = maxActionPoints;}

    public int getMoveSpeed() {return moveSpeed;}

    public int getSightRange() {return sightRange;}

    public int getDamage() {return damage;}

    public MapManager getMapManager() {return mapManager;}

    public void init(int Health,int Defense,int MoveSpeed,int ActionPoints,int Damage,int SightRange, Tile loc, MapManager mm)
    {
        maxHealth = currHealth = Health;
        defense = Defense;
        moveSpeed = MoveSpeed;
        maxActionPoints = currActionPoints = ActionPoints;
        damage = Damage;
        sightRange = SightRange;
        updatePosition(loc);
        mapManager = mm;
        isDummy = false;
    }
}
