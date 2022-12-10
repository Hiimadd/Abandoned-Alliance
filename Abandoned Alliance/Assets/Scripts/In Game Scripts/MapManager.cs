using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    //Struct of properties required to define a Hero object.
    //Used in combination with a list to be able to spawn many situations with the same MapManager script,
    //only changing the set of Heroes that are passed in.
    [Serializable]
    public struct HeroArgs
    {
        public int Health;
        public int Defense;
        public int MoveSpeed;
        public int ActionPoints;
        public int Damage;
        public int SightRange;
        public bool Dummy;
        public Vector2Int StartPos;
        public int HeroType;
    }

    [Serializable]
    public struct Objective
    {
        public Vector2Int ExitLoc;
        public bool NeedClearLevel;
    }


    List<List<Tile>> _map;
    public List<Hero> TurnOrder;
    public int CurrTurn;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private MapManager _myself;
    [SerializeField] private Hero _heroPrefab;
    [SerializeField] private Tilemap _fog;
    [SerializeField] private int _width = 32;
    [SerializeField] private int _height = 32;
    [SerializeField] private float _startX;
    [SerializeField] private float _startY;
    [SerializeField] private List<Vector3> _blockedTiles; //X = Column, Y = bottom row to be blocked, Z = top row to be blocked. Should be least-to greatest.
    [SerializeField] private List<HeroArgs> _toSpawn; //List of Heroes to be spawned on the map
    [SerializeField] private Objective _objective;
    private double _canUseShortcut = 0;
    private double _canUseShortcutWait = 0.25;

    //Generate the set of tiles needed to overlay the TileMap with interactable/highlightable squares
    //Takes advantage of startX and startY to align with maps that don't have the first tile at 0,0
    //Uses width and height to create any rectangular board
    //Additionally checks blockedTiles to see which tiles should be marked as non-walkable.
    public void MakeGrid()
    {
        _map = new List<List<Tile>>();
        int blockedTilesPos = 0;
        for(int i = 0; i < _width; ++i)
        {
            if(blockedTilesPos < _blockedTiles.Count)
            {
                while(_blockedTiles[blockedTilesPos].x < i)
                {
                    ++blockedTilesPos;
                    if(blockedTilesPos >= _blockedTiles.Count)
                    {
                        break;
                    }
                }
            }
            List<Tile> row = new List<Tile>();
            for(int j = 0; j < _height; ++j)
            {
                Tile spawnedTile = Instantiate(_tilePrefab, new Vector3(_startX+i, _startY+j), Quaternion.identity);
                spawnedTile.name = $"Tile {i} {j}";
                if(blockedTilesPos < _blockedTiles.Count && _blockedTiles[blockedTilesPos].x == i && _blockedTiles[blockedTilesPos].z < j && blockedTilesPos < _blockedTiles.Count-1 && _blockedTiles[blockedTilesPos+1].x == i)
                {
                    ++blockedTilesPos;
                }

                if(blockedTilesPos < _blockedTiles.Count && _blockedTiles[blockedTilesPos].x == i && _blockedTiles[blockedTilesPos].y <= j && _blockedTiles[blockedTilesPos].z >= j)
                {
                    spawnedTile.Init(2, _myself, i, j);
                }
                else
                {
                    spawnedTile.Init(1, _myself, i, j);
                }
                row.Add(spawnedTile);
            }
            _map.Add(row);
        }
    }

    //Uses toSpawn to generate all player and non-player units on the map.
    //Locates the first Player unit's UI to enable it, and does the other basic configuration steps required to
    //define each unit as a unique element from health to posiiton to player/ai status.
    public void SpawnHeroes()
    {
        TurnOrder = new List<Hero>();
        CurrTurn = 0;
        bool firstPlayer = true;
        for(int i = 0; i < _toSpawn.Count; ++i)
        {
            Hero h = Instantiate(_heroPrefab, new Vector3(0,0), Quaternion.identity);
            HeroArgs a = _toSpawn[i];
            h.Init(a.Health, a.Defense, a.MoveSpeed, a.ActionPoints, a.Damage, a.SightRange, a.Dummy, _map[a.StartPos.x][a.StartPos.y], this, _fog);
            TurnOrder.Add(h);
            switch(a.HeroType)
            {
                case 1: //knight
                    h.transform.Find("KnightSprite").gameObject.SetActive(true);
                    h.transform.Find("KnightSprite").gameObject.name = "Sprite";
                    h.transform.Find("KnightCanvas").gameObject.name = "Canvas";
                break;
                case 2: //wizard
                    h.transform.Find("WizardSprite").gameObject.SetActive(true);
                    h.transform.Find("WizardSprite").gameObject.name = "Sprite";
                    h.transform.Find("WizardCanvas").gameObject.name = "Canvas";
                break;
                case 3: //ranger
                    h.transform.Find("RangerSprite").gameObject.SetActive(true);
                    h.transform.Find("RangerSprite").gameObject.name = "Sprite";
                    h.transform.Find("RangerCanvas").gameObject.name = "Canvas";
                break;
                default: //monster
                    h.transform.Find("MonsterSprite").gameObject.SetActive(true);
                    h.transform.Find("MonsterSprite").gameObject.name = "Sprite";
                break;
            }
            if(!a.Dummy)
            {
                foreach(Transform t in h.transform.Find("Canvas"))
                {
                    Ability toInit = t.gameObject.GetComponent(typeof(Ability)) as Ability;
                    toInit.Init();
                }

                if(firstPlayer)
                {
                    h.transform.Find("Canvas").gameObject.SetActive(true);
                    firstPlayer = false;
                }
            }
        }
        /*Hero hero1 = Instantiate(heroPrefab, new Vector3(0,0), Quaternion.identity);
        hero1.init(10, 0, 1, 3, 2, 3, false, map[0][0], this, fog);
        hero1.name = "Knight";
        turnOrder.Add(hero1);
        hero1.transform.Find("Canvas").gameObject.SetActive(true);


        Hero dummy = Instantiate(heroPrefab, new Vector3(5,5), Quaternion.identity);
        dummy.init(10, 0, 1, 3, 2, 3, true, map[3][3], this, fog);
        dummy.name = "Dummy1";
        turnOrder.Add(dummy);*/

    }

    //Another part of the more advanced mouse highlight system intended to be used to allow AOE attacks to highlight their effect radius.
    //Currently each conditional goes to this default behavior as AOE attacks haven't been implemented and for the sake of variable isolation, this should be kept simple until then.
    public void MouseHighlight(int X, int Y, bool addHighlight)
    {
        Hero activeHero = TurnOrder[CurrTurn];
        if(activeHero.ActiveAbility == null)
        {
            _map[X][Y].ToggleMouseHighlight(addHighlight);
        }
        else
        {
            activeHero.ActiveAbility.MouseOver(_map[X][Y], addHighlight);
        }
    }

    //Called with a specific tile to indicate the current Hero should try to use their ability there.
    //The effect this has is completely determined by that activeAbility, and this is usually called by a tile passing "this" after a user clicks on it.
    public void TriggerAbility(Tile loc)
    {
        Hero activeHero = TurnOrder[CurrTurn];
        if(activeHero.ActiveAbility == null)
        {
            return;
        }
        int advancedTime = activeHero.ActiveAbility.GetCost();
        if(activeHero.ActiveAbility.UseAbility(loc))
        {
            foreach(Transform t in activeHero.transform.Find("Canvas"))
            {
                Ability a = t.gameObject.GetComponent(typeof(Ability)) as Ability;
                a.AdvCooldown(advancedTime);
            }
        }
    }

    //Checks an X,Y position to see if it is within the generated tilemap. If it is, returns the associated tile.
    //If it is not, returns null.
    public Tile GetPos(int x, int y)
    {
        if(x > -1 && x < _width && y > -1 && y < _height)
        {
            return _map[x][y];
        }
        return null;
    }

    //Called when a Hero's health passes 0.
    //Includes check for the unit being the last player unit (ejects to Lobby for now),
    //Checking for if the unit manages to hit and destroy itself,
    //and checking for if the removal of this unit changes turnOrder[currTurn]'s (AKA activeHero/the Unit who is currently acting) position in the list.
    public void KillHero(Hero hero)
    {
        if(hero.CheckIsDummy() == false)
        {
            int playerUnitCount = 0;
            for(int i = 0; i < TurnOrder.Count; ++i)
            {
                if(TurnOrder[i].CheckIsDummy() == false)
                {
                    ++playerUnitCount;
                    if(playerUnitCount == 2)
                    {
                        break;
                    }
                }
            }
            if(playerUnitCount == 1)
            {
                SceneManager.LoadScene("Lobby");
            }
        }

        if(TurnOrder[CurrTurn] == hero) //unit manages to kill itself
        {
            AdvTurn(hero.GetAP());
            TurnOrder.Remove(hero);
        }
        else
        {
            for(int i = 0; i < CurrTurn; ++i)
            {
                if(TurnOrder[i] == hero)
                {
                    --CurrTurn; break;
                }
            }
            TurnOrder.Remove(hero);
        }
        Destroy(hero.gameObject);
    }

    //Contains the complete set of logic for advancing to the next Player turn.
    //Will expand in the future to call AI Hero logic as needed, but for now just calls dummyAttack for the dummy units to attack a player unit within range.
    //It only actually advances to the next turn if the ability takes the unit to 0 AP. Otherwise just subtracts AP from the unit.
    public void AdvTurn(int apUsed)
    {
        Hero activeHero = TurnOrder[CurrTurn];
        if(activeHero.GetAP() <= apUsed)
        {
            activeHero.ResetAP();
            activeHero.transform.Find("Canvas").gameObject.SetActive(false);
            ++CurrTurn;
            if(CurrTurn >= TurnOrder.Count-1)
            {
                CurrTurn = 0;
            }
            int startingPosition = CurrTurn;
            while(TurnOrder[CurrTurn].CheckIsDummy())
            {
                dummyAttack(CurrTurn);
                if(CurrTurn >= TurnOrder.Count-2) 
                {
                    if(TurnOrder[CurrTurn+1].CheckIsDummy())
                    {
                        dummyAttack(CurrTurn+1);
                        CurrTurn = 0;
                        continue;
                    }
                }
                ++CurrTurn;
                if(CurrTurn == startingPosition)
                {
                    break;
                } //case where the full list is dummies, shouldn't happen in demo but might when dummies are replaced with enemies
            }
            if(CurrTurn == startingPosition && TurnOrder[CurrTurn].CheckIsDummy())
            {
                return;
            }
            TurnOrder[CurrTurn].transform.Find("Canvas").gameObject.SetActive(true);
        }
        else
        {
            activeHero.UseAP(apUsed);
        }
    }

    //Functionally a placeholder for more advanced AI scripts that will be contained within the Hero class, or a class that inherits Hero,
    //This method simply takes a dummy and attempts to attack any non-dummy units within its one-tile range.
    private void dummyAttack(int turn)
    {
        Hero currDummy = TurnOrder[turn];
        Tile currPos = currDummy.GetCurrentPos();
        for(int i = -1; i < 2; ++i)
        {
            for(int j = -1; j < 2; ++j)
            {
                if(j == 0 && i == 0)
                {
                    continue;
                }
                Tile check = GetPos(currPos.GetX() + i, currPos.GetY() + j);
                if(check != null && check.GetHero() != null && check.GetHero().CheckIsDummy() == false)
                {
                    check.GetHero().ChangeHealth(-1*currDummy.GetDamage());
                    break;
                }
            }
        }
    }

    //Called on scene load. Used to execute the two functions that initialize the entire map.
    void Start()
    {
        MakeGrid();
        SpawnHeroes();
    }


    void checkObj()
    {
        if(transform.Find("victoryCanvas").gameObject.activeSelf == false && TurnOrder[CurrTurn] == _map[_objective.ExitLoc.x][_objective.ExitLoc.y].GetHero())
        {
            if(_objective.NeedClearLevel)
            {
                foreach (Hero h in TurnOrder)
                {
                    if(h.CheckIsDummy())
                    {
                        return;
                    }
                }
            }
            
            transform.Find("victoryCanvas").gameObject.SetActive(true);
    	    //UnityEditor.EditorApplication.isPlaying = false; // Editor version
        }
    }

    //Called every frame.
    //Primarily used as keybind detection for shortcuts for Player abilities.
    void Update()
    {
        checkObj();
        //This if was created specificaly for our Beta release to create a hidden area with an "easter egg" of force-quitting the application if
        //the users found a hidden area that seems like it should be non-traversable.
        
        if(_canUseShortcut <= 0)
        {
            if(Input.GetKey("1"))
            {
                if(TurnOrder[CurrTurn].transform.Find("Canvas").childCount >= 1)
                {
                    Ability temp = TurnOrder[CurrTurn].transform.Find("Canvas").GetChild(0).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.ToggleActive();
                    _canUseShortcut = _canUseShortcutWait;
                }
            }
            else if(Input.GetKey("2"))
            {
                if(TurnOrder[CurrTurn].transform.Find("Canvas").childCount >= 2)
                {
                    Ability temp = TurnOrder[CurrTurn].transform.Find("Canvas").GetChild(1).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.ToggleActive();
                    _canUseShortcut = _canUseShortcutWait;
                }
            }
            else if(Input.GetKey("3"))
            {
                if(TurnOrder[CurrTurn].transform.Find("Canvas").childCount >= 3)
                {
                    Ability temp = TurnOrder[CurrTurn].transform.Find("Canvas").GetChild(2).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.ToggleActive();
                    _canUseShortcut = _canUseShortcutWait;
                }
            }
            else if(Input.GetKey("4"))
            {
                if(TurnOrder[CurrTurn].transform.Find("Canvas").childCount >= 4)
                {
                    Ability temp = TurnOrder[CurrTurn].transform.Find("Canvas").GetChild(3).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.ToggleActive();
                    _canUseShortcut = _canUseShortcutWait;
                }
            }
            else if(Input.GetKey("5"))
            {
                if(TurnOrder[CurrTurn].transform.Find("Canvas").childCount >= 5)
                {
                    Ability temp = TurnOrder[CurrTurn].transform.Find("Canvas").GetChild(4).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.ToggleActive();
                    _canUseShortcut = _canUseShortcutWait;
                }
            }
        }
        else
        {
            _canUseShortcut -= Time.deltaTime;
        }
    }
}