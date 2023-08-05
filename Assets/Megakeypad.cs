using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RNG = UnityEngine.Random;

public class Megakeypad : MonoBehaviour
{
	public new KMAudio audio;
	public KMBombInfo bomb;

	public KMSelectable[] keypad;
	public Material[] keypadLedMats;
	public Material[] keypadLabelMats;

	private static readonly int[][] KeypadSolvingArray = new int[][] { 
		new int[]{  1,  2,  3 }, new int[]{  4,  5,  6 }, new int[]{  7,  8,  9 },
		new int[]{ 10, 11, 12 }, new int[]{ 13, 14, 15 }, new int[]{ 16, 17, 18 },
		new int[]{ 19, 20, 21 }, new int[]{ 22, 23, 24 }, new int[]{ 25,  5, 26 },
		new int[]{ 27, 22, 23 }, new int[]{ 28,  6, 29 }, new int[]{ 30,  4, 31 },
		new int[]{ 14, 27, 10 }, new int[]{  5, 17, 12 }, new int[]{  2, 19, 32 },
		new int[]{ 26, 24, 33 }, new int[]{ 34, 35,  5 }, new int[]{ 11, 25, 35 },
		new int[]{ 28, 29,  7 }, new int[]{ 15, 16, 36 }, new int[]{ 34, 13, 30 },
		new int[]{  9,  3, 20 }, new int[]{  1,  8, 32 }, new int[]{  2, 18, 21 }
	};

	readonly int[][][] _keypadAssignedSymbols = new int[4][][];

	private int[] _stageNumber = new int[4]{0, 0, 0, 0};
	private bool _strike;

	private const string ModuleName = "Megakeypad";

	private static int _moduleIdCounter = 1;
	private int _moduleId;
	private bool _moduleSolved = false;

	// Logger stuff
	void Logger(string logMessage)
	{
		Debug.LogFormat("[{0} #{1}] {2}", ModuleName, _moduleId, logMessage);
	}

	void LoggerKeypress(int key, string answer)
	{
		string keyname = keypad[key].name;
		string[] keycoords = keyname.Split('_');
		Logger($"Pressed key at [{keycoords[2]}, {keycoords[1]}], which was {answer}");
	}

	// Standard KTANE stuff
	void Awake ()
	{
		_moduleId = _moduleIdCounter++;

		for (int i = 0; i < 24; i++)
		{
			int j = i;
			keypad[j].OnInteract += delegate
			{
				HandleKey(j);
				keypad[j].AddInteractionPunch();
				return false;
			};
		}
	}

	// Here's where the fun begin's, setting up the symbol labels and stuff!
	void Start ()
	{
		// Setup keypad symbols
		SetupSymbols();
		// Place them onto the keys
		SetAll2x3Quadrants();
	}

	// Thanks Yewchung for the massive amount of help with the main logic! Go check out the module that they made, "Simultaneous Simons"
    void SetupSymbols()
	{
		for (int i = 0; i < 4; i++)
		{
			List<int> seedPool = new List<int>(Enumerable.Range(0,24));
			int seed1 = PullElementFromList(seedPool);
			int seed2;
			do
			{
				seed2 = PullElementFromList(seedPool);
			} while (KeypadSolvingArray[seed1].Any(n => KeypadSolvingArray[seed2].Contains(n)));
			
			int[] seeds = new int[] { seed1, seed2 };
			int[][] set = new int[6][];

			if (seed2 < seed1)
			{
				int temp = seed1;
				seed1 = seed2;
				seed2 = temp;
			}

			List<int> bucket1 = new List<int>(KeypadSolvingArray[seed1]);
			List<int> bucket2 = new List<int>(KeypadSolvingArray[seed2]);
			List<int>[] buckets = new List<int>[] { bucket1, bucket2 };
			
			for (int j = 0; j < 6; j++)
			{
				int index;
				if (bucket1.Count <= 0)
				{
					index = 1;
				}
				else if (bucket2.Count <= 0)
				{
					index = 0;
				}
				else
				{
					index = RNG.Range(0, 2);
				}
				int element = PullElementFromList(buckets[index]);
				set[j] = new int[]{element, Array.IndexOf(KeypadSolvingArray[seeds[index]], element)+(3*index)};
			}
			_keypadAssignedSymbols[i] = set;
		}
	}

	int PullElementFromList(List<int> list)
	{
		int temp = RNG.Range(0, list.Count);
		int temp2 = list[temp];
		list.RemoveAt(temp);
		return temp2;
	}
	
	void SetAll2x3Quadrants()
	{
		for (int i = 0; i < 24; i++)
		{
			int bottomHalf = i / 12;
			int rightHalf = (i / 3) % 2;
			int quadrent = (2 * bottomHalf) + rightHalf;
			int element = (((i % 12) / 6) * 3) + (i % 3);
			SetLabel(keypad[i], _keypadAssignedSymbols[quadrent][element][0]);
		}
	}

	// Helper functions for doing stuff
	void SetLabel(KMSelectable key, int labelIndex)
	{
		key.transform.Find("label").GetComponent<MeshRenderer>().material = keypadLabelMats[labelIndex-1];
	}

	void SetLED(int key, int status)
	{
		// 0 = Off, 1 = Correct and 2 = Wrong
		keypad[key].transform.Find("led").GetComponent<MeshRenderer>().material = keypadLedMats[status];
	}

	void Strike(int key)
	{
		_strike = true;
		GetComponent<KMBombModule>().HandleStrike();
		StartCoroutine(StrikeLEDSet(key));
		_stageNumber = new int[]{0, 0, 0, 0};
	}

	IEnumerator StrikeLEDSet(int key)
	{
		for (int i = 0; i < 24; i++)
		{
			SetLED(i, 0);
		}
		SetLED(key, 2);
		yield return new WaitForSeconds(1f);
		SetLED(key, 0);
		_strike = false;
	}

	// Also thanks to Yewchung for most of this!
	void HandleKey(int key)
	{		
		if (_moduleSolved != true || _strike)
		{
			int bottomHalf = key / 12;
			int rightHalf = (key / 3) % 2;
			int quadrent = (2 * bottomHalf) + rightHalf;
			int element = (((key % 12) / 6) * 3) + (key % 3);
			if (_stageNumber[quadrent] == _keypadAssignedSymbols[quadrent][element][1])
			{
				LoggerKeypress(key, "Right");
				SetLED(key, 1);
				_stageNumber[quadrent]++;
			}
			else
			{
				LoggerKeypress(key, "Wrong");
				Strike(key);
			}
			if (_stageNumber[0] >= 6 && _stageNumber[1] >= 6 && _stageNumber[2] >= 6 && _stageNumber[3] >= 6)
			{
				Logger("Module Disarmed!!!");
				GetComponent<KMBombModule>().HandlePass();
			}
		}
	}
}