using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBase
{
    string armorType;
    string attackType;
    int health;
    int defense;
    int attack;
    int movementSpeed;
    int range;
    int actionPoint;

    /* maybe some method that all hero have */
    public void Die()
    {

    }

    public void usePowerUps(string powerUpName)
    {

    }


    /* Ability related methods */
    public void unlockAbility()
    {

    }

    public void upgradeAbility()
    {

    }

    /* Method to be override for different heroes
     * Subject to change
     */

    /* Basic attack: can attack single target or multiple target */
    /* Parent class will only support single target attack with 1 tile*/
    public virtual void basicAttack(int range)
    {
        /* select one tile around you */
        Debug.Log("Used basic attack!");
    }

    /* Passive/Active Ability */
    /* Parent class will only support active skill */
    public virtual void ability2()
    {
        Debug.Log("Used ability 2");
    }

    /* Parent class will not support ultimate attack because each hero have very different ultimate skills */
}
