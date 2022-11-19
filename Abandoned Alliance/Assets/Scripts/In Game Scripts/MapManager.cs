using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    List<List<Tile>> map;
    List<Hero> turnOrder;
    int currTurn;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private MapManager myself;
    [SerializeField] private Hero heroPrefab;
    private int width = 32;
    private int height = 32;

    public void makeGrid()
    {
        map = new List<List<Tile>>();
        for(int i = 0; i < width; ++i)
        {
            List<Tile> row = new List<Tile>();
            for(int j = 0; j < height; ++j)
            {
                Tile spawnedTile = Instantiate(tilePrefab, new Vector3(i, j), Quaternion.identity);
                spawnedTile.name = $"Tile {i} {j}";
                spawnedTile.init(1, myself, i, j);
                row.Add(spawnedTile);
            }
            map.Add(row);
        }
    }

    public void spawnHeroes()
    {
        turnOrder = new List<Hero>();
        currTurn = 0;
        Hero hero1 = Instantiate(heroPrefab, new Vector3(0,0), Quaternion.identity);
        hero1.init(10, 0, 1, 3, 2, 3, map[0][0], this);
        hero1.name = "Knight";
        turnOrder.Add(hero1);
        map[0][0].setHero(hero1);
        hero1.transform.Find("Canvas").gameObject.SetActive(true);


        Hero dummy = Instantiate(heroPrefab, new Vector3(5,5), Quaternion.identity);
        dummy.init(10, 0, 1, 3, 2, 3, map[5][5], this);
        dummy.isDummy = true;
        dummy.name = "Dummy1";
        turnOrder.Add(dummy);
        map[5][5].setHero(dummy);

    }

    public void triggerAbility(Tile loc)
    {
        Hero activeHero = turnOrder[currTurn];
        if(activeHero.activeAbility == null) {return;}
        activeHero.activeAbility.UseAbility(loc);
    }

    public Tile getPos(int x, int y)
    {
        if(x > -1 && x < width && y > -1 && y < height)
        {
            return map[x][y];
        }
        return null;
    }

    public void killHero(Hero hero)
    {
        if(turnOrder[currTurn] == hero) //unit manages to kill itself
        {
            advTurn(hero.getAP());
            turnOrder.Remove(hero);
        }
        else
        {
            turnOrder.Remove(hero);
        }
        Destroy(hero.gameObject);
    }

    public void advTurn(int apUsed)
    {
        Hero activeHero = turnOrder[currTurn];
        Debug.Log(activeHero.name);
        if(activeHero.getAP() == apUsed)
        {
            activeHero.resetAP();
            activeHero.transform.Find("Canvas").gameObject.SetActive(false);

            ++currTurn;
            if(currTurn >= turnOrder.Count) {currTurn = 0;}
            int startingPosition = currTurn;
            while(turnOrder[currTurn].isDummy)
            {
                if(currTurn >= turnOrder.Count-1) 
                {
                    if(turnOrder[currTurn+1].isDummy) {currTurn = 0; continue;}
                }
                ++currTurn;
                if(currTurn == startingPosition) {break;} //case where the full list is dummies, shouldn't happen in demo but might when dummies are replaced with enemies
            }

            turnOrder[currTurn].transform.Find("Canvas").gameObject.SetActive(true);
        }
        else {activeHero.useAP(apUsed);}
    }

    void Start()
    {
        makeGrid();
        spawnHeroes();
    }


}