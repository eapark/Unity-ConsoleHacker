using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random=UnityEngine.Random;

public class Game : MonoBehaviour {
	private string[,] strings;
	private Word[] words = new Word[100];
	private int tries = 5;
	public bool answerFound = false;

	public GameObject Panel;

	// Use this for initialization
	void Start () {
		// Initialize array of strings
		// A row has 12 characters (ie. length of string)
		// There are 17 rows * 2 = 34 rows total

		// Choose 15 random words
		int numWords = 3;
		string[] words = new string[]{"hello", "mello", "jello"};
		int wordLength = words [0].Length;

		// Randomly choose an answer
		string answer = words[ Random.Range(0, numWords) ];

		// Make hints, which are special characters enclosed in brackets
		// and which remove a wrong password or give an extra try upon clicking
		char[] specialChar = new char[]{ '*', '/', '@', '$', '=', '^', '.', ',', '-', '!', ';', ':', '+', '%' };
		char[] openBracket = new char[]{ '[', '(', '<' };
		char[] closeBracket = new char[]{ ']', ')', '>' };
		int numBracket = openBracket.Length;
		int numDuds = numWords / 2;
		string[] duds = new string[numDuds];
		for (int i = 0; i < numDuds; i++) {
			int bracket = Random.Range(0, numBracket);
			string dud = "";
			dud += openBracket[bracket];
			int dudLength = Random.Range(1, 6);
			for (int d = 0; d < dudLength; d++) {
				dud += specialChar[ Random.Range(0, specialChar.Length) ];
			}
			dud += closeBracket[bracket];
			duds[i] = dud;
		}

		// Randomly choose which row the words and the hints belong on


		// Fill the rest with a random special char

		// Create buttons out of the array

		// Create two panels, left and right side of the terminal
		GameObject canvas = GameObject.Find("Canvas");

		GameObject leftPanel = (GameObject)Instantiate (Panel);
		GameObject rightPanel = (GameObject)Instantiate (Panel);

		leftPanel.name = "leftPanel";
		rightPanel.name = "rightPanel";

		int width = Screen.width;
		int height = Screen.height;

		RectTransform leftRect = leftPanel.GetComponent<RectTransform> ();
		RectTransform rightRect = rightPanel.GetComponent<RectTransform> ();

		leftRect.sizeDelta = new Vector2 (width/2,height/2);
		rightRect.sizeDelta = new Vector2 (width/2,height/2);

		leftRect.position = new Vector2 (0,0);
		rightRect.position = new Vector2 (width/2,height/2);

		leftPanel.transform.SetParent (canvas.transform, false);
		rightPanel.transform.SetParent (canvas.transform, false);
	}

	// Update is called once per frame
	void Update () {

	}

	void onButtonClick() {
		// Check what kind of word was pressed
	}
}
