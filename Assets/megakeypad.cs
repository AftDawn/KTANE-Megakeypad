using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RNG = UnityEngine.Random;
using KModkit;

public class megakeypad : MonoBehaviour
{
	new public KMAudio audio;
	public KMBombInfo bomb;

	public KMSelectable[] keypad;
	public Material[] keypadLedMats;

	string[,] keypadSolvingArray = new string[ , ] { 
		{  "1",  "2",  "3" }, {  "4",  "5",  "6" }, {  "7",  "8",  "9" },
		{ "10", "11", "12" }, { "13", "14", "15" }, { "16", "17", "18" },
		{ "19", "20", "21" }, { "22", "23", "24" }, { "25",  "5", "26" },
		{ "27", "22", "23" }, { "28",  "6", "29" }, { "30",  "4", "31" },
		{ "14", "27", "32" }, {  "5", "17", "12" }, {  "2", "19", "33" },
		{ "26", "24", "34" }, { "35", "36",  "5" }, { "11", "25", "36" },
		{ "28", "29",  "7" }, { "15", "16", "37" }, { "35", "38", "30" },
		{  "9",  "3", "20" }, {  "1",  "8", "33" }, {  "2", "18", "21" }
	};

	static int moduleIdCounter = 1;
	int moduleId = 0;
	private bool moduleSolved = false;



	void Awake ()
	{
		moduleId = moduleIdCounter++;

		foreach (KMSelectable key in keypad)
		{
			KMSelectable pressedkey = key;
			key.OnInteract += delegate () { HandleKey(pressedkey); return false; };
		}
	}

	// Use this for initialization
	void Start ()
	{
		Debug.LogFormat("[Megakeypad #{0}] Hello World!", moduleId);
		Determine2x3KeyGrid("TL");
	}

	void Determine2x3KeyGrid(string quadrent)
	{
		return;
	}

	void AvoidCollisions()
	{

	}

	Vector2 GetCoordsFromKeyName(KMSelectable kp)
	{
		string kpname = kp.name;
		string[] kpcoords = kpname.Split('_');
		return new Vector2(int.Parse(kpcoords[1]), int.Parse(kpcoords[2]));
	}

	void HandleKey(KMSelectable key)
	{

		//fix name into something alot more human readable, like a coordinet system!
		
		Vector2 keycoords = GetCoordsFromKeyName(key);

		Debug.LogFormat("[Megakeypad #{0}] Pressed key at {1}/{2}.", moduleId, keycoords.x, keycoords.y);
	}
}
