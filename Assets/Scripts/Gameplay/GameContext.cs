﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContext : MonoBehaviour {

	public static GameContext Instance
    {
        get
        {
            return _instance;
        }
    }
    private static GameContext _instance;

    private void Awake()
    {
        _instance = this;
    }

    public List<SpaceShipInfo> ships = new List<SpaceShipInfo>();

    public List<Projectile> projectiles = new List<Projectile>();

    private void Update()
    {
        Debug.Log("Ships:" + ships.Count + "///       Projectiles:" + projectiles.Count);
    }
}