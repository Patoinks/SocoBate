using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DuelScript : MonoBehaviour
{
   /* // Declare the UI elements
    public TMP_Text jogador1Text;  // Player 1's name (or unit name)
    public TMP_Text jogador2Text;  // Player 2's name (or unit name)
    public Slider healthBarUnit1;  // Health bar for Player 1's unit
    public Slider healthBarUnit2;  // Health bar for Player 2's unit
    public TMP_Text statusText;    // Display current turn or battle status

    // Unit data (simplified for this example)
    public int playerUnitHealth = 100;
    public int enemyUnitHealth = 100;
    public int playerUnitAttack = 20;
    public int enemyUnitAttack = 15;
    public int playerUnitSpeed = 10;
    public int enemyUnitSpeed = 8;

    private bool playerTurn = true; // To track whose turn it is
    private bool battleOngoing = true;

    void Start()
    {
        // Initialize health bars and status
        healthBarUnit1.value = playerUnitHealth;
        healthBarUnit2.value = enemyUnitHealth;
        statusText.text = "Player 1's Turn";
    }

    void Update()
    {
        if (battleOngoing)
        {
            // Handle turn-based system
            if (playerTurn)
            {
                if (Input.GetKeyDown(KeyCode.Space)) // Simulate player action with spacebar
                {
                    PlayerTurnAttack();
                    playerTurn = false;
                    statusText.text = "Enemy's Turn";
                }
            }
            else
            {
                EnemyTurnAttack();
                playerTurn = true;
                statusText.text = "Player 1's Turn";
            }

            // Update health bars
            healthBarUnit1.value = Mathf.Clamp(playerUnitHealth, 0, 100);
            healthBarUnit2.value = Mathf.Clamp(enemyUnitHealth, 0, 100);

            // Check battle status after every action
            CheckBattleStatus();
        }
    }

    // Player's turn to attack
    void PlayerTurnAttack()
    {
        // Simulate attack by reducing enemy health
        enemyUnitHealth -= playerUnitAttack;
        if (enemyUnitHealth < 0) enemyUnitHealth = 0;  // Prevent negative health
    }

    // Enemy's turn to attack
    void EnemyTurnAttack()
    {
        // Simulate attack by reducing player health
        playerUnitHealth -= enemyUnitAttack;
        if (playerUnitHealth < 0) playerUnitHealth = 0;  // Prevent negative health
    }

    // Check if the battle is over
    void CheckBattleStatus()
    {
        if (playerUnitHealth <= 0)
        {
            battleOngoing = false;
            statusText.text = "Enemy Wins!";
        }
        else if (enemyUnitHealth <= 0)
        {
            battleOngoing = false;
            statusText.text = "Player 1 Wins!";
        }
    }*/
}
