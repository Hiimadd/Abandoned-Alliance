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
        public bool dummy;
        public Vector2Int startPos;
        public int heroType;
    }

    [Serializable]
    public struct Objective
    {
        public Vector2Int exitLoc;
        public bool needClearLevel;
    }


    List<List<Tile>> map;
    public List<Hero> turnOrder;
    public int currTurn;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private MapManager myself;
    [SerializeField] private Hero heroPrefab;
    [SerializeField] private Tilemap fog;
    [SerializeField] private int width = 32;
    [SerializeField] private int height = 32;
    [SerializeField] private float startX;
    [SerializeField] private float startY;
    [SerializeField] private List<Vector3> blockedTiles; //X = Column, Y = bottom row to be blocked, Z = top row to be blocked. Should be least-to greatest.
    [SerializeField] private List<HeroArgs> toSpawn; //List of Heroes to be spawned on the map
    [SerializeField] private Objective objective;
    private double canUseShortcut = 0;
    private double canUseShortcutWait = 0.25;

    //Generate the set of tiles needed to overlay the TileMap with interactable/highlightable squares
    //Takes advantage of startX and startY to align with maps that don't have the first tile at 0,0
    //Uses width and height to create any rectangular board
    //Additionally checks blockedTiles to see which tiles should be marked as non-walkable.
    public void makeGrid()
    {
        map = new List<List<Tile>>();
        int blockedTilesPos = 0;
        for(int i = 0; i < width; ++i)
        {
            if(blockedTilesPos < blockedTiles.Count)
            {
                while(blockedTiles[blockedTilesPos].x < i) {++blockedTilesPos; if(blockedTilesPos >= blockedTiles.Count) {break;}}
            }
            List<Tile> row = new List<Tile>();
            for(int j = 0; j < height; ++j)
            {
                Tile spawnedTile = Instantiate(tilePrefab, new Vector3(startX+i, startY+j), Quaternion.identity);
                spawnedTile.name = $"Tile {i} {j}";
                if(blockedTilesPos < blockedTiles.Count && blockedTiles[blockedTilesPos].x == i && blockedTiles[blockedTilesPos].z < j && blockedTilesPos < blockedTiles.Count-1 && blockedTiles[blockedTilesPos+1].x == i) {++blockedTilesPos;}

                if(blockedTilesPos < blockedTiles.Count && blockedTiles[blockedTilesPos].x == i && blockedTiles[blockedTilesPos].y <= j && blockedTiles[blockedTilesPos].z >= j)
                {
                    spawnedTile.init(2, myself, i, j);
                }
                else
                {
                    spawnedTile.init(1, myself, i, j);
                }
                row.Add(spawnedTile);
            }
            map.Add(row);
        }
    }

    //Uses toSpawn to generate all player and non-player units on the map.
    //Locates the first Player unit's UI to enable it, and does the other basic configuration steps required to
    //define each unit as a unique element from health to posiiton to player/ai status.
    public void spawnHeroes()
    {
        turnOrder = new List<Hero>();
        currTurn = 0;
        bool firstPlayer = true;
        for(int i = 0; i < toSpawn.Count; ++i)
        {
            Hero h = Instantiate(heroPrefab, new Vector3(0,0), Quaternion.identity);
            HeroArgs a = toSpawn[i];
            h.init(a.Health, a.Defense, a.MoveSpeed, a.ActionPoints, a.Damage, a.SightRange, a.dummy, map[a.startPos.x][a.startPos.y], this, fog);
            turnOrder.Add(h);
            switch(a.heroType)
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
            if(!a.dummy)
            {
                foreach(Transform t in h.transform.Find("Canvas"))
                {
                    Ability toInit = t.gameObject.GetComponent(typeof(Ability)) as Ability;
                    toInit.init();
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
    public void mouseHighlight(int X, int Y, bool addHighlight)
    {
        Hero activeHero = turnOrder[currTurn];
        if(activeHero.activeAbility == null)
        {
            map[X][Y].toggleMouseHighlight(addHighlight);
        }
        else
        {
            activeHero.activeAbility.mouseOver(map[X][Y], addHighlight);
        }
    }

    //Called with a specific tile to indicate the current Hero should try to use their ability there.
    //The effect this has is completely determined by that activeAbility, and this is usually called by a tile passing "this" after a user clicks on it.
    public void triggerAbility(Tile loc)
    {
        Hero activeHero = turnOrder[currTurn];
        if(activeHero.activeAbility == null) {return;}
        int advancedTime = activeHero.activeAbility.getCost();
        if(activeHero.activeAbility.UseAbility(loc))
        {
            foreach(Transform t in activeHero.transform.Find("Canvas"))
            {
                Ability a = t.gameObject.GetComponent(typeof(Ability)) as Ability;
                a.advCooldown(advancedTime);
            }
        }
    }

    //Checks an X,Y position to see if it is within the generated tilemap. If it is, returns the associated tile.
    //If it is not, returns null.
    public Tile getPos(int x, int y)
    {
        if(x > -1 && x < width && y > -1 && y < height)
        {
            return map[x][y];
        }
        return null;
    }

    //Called when a Hero's health passes 0.
    //Includes check for the unit being the last player unit (ejects to Lobby for now),
    //Checking for if the unit manages to hit and destroy itself,
    //and checking for if the removal of this unit changes turnOrder[currTurn]'s (AKA activeHero/the Unit who is currently acting) position in the list.
    public void killHero(Hero hero)
    {
        if(hero.checkIsDummy() == false)
        {
            int playerUnitCount = 0;
            for(int i = 0; i < turnOrder.Count; ++i)
            {
                if(turnOrder[i].checkIsDummy() == false) {++playerUnitCount; if(playerUnitCount == 2) {break;}}
            }
            if(playerUnitCount == 1) {SceneManager.LoadScene("Lobby");}
        }

        if(turnOrder[currTurn] == hero) //unit manages to kill itself
        {
            advTurn(hero.getAP());
            turnOrder.Remove(hero);
        }
        else
        {
            for(int i = 0; i < currTurn; ++i) {if(turnOrder[i] == hero) {--currTurn; break;}}
            turnOrder.Remove(hero);
        }
        Destroy(hero.gameObject);
    }

    //Contains the complete set of logic for advancing to the next Player turn.
    //Will expand in the future to call AI Hero logic as needed, but for now just calls dummyAttack for the dummy units to attack a player unit within range.
    //It only actually advances to the next turn if the ability takes the unit to 0 AP. Otherwise just subtracts AP from the unit.
    public void advTurn(int apUsed)
    {
        Hero activeHero = turnOrder[currTurn];
        if(activeHero.getAP() <= apUsed)
        {
            activeHero.resetAP();
            activeHero.transform.Find("Canvas").gameObject.SetActive(false);
            ++currTurn;
            if(currTurn >= turnOrder.Count-1) {currTurn = 0;}
            int startingPosition = currTurn;
            while(turnOrder[currTurn].checkIsDummy())
            {
                dummyAttack(currTurn);
                if(currTurn >= turnOrder.Count-2) 
                {
                    if(turnOrder[currTurn+1].checkIsDummy()) {dummyAttack(currTurn+1); currTurn = 0; continue;}
                }
                ++currTurn;
                if(currTurn == startingPosition) {break;} //case where the full list is dummies, shouldn't happen in demo but might when dummies are replaced with enemies
            }
            if(currTurn == startingPosition && turnOrder[currTurn].checkIsDummy()) {return;}
            turnOrder[currTurn].transform.Find("Canvas").gameObject.SetActive(true);
        }
        else {activeHero.useAP(apUsed);}
    }

    //Functionally a placeholder for more advanced AI scripts that will be contained within the Hero class, or a class that inherits Hero,
    //This method simply takes a dummy and attempts to attack any non-dummy units within its one-tile range.
    private void dummyAttack(int turn)
    {
        Hero currDummy = turnOrder[turn];
        Tile currPos = currDummy.getCurrentPos();
        for(int i = -1; i < 2; ++i)
        {
            for(int j = -1; j < 2; ++j)
            {
                if(j == 0 && i == 0) {continue;}
                Tile check = getPos(currPos.getX() + i, currPos.getY() + j);
                if(check != null && check.getHero() != null && check.getHero().checkIsDummy() == false)
                {
                    check.getHero().changeHealth(-1*currDummy.getDamage());
                    break;
                }
            }
        }
    }

    //Called on scene load. Used to execute the two functions that initialize the entire map.
    void Start()
    {
        makeGrid();
        spawnHeroes();
    }


    void checkObj()
    {
        if(transform.Find("victoryCanvas").gameObject.activeSelf == false && turnOrder[currTurn] == map[objective.exitLoc.x][objective.exitLoc.y].getHero())
        {
            if(objective.needClearLevel)
            {
                foreach (Hero h in turnOrder)
                {
                    if(h.checkIsDummy()) {return;}
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
        
        if(canUseShortcut <= 0)
        {
            if(Input.GetKey("1"))
            {
                if(turnOrder[currTurn].transform.Find("Canvas").childCount >= 1)
                {
                    Ability temp = turnOrder[currTurn].transform.Find("Canvas").GetChild(0).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.toggleActive();
                    canUseShortcut = canUseShortcutWait;
                }
            }
            else if(Input.GetKey("2"))
            {
                if(turnOrder[currTurn].transform.Find("Canvas").childCount >= 2)
                {
                    Ability temp = turnOrder[currTurn].transform.Find("Canvas").GetChild(1).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.toggleActive();
                    canUseShortcut = canUseShortcutWait;
                }
            }
            else if(Input.GetKey("3"))
            {
                if(turnOrder[currTurn].transform.Find("Canvas").childCount >= 3)
                {
                    Ability temp = turnOrder[currTurn].transform.Find("Canvas").GetChild(2).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.toggleActive();
                    canUseShortcut = canUseShortcutWait;
                }
            }
            else if(Input.GetKey("4"))
            {
                if(turnOrder[currTurn].transform.Find("Canvas").childCount >= 4)
                {
                    Ability temp = turnOrder[currTurn].transform.Find("Canvas").GetChild(3).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.toggleActive();
                    canUseShortcut = canUseShortcutWait;
                }
            }
            else if(Input.GetKey("5"))
            {
                if(turnOrder[currTurn].transform.Find("Canvas").childCount >= 5)
                {
                    Ability temp = turnOrder[currTurn].transform.Find("Canvas").GetChild(4).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.toggleActive();
                    canUseShortcut = canUseShortcutWait;
                }
            }
        }
        else
        {
            canUseShortcut -= Time.deltaTime;
        }
    }
}