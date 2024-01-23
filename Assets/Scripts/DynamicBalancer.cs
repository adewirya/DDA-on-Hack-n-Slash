using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBalancer : MonoBehaviour
{
    public static void BalanceGame( double positiveMultiplier, double negativeMultiplier)
    {
        Enemy.maxHealth = 100 * negativeMultiplier;

        //Code for values that the bigger the easier the game.
        PlayerController.playerMaxHealth = 100 * positiveMultiplier;
        PlayerController.attackDamage1 = 25 * positiveMultiplier;
        PlayerController.attackDamage2 = 50 * positiveMultiplier;
        PotionSpawner.spawnTime = 45 * (float)negativeMultiplier;



        UnityEngine.Debug.Log("maxHealth(" + PlayerController.playerMaxHealth + ", " + Enemy.maxHealth + ")");
        UnityEngine.Debug.Log("dmg(" + PlayerController.attackDamage1 + ", " + PlayerController.attackDamage2 + ")");
        UnityEngine.Debug.Log("PSpawnTime(" + PotionSpawner.spawnTime + ")");
    }
}
