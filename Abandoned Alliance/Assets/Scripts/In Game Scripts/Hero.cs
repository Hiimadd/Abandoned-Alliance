using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Hero : MonoBehaviour
{
    public Ability activeAbility;
    private MapManager mapManager;
    private bool isDummy;
    private int maxHealth;
    private int currHealth;
    private int defense;
    private int moveSpeed;
    private int maxActionPoints;
    private int currActionPoints;
    private int damage;
    private int sightRange;
    private Tile currLoc;
    private Tilemap fog;

    //Pass in the amount the health should change by, positive = increase in health, negative = decrease in health

    public void updatePosition(Tile loc)
    {
        if(currLoc != null) {currLoc.setHero(null);}
        transform.position = loc.transform.position;
        currLoc = loc;
        loc.setHero(this);
        if(!isDummy) {FogUpdate();}
    }

    public Tile getCurrentPos() {return currLoc;}

    public int getHealth() {return currHealth;}

    public int getMaxHealth() {return maxHealth;}

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

    public bool checkIsDummy() {return isDummy;}

    public MapManager getMapManager() {return mapManager;}

    public void init(int Health,int Defense,int MoveSpeed,int ActionPoints,int Damage,int SightRange, bool dummy, Tile loc, MapManager mm, Tilemap tm)
    {
        maxHealth = currHealth = Health;
        defense = Defense;
        moveSpeed = MoveSpeed;
        maxActionPoints = currActionPoints = ActionPoints;
        damage = Damage;
        sightRange = SightRange;
        isDummy = dummy;
        mapManager = mm;
        fog = tm;
        updatePosition(loc);
        
    }

    private void FogUpdate()
    {
        Vector3Int currPlayerPos = fog.WorldToCell(transform.position); // cell position convert to world position
        for(int i = -sightRange; i <= sightRange; i++)
        {
            for (int j = -sightRange; j <= sightRange; j++)
            {
                fog.SetTile(currPlayerPos + new Vector3Int(i, j, 0), null);// remove tile surround the player
            }
        }
    }
}
