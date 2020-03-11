using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Combat : NetworkBehaviour
{
    public const int maxHealth = 100;

    public SimpleHealthBar healthBar;

    [SyncVar(hook = "OnChangeHealth")]
    public int health = maxHealth;

    [ClientRpc]
    void RpcDeath()
    {
        gameObject.GetComponent<Player>().playerDeath();
    }

    public void TakeDamage(int amount)
    {
        if (!isServer)
            return;

        health -= amount;
        Debug.Log("Current HP: " + health);
        if (health <= 0)
        {
            RpcDeath();
            health = 0;
            Debug.Log("Dead!");
        }
    }

    void OnChangeHealth(int health)
    {
        // Update the UI
        healthBar.UpdateBar(health, maxHealth);
    }

    public int getMaxHealth()
    {
        return maxHealth;
    }

}
