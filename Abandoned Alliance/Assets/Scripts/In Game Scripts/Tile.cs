using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Hero _currUnit;
    private int _tileType;
    private MapManager _mapManager;
    private int _x;
    private int _y;
    [SerializeField] private Color _transparent, _highlight, _abilityHighlight, _invalidAbilityHighlight;
    [SerializeField] private SpriteRenderer _renderer;
    private Color _defaultColor;

    //Roughly equivelent to a constructor, this is called on Tile creation to set the properties of the generated Tile.
    public void Init(int type, MapManager mm, int x, int y)
    {
        _currUnit = null;
        _tileType = type;
        _mapManager = mm;
        _renderer.color = _defaultColor = _transparent;
        _x = x;
        _y = y;
    }

    //Variables to store/return the position of this tile relitive to the other tiles that have been generated.
    //Useful if you want to find adjacent tiles to a given position
    public int GetX() {return _x;}
    public int GetY() {return _y;}

    //Update the tracking of the Hero which is currently on this tile
    public void SetHero(Hero hero)
    {
        _currUnit = hero;
    }

    //Return the Hero currently standing on this tile, for use with abilities or more general checks within a MapManager.
    public Hero GetHero()
    {
        return _currUnit;
    }

    //Returns an int indicating if the tile is walkable (1) or not (any other number).
    //May be useful to eventually to create more tile type definitions, but for now walkable or not is sufficient.
    public int GetTileType()
    {
        return _tileType;
    }

    //Indended use is to indicate that an ability can be used on this tile if it is clicked
    //Functionally all it does is change the "resting" (not moused over) state of a tile to be abilityHighlight.
    public void ToggleAbilityHighlight()
    {
        if(_defaultColor != _abilityHighlight)
        {
            _defaultColor = _abilityHighlight;
        }
        else
        {
            _defaultColor = _transparent;
        }
        _renderer.color = _defaultColor;
    }

    //Indended use is to indicate that an ability could be used on this tile, if not for some factor that makes it invalid, such as a presence/lack of units.
    //Functionally all it does is change the "resting" (not moused over) state of a tile to be invalidAbilityHighlight.
    public void ToggleInvalidAbilityHighlight()
    {
        if(_defaultColor != _invalidAbilityHighlight)
        {
            _defaultColor = _invalidAbilityHighlight;
        }
        else
        {
            _defaultColor = _transparent;
        }
        _renderer.color = _defaultColor;
    }

    //Part of a more complex mouseover highlight system that simply highlights the tile the user is hovering over normally,
    //But should also show the effect radius of an area attack once those are implemented correctly.
    public void ToggleMouseHighlight(bool shouldHighlight)
    {
        if(shouldHighlight)
        {
            _renderer.color = _highlight;
        }
        else
        {
            _renderer.color = _defaultColor;
        }
    }

    //Part of the mouseover highlight system, this indicates that the user is hovering over the tile.
    void OnMouseEnter()
    {
        if(_tileType == 1)
        {
            _mapManager.MouseHighlight(_x, _y, true);
        }
    }

    //Part of the mouseover highlight system, this indicates that the user has stopped hovering over the tile and that it therefore should no longer be highlighted.
    void OnMouseExit()
    {
        if(_tileType == 1)
        {
            _mapManager.MouseHighlight(_x, _y, false);
        }
    }

    //Captures click events on tiles. Essential for ability activation, as they need to know where the user is attempting to use a given ability.
    void OnMouseDown()
    {
        //Debug.Log($"X: {X}, Y: {Y}");
        _mapManager.TriggerAbility(this);
    }
}