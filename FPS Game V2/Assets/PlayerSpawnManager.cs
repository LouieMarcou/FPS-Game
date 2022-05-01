using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    public Transform[] spawnLocations;

    private int numPlayers = 0;

    void OnPlayerJoined(PlayerInput playerInput)
    {
        //Debug.Log("PlayerInput ID: " + playerInput.playerIndex);
        numPlayers++;
        playerInput.gameObject.GetComponent<PlayerDetails>().playerID = playerInput.playerIndex + 1;

        playerInput.gameObject.GetComponent<PlayerDetails>().startPos = spawnLocations[playerInput.playerIndex].position;
        playerInput.gameObject.GetComponent<PlayerDetails>().startRot = spawnLocations[playerInput.playerIndex].rotation;
    }

    public int getNumPlayers()
    {
        return numPlayers;
    }
}
