﻿using UnityEngine;
using System.Collections;

public class StatsController : MonoBehaviour {

	public static StatsController instance;

	public int xpMultiply = 1;
	public float xpFirstLevel = 100;
	public float difficultFactor = 1.5f;

	// Use this for initialization
	void Start () {
		instance = this;
		DontDestroyOnLoad (gameObject);
		Application.LoadLevel("Caapora");

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void AddXp (float xpAdd) {
		float newXp = (GetCurrentXp() + xpAdd)* StatsController.instance.xpMultiply;
		while(newXp >= GetNextXp ()) {
			newXp -= GetNextXp();
			AddLevel();

		}

		PlayerPrefs.SetFloat("currentXp", newXp);
	}

	public static float GetCurrentXp() {
		return PlayerPrefs.GetFloat("currentXp");
	}

	public static int GetCurrentLevel() {
		return PlayerPrefs.GetInt("currentLevel");
	}

	public static void AddLevel() {
		int newLevel = GetCurrentLevel() + 1;
		PlayerPrefs.SetInt("currentLevel", newLevel);
	}

	public static float GetNextXp() {
		return StatsController.instance.xpFirstLevel * (GetCurrentLevel() + 1) * StatsController.instance.difficultFactor;
	}
	
}