﻿using System;
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
	public Material[] keypadLabelMats;

	int[,] keypadSolvingArray = new int[ , ] { 
		{  1,  2,  3 }, {  4,  5,  6 }, {  7,  8,  9 },
		{ 10, 11, 12 }, { 13, 14, 15 }, { 16, 17, 18 },
		{ 19, 20, 21 }, { 22, 23, 24 }, { 25,  5, 26 },
		{ 27, 22, 23 }, { 28,  6, 29 }, { 30,  4, 31 },
		{ 14, 27, 10 }, {  5, 17, 12 }, {  2, 19, 32 },
		{ 26, 24, 33 }, { 34, 35,  5 }, { 11, 25, 35 },
		{ 28, 29,  7 }, { 15, 16, 36 }, { 34, 13, 30 },
		{  9,  3, 20 }, {  1,  8, 32 }, {  2, 18, 21 }
	};

	int[,] keypadAssignedSymbols = new int[6,4];

	static string moduleName = "Megakeypad";

	static int moduleIdCounter = 1;
	int moduleId = 0;
	private bool moduleSolved = false;

	//Logger stuff
	void Logger(string logMessage)
	{
		Debug.LogFormat("[{0} #{1}] {2}", moduleName, moduleId, logMessage);
	}

	void LoggerKeypress(Vector2 keycoords)
	{
		Logger(String.Format("Pressed key at [{0}, {1}]", keycoords.x, keycoords.y));
	}

	// void LoggerTitleAndInfo()
	// {
	// 	Logger("Module is set up! Pressed keypad coordinates are going to be laid out as [Row, Column] (think: \"Crawl before you Climb\").");
	// }


	void Awake ()
	{
		moduleId = moduleIdCounter++;

		foreach (KMSelectable key in keypad)
		{
			KMSelectable pressedkey = key;
			key.OnInteract += delegate () {HandleKey(pressedkey); return false; };
		}
	}

	// Use this for initialization
	void Start ()
	{
		// LoggerTitleAndInfo();

		foreach (KMSelectable key in keypad)
		{
			SetLabel(key, RNG.Range(0,36));
		}

	}

	void Set2x3Quadrant(string Quadrant)
	{
		switch (Quadrant)
		{
			case "TL":
			{

				break;
			};
			case "TR":
			{
				
				break;
			};
			case "BL":
			{
					
				break;
			};
			case "BR":
			{
				
				break;
			};
		}
	}

	void SetLabel(KMSelectable key, int labelIndex)
	{
		key.transform.Find("label").GetComponent<MeshRenderer>().material = keypadLabelMats[labelIndex];
	}

	void AvoidSymbolCollisions()
	{

	}

	void Complete()
	{

	}

	void Strike()
	{

	}

		Vector2 GetCoordsFromKeyName(KMSelectable kp)
	{
		string kpname = kp.name;
		string[] kpcoords = kpname.Split('_');
		return new Vector2(int.Parse(kpcoords[2]), int.Parse(kpcoords[1]));
	}

	void HandleKey(KMSelectable key)
	{		
		LoggerKeypress(GetCoordsFromKeyName(key));

		SetLabel(key, RNG.Range(0,36));

		// key.transform.Find("label").GetComponent<MeshRenderer>().material = keypadLabelMats[RNG.Range(0,37)];

		// keyLabel.Material = keypadLabelMats[RNG.Range(0,24)];

	}

	
}