using System;
using System.Collections;
using System.Collections.Generic;
using Reginald;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private Player player;

    private void Start()
    {
        player.DisableInput();
    }

    public void StartPlaying()
    {
        startScreen.SetActive(false);
        player.EnableInput();
    }
}
