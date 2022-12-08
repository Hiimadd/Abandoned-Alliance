using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Hero : MonoBehaviour
{
    public Ability activeAbility;
    private MapManager _mapManager;
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

    //Move Hero to the passed in Tile's location. Verifies that the tile it is currently associated with has that association removed first.
    public void UpdatePosition(Tile loc)
    {
        if(currLoc != null) 
        {
            currLoc.SetHero(null);
        }
        transform.position = loc.transform.position;
        currLoc = loc;
        loc.SetHero(this);
        if(!isDummy) 
        {
            FogUpdate();
        }
    }

    //Return the current Tile location of the hero. Especially useful for abilities, which need to reference Hero position to calculate distance.
    public Tile getCurrentPos() {return currLoc;}

    //Returns the current health of the Hero
    public int getHealth() {return currHealth;}

    //Returns the maximum health of the Hero.
    //Unlikely to be used in isolation, mostly useful for calculating currHealth:maxHealth ratio.
    public int getMaxHealth() {return maxHealth;}

    //Pass in the amount the health should change by, positive = increase in health, negative = decrease in health.
    //Will not increase health above the character's initial health points.
    //Will also tell the associated MapManager to delete itself if health is reduced to/below zero.
    public void changeHealth(int change)
    {
        currHealth += change;
        if(currHealth > maxHealth) {currHealth = maxHealth;}
        if(currHealth < 1) {currLoc.SetHero(null); _mapManager.killHero(this);}
    }

    //Returns the number of action points a Hero has remaining on their turn.
    public int getAP() {return currActionPoints;}

    //Subtracts used from the number of action points the Hero has remaining
    //Doesn't explicitly check to make sure it doesn't go below zero, as it's expected that whatever ability that is used to trigger this already checks that.
    public void useAP(int used) {currActionPoints -= used;}

    //Sets current action points equal to the max for the Hero, usually at the end of their turn to prepare for the next round
    public void resetAP() {currActionPoints = maxActionPoints;}

    //Returns the movement speed of the Hero. Not currently in use.
    public int getMoveSpeed() {return moveSpeed;}

    //Returns the number of tiles away from a character the fog of war should be revealed.
    public int getSightRange() {return sightRange;}

    //Returns the base damage for attacks done by this Hero.
    public int getDamage() {return damage;}

    //Used in Tutorial map to create enemies that are recognized as neither player-controled nor actual opponents.
    public bool checkIsDummy() {return isDummy;}

    //Returns the MapManager that spawned this Hero. Useful in passing information between abilities and the MapManager, as abilities do not have a direct reference to the MapManager.
    public MapManager getMapManager() {return _mapManager;}

    //Roughly equivelent to a constructor, this is called on Hero creation to set the properties of the generated unit.
    public void init(int Health,int Defense,int MoveSpeed,int ActionPoints,int Damage,int SightRange, bool dummy, Tile loc, MapManager mm, Tilemap tm)
    {
        maxHealth = currHealth = Health;
        defense = Defense;
        moveSpeed = MoveSpeed;
        maxActionPoints = currActionPoints = ActionPoints;
        damage = Damage;
        sightRange = SightRange;
        isDummy = dummy;
        _mapManager = mm;
        fog = tm;
        UpdatePosition(loc);
        
    }

    //Interaction with the fog of war tilemap to reveal map regions as a unit moves
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
