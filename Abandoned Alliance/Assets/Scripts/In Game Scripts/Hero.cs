using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Hero : MonoBehaviour
{
    public Ability ActiveAbility;
    private MapManager _mapManager;
    private bool _isDummy;
    private int _maxHealth;
    private int _currHealth;
    private int _defense;
    private int _moveSpeed;
    private int _maxActionPoints;
    private int _currActionPoints;
    private int _damage;
    private int _sightRange;
    private Tile _currLoc;
    private Tilemap _fog;

    //Move Hero to the passed in Tile's location. Verifies that the tile it is currently associated with has that association removed first.
    public void UpdatePosition(Tile loc)
    {
        if(_currLoc != null) 
        {
            _currLoc.SetHero(null);
        }
        transform.position = loc.transform.position;
        _currLoc = loc;
        loc.SetHero(this);
        if(!_isDummy) 
        {
            fogUpdate();
        }
    }

    //Return the current Tile location of the hero. Especially useful for abilities, which need to reference Hero position to calculate distance.
    public Tile GetCurrentPos() {return _currLoc;}

    //Returns the current health of the Hero
    public int GetHealth() {return _currHealth;}

    //Returns the maximum health of the Hero.
    //Unlikely to be used in isolation, mostly useful for calculating currHealth:maxHealth ratio.
    public int GetMaxHealth() {return _maxHealth;}

    //Pass in the amount the health should change by, positive = increase in health, negative = decrease in health.
    //Will not increase health above the character's initial health points.
    //Will also tell the associated MapManager to delete itself if health is reduced to/below zero.
    public void ChangeHealth(int change)
    {
        _currHealth += change;
        if(_currHealth > _maxHealth) {_currHealth = _maxHealth;}
        if(_currHealth < 1) {_currLoc.SetHero(null); _mapManager.KillHero(this);}
    }

    //Returns the number of action points a Hero has remaining on their turn.
    public int GetAP() {return _currActionPoints;}

    //Subtracts used from the number of action points the Hero has remaining
    //Doesn't explicitly check to make sure it doesn't go below zero, as it's expected that whatever ability that is used to trigger this already checks that.
    public void UseAP(int used) {_currActionPoints -= used;}

    //Sets current action points equal to the max for the Hero, usually at the end of their turn to prepare for the next round
    public void ResetAP() {_currActionPoints = _maxActionPoints;}

    //Returns the movement speed of the Hero. Not currently in use.
    public int GetMoveSpeed() {return _moveSpeed;}

    //Returns the number of tiles away from a character the fog of war should be revealed.
    public int GetSightRange() {return _sightRange;}

    //Returns the base damage for attacks done by this Hero.
    public int GetDamage() {return _damage;}

    //Used in Tutorial map to create enemies that are recognized as neither player-controled nor actual opponents.
    public bool CheckIsDummy() {return _isDummy;}

    //Returns the MapManager that spawned this Hero. Useful in passing information between abilities and the MapManager, as abilities do not have a direct reference to the MapManager.
    public MapManager GetMapManager() {return _mapManager;}

    //Roughly equivelent to a constructor, this is called on Hero creation to set the properties of the generated unit.
    public void Init(int health,int defense,int moveSpeed,int actionPoints,int damage,int sightRange, bool dummy, Tile loc, MapManager mm, Tilemap tm)
    {
        _maxHealth = _currHealth = health;
        _defense = defense;
        _moveSpeed = moveSpeed;
        _maxActionPoints = _currActionPoints = actionPoints;
        _damage = damage;
        _sightRange = sightRange;
        _isDummy = dummy;
        _mapManager = mm;
        _fog = tm;
        UpdatePosition(loc);
        
    }

    //Interaction with the fog of war tilemap to reveal map regions as a unit moves
    private void fogUpdate()
    {
        Vector3Int currPlayerPos = _fog.WorldToCell(transform.position); // cell position convert to world position
        for(int i = -_sightRange; i <= _sightRange; i++)
        {
            for (int j = -_sightRange; j <= _sightRange; j++)
            {
                _fog.SetTile(currPlayerPos + new Vector3Int(i, j, 0), null);// remove tile surround the player
            }
        }
    }
}
