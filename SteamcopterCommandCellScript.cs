using UnityEngine;
using System.Collections;

public class SteamcopterCommandCellScript : SteamcopterHexagonScript
{
    void Awake()
    {
        health = 150;
    }

    public override void Destroy()
    {
        SteamcopterManager.Instance.DestroySteamcopterCommandCell();
    }

	void Start () 
    {
        currentHealth = health;
	}
}
