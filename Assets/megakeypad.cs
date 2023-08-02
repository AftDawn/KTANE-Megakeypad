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
	public Material[] keypadLabelMats;

	static int[,] keypadSolvingArray = new int[24,3] { 
		{  1,  2,  3 }, {  4,  5,  6 }, {  7,  8,  9 },
		{ 10, 11, 12 }, { 13, 14, 15 }, { 16, 17, 18 },
		{ 19, 20, 21 }, { 22, 23, 24 }, { 25,  5, 26 },
		{ 27, 22, 23 }, { 28,  6, 29 }, { 30,  4, 31 },
		{ 14, 27, 10 }, {  5, 17, 12 }, {  2, 19, 32 },
		{ 26, 24, 33 }, { 34, 35,  5 }, { 11, 25, 35 },
		{ 28, 29,  7 }, { 15, 16, 36 }, { 34, 13, 30 },
		{  9,  3, 20 }, {  1,  8, 32 }, {  2, 18, 21 }
	};

	int[,] keypadAssignedSymbols = new int[24,2];

	private int stageNumber;

	static string moduleName = "Megakeypad";

	static int moduleIdCounter = 1;
	int moduleId = 0;
	private bool moduleSolved = false;

	// Logger stuff
	void Logger(string logMessage)
	{
		Debug.LogFormat("[{0} #{1}] {2}", moduleName, moduleId, logMessage);
	}

	void LoggerKeypress(KMSelectable key, string answer)
	{
		string keyname = key.name;
		string[] keycoords = keyname.Split('_');
		Logger(String.Format("Pressed key at [{0}, {1}], which was {2}", keycoords[2], keycoords[1], answer));
	}

	void Logger2DArray(int[,] array)
	{
		
	}

	// Standard KTANE stuff
	void Awake ()
	{
		moduleId = moduleIdCounter++;

		foreach (KMSelectable key in keypad)
		{
			KMSelectable pressedkey = key;
			key.OnInteract += delegate () {HandleKey(pressedkey); return false; };
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

	void SetupSymbols()
	{
		// [✓] Step 1: Generate first set, check for edge case of first set being at the last possible set of symbols
		// [✓] Step 2: Generate second set
		// [] Step 3: Check second set against first set for collisions
		// [] Step 4: Re-generate second set
		// [] Step 5: Re-check second set against first set for collisions, repeat until no collisions occur
		// [✓] Step 6: Merge first and second set's into one array, ready for going through the randomiser
		// [] Step 7: Do three more times
		for (int i = 0; i < 4; i++)
		{
			int seed1 = RNG.Range(0,25);
			int seed2 = -1;
			int[,] firstSet = new int[3,2];
			int[,] secondSet = new int[3,2];
			int[,] finalSet = new int[6,2];
			bool colliding = true;

			for (int j = 0; j < 3; j++)
			{
				while (seed1 == 24)
				{
					seed1 = RNG.Range(0,25);
				}
				firstSet[j, 0] = keypadSolvingArray[seed1, j];
				firstSet[j, 1] = j;
			}

			while (colliding == true)
			{
				while (seed2 <= seed1)
				{
					seed2 = RNG.Range(0,25);
					for (int j = 0; j < 3; j++)
					{
						secondSet[j, 0] = keypadSolvingArray[seed2, j];
						secondSet[j, 1] = j+3;
					}

					for (int index1 = 0; index1 < 3; index1++)
					{
						for (int index2 = 0; index2 < 3; index2++)
						{
							if (secondSet[index1, 1] == firstSet[index2, 1])
							{
								seed2 = RNG.Range(0,25);
								for (int j = 0; j < 3; j++)
								{
									secondSet[j, 0] = keypadSolvingArray[seed2, j];
									secondSet[j, 1] = j+3;
								}
							}
						}
					}
				}
				colliding = false;
			}




			for (int index = 0; index < 3; index++)
			{
				finalSet[index, 0] = firstSet[index, 0];
			}
			for (int index = 0; index < 3; index++)
			{
				finalSet[index, 1] = firstSet[index, 1];
			}
			for (int index = 0; index < 3; index++)
			{
				finalSet[index+3, 0] = secondSet[index, 0];
			}
			for (int index = 0; index < 3; index++)
			{
				finalSet[index+3, 1] = secondSet[index, 1];
			}

			// for (int j = 0; j < 6; j++ )
			// {
			// 	int tmp1 = finalSet[j,0];
			// 	int tmp2 = finalSet[j,1];
			// 	int r = RNG.Range(j, 6);
			// 	finalSet[j,0] = finalSet[r,0];
			// 	finalSet[r,0] = tmp1;
			// 	finalSet[j,1] = finalSet[r,1];
			// 	finalSet[r,1] = tmp2;
			// }
			Logger(String.Format("{0} {1} {2} {3} {4} {5}", finalSet[0,0], finalSet[1,0], finalSet[2,0], finalSet[3,0], finalSet[4,0], finalSet[5,0]));
			Logger(String.Format("{0} {1} {2} {3} {4} {5}", finalSet[0,1], finalSet[1,1], finalSet[2,1], finalSet[3,1], finalSet[4,1], finalSet[5,1]));

			switch (i)
			{
				// Top Left
				case 1:
				{
					for (int j = 0; j < 3; j++)
					{
						keypadAssignedSymbols[j+0, 0] = finalSet[j, 0];
						keypadAssignedSymbols[j+0, 1] = finalSet[j, 1]+0;
					}
					for (int j = 0; j < 3; j++)
					{
						keypadAssignedSymbols[j+3, 0] = finalSet[j+3, 0];
						keypadAssignedSymbols[j+3, 1] = finalSet[j+3, 1]+0;
					}
					break;
				}
				// Top Right
				case 2:
				{
					for (int j = 0; j < 3; j++)
					{
						keypadAssignedSymbols[j+3, 0] = finalSet[j, 0];
						keypadAssignedSymbols[j+3, 1] = finalSet[j, 1]+6;
					}
					for (int j = 0; j < 3; j++)
					{
						keypadAssignedSymbols[j+6, 0] = finalSet[j+3, 0];
						keypadAssignedSymbols[j+6, 1] = finalSet[j+3, 1]+6;
					}
					break;
				}
				// Bottom Left
				case 3:
				{
					for (int j = 0; j < 3; j++)
					{
						keypadAssignedSymbols[j+12, 0] = finalSet[j, 0];
						keypadAssignedSymbols[j+12, 1] = finalSet[j, 1]+12;
					}
					for (int j = 0; j < 3; j++)
					{
						keypadAssignedSymbols[j+15, 0] = finalSet[j+3, 0];
						keypadAssignedSymbols[j+15, 1] = finalSet[j+3, 1]+12;
					}
					break;
				}
				// Bottom Right
				case 4:
				{
					for (int j = 0; j < 3; j++)
					{
						keypadAssignedSymbols[j+15, 0] = finalSet[j, 0];
						keypadAssignedSymbols[j+15, 1] = finalSet[j, 1]+18;
					}
					for (int j = 0; j < 3; j++)
					{
						keypadAssignedSymbols[j+18, 0] = finalSet[j+3, 0];
						keypadAssignedSymbols[j+18, 1] = finalSet[j+3, 1]+18;
					}
					break;
				}
			}
		}
	}

	void SetAll2x3Quadrants()
	{
		for (int i = 0; i < 24; i++)
		{
			SetLabel(keypad[i], keypadAssignedSymbols[i, 0]);
		}
	}

	// Helper functions for doing stuff
	void SetLabel(KMSelectable key, int labelIndex)
	{
		key.transform.Find("label").GetComponent<MeshRenderer>().material = keypadLabelMats[labelIndex-1];
	}

	void SetLED(KMSelectable key, int status)
	{
		// 0 = Off, 1 = Correct and 2 = Wrong
		key.transform.Find("led").GetComponent<MeshRenderer>().material = keypadLedMats[status];
	}

	void Strike(KMSelectable key)
	{
		GetComponent<KMBombModule>().HandleStrike();
		StartCoroutine(StrikeLEDSet(key));
		stageNumber = 0;
	}

	IEnumerator StrikeLEDSet(KMSelectable key)
	{
		foreach (KMSelectable currentkey in keypad)
		{
			SetLED(currentkey, 0);
		}
		SetLED(key, 2);
		yield return new WaitForSeconds(1f);
		SetLED(key, 0);
	}

	void HandleKey(KMSelectable key)
	{		
		if (moduleSolved != true)
		{
			LoggerKeypress(key, "Wrong");
			Strike(key);
		}
	}
}