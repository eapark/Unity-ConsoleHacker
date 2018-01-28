using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using Random=UnityEngine.Random;

public class LineClass {
	public int length; // sum of all strings in wordList
	public List<string> wordList; // the strings in a line

	public LineClass() {
		length = 0;
		wordList = new List<string>();
	}
}

public class Game : MonoBehaviour {
	private LineClass[] strings; // array of LineClass
	private List<string> selectedWords;
	private int maxTries = 4;
	private int tries = 4;
	public bool answerFound = false;
	private bool triesReset = false;
	public string answer;
	private int answerRow;
	private List<int> wordRows; // list of row nums that the words belong in
	private List<int> hintRows; // list of row nums that the hints belong in

	private GameObject canvas, ProgressPanel, leftPanel, rightPanel, fullPanel;
	private GameObject[] panels; // leftPanel and rightPanel

	private Queue<string> progressQueue;

	private int numPanel = 2;
	private int numLine  = 17;
	private int numChar  = 12;
	private int wordLength;
	private GameObject[] lines; // array of Line Prefabs

	private string difficulty = "novice"; // default difficulty

	// Use this for initialization
	void Start () {
	}

	void SetupGame() {
		ParseDict newDict = new ParseDict ();

		// Get in-game canvas and panels
		canvas = GameObject.Find("Canvas");
		leftPanel = GameObject.Find ("LeftPanel");
		rightPanel = GameObject.Find ("RightPanel");
		ProgressPanel = GameObject.Find ("ProgressPanel");

		lines = new GameObject[numPanel * numLine];
		panels = new GameObject[]{leftPanel, rightPanel};

		// For showing the progress of the game on the right side
		progressQueue = new Queue<string> ();

		InitializePanels ();

		// Initialize array of LineClass, which is each row of text on the console
		// A line has 12 characters (ie. length of string)
		// There are 17 lines * 2 = 34 lines total
		strings = new LineClass[numPanel * numLine];
		for (int l = 0; l < strings.Length; l++) {
			strings [l] = new LineClass ();
		}

		tries = maxTries;

		// Get selected words
		selectedWords = new List<string>( newDict.SelectWords(difficulty) );
		int numWords = selectedWords.Count;
		wordLength = selectedWords[0].Length;

		answer = selectedWords[0];

		// Make hints, which are special characters enclosed in brackets
		// and which remove a wrong password or give an extra try upon clicking
		char[] specialChar = new char[]{ '*', '/', '@', '$', '=', '^', '.', ',', '-', '!', ';', ':', '+', '%' };
		char[] openBracket = new char[]{ '[', '(', '<' };
		char[] closeBracket = new char[]{ ']', ')', '>' };

		List<char> allSpecialChar = new List<char>();
		for (int c = 0; c < specialChar.Length; c++) {
			allSpecialChar.Add (specialChar[c]);
		}
		for (int b = 0; b < openBracket.Length; b++) {
			allSpecialChar.Add (openBracket [b]);
			allSpecialChar.Add (closeBracket [b]);
		}

		int numBracket = openBracket.Length;
		int numHints = numWords / 2;
		string[] hints = new string[numHints];
		for (int i = 0; i < numHints; i++) {
			int bracket = Random.Range(0, numBracket);
			string hint = "";
			hint += openBracket[bracket];
			int hintLength = Random.Range(0, numChar - wordLength - 2 + 1);
			// max length is numChar - wordLength - 2 to prevent a line with one word and one hint from 'overflowing'
			// subtract by 2 for the brackets, add 1 because Random.Range(min, max) is max-exclusive

			for (int h = 0; h < hintLength; h++) {
				hint += specialChar[ Random.Range(0, specialChar.Length) ];
			}
			hint += closeBracket[bracket];
			hints[i] = hint;
		}

		// Randomly choose which row the words belong in
		wordRows = new List<int>();
		for (int w = 0; w < numWords; w++) {
			int tempRow = Random.Range (0, numPanel * numLine);
			// No two words in one row
			while(wordRows.Contains(tempRow)){
				tempRow = Random.Range (0, numPanel * numLine);
			}
			wordRows.Add(tempRow);
			strings [tempRow].length += wordLength;
			strings [tempRow].wordList.Add (selectedWords[w]);
		}

		// Randomly choose which row the hints belong in
		hintRows = new List<int>();
		for (int h = 0; h < numHints; h++) {
			int tempRow = Random.Range (0, numPanel * numLine);
			// No two hints in one row
			while(hintRows.Contains(tempRow)){
				tempRow = Random.Range (0, numPanel * numLine);
			}

			hintRows.Add(tempRow);
			strings [tempRow].length += hints[h].Length;
			strings [tempRow].wordList.Add (hints[h]);
		}

		// Fill the rest with a random special char
		for (int l = 0; l < strings.Length; l++) {
			while (strings [l].length < numChar) {
				string randomChar = allSpecialChar[ Random.Range(0, allSpecialChar.Count) ] + "";
				strings [l].wordList.Add (randomChar);
				strings [l].length++;
			}
		}

		// Create buttons out of the array, put them in the Lines in the Panels
		for (int l = 0; l < lines.Length; l++) {
			int count = strings [l].wordList.Count;
			while (count > 0) {
				// Randomly select a word from strings[l].wordList
				int rand = Random.Range (0, count);
				// Create String Prefab
				GameObject newString = (GameObject)Instantiate(Resources.Load("String"));
				newString.name = "string"+l+"_"+count;
				Button newStringButton = newString.GetComponent<Button> ();
				newString.transform.SetParent (lines[l].transform);
				Text newStringText = newString.transform.GetChild(0).transform.GetComponentInChildren<Text>();
				newStringText.text = strings[l].wordList[rand];

				// Update answerRow
				if (strings [l].wordList [rand].Equals(answer)) {
					answerRow = l;
				}

				newStringButton.onClick.AddListener (() => onButtonClick( newString ) );
				// Remove the randomly selected word from wordList
				strings [l].wordList.RemoveAt (rand);
				// Update count for while loop
				count = strings [l].wordList.Count;
			}

		}

		UpdateProgressPanelText ();
	}

	// Fill left and right panels with Line prefabs and add the prefab to 'lines'
	void InitializePanels() {
		for (int p = 0; p < numPanel; p++) {
			GameObject panel = panels [p];
			for (int l = 0; l < numLine; l++) {
				GameObject line = (GameObject)Instantiate (Resources.Load("Line"));
				line.name = "line" + p + "_" + l;
				lines[ p*numLine + l ] = line;
				line.transform.SetParent(panel.transform); // make the line a child of the panel
			}
		}
	}

	// Update is called once per frame
	void Update () {
	}

	void onButtonClick( GameObject clickedString ) {
		Text clickedStringText = clickedString.transform.GetChild(0).transform.GetComponentInChildren<Text>();
		string buttonText = clickedStringText.text;

		bool isLetter = System.Char.IsLetter ( buttonText[0] );

		// Compare the number of letters shared between answer and buttonText.
		// Show this number if buttonText isn't the answer
		int likeliness = 0;
		for (int c = 0; c < buttonText.Length && isLetter ; c++) {
			if (buttonText [c] == answer [c]) {
				likeliness++;
			}
		}

		if (likeliness == wordLength) {
			tries--;
			answerFound = true;
			EndGame (true);
			return;
		}
		else if ( likeliness != wordLength && isLetter ){
			tries--;

			progressQueue.Enqueue (buttonText);
			progressQueue.Enqueue ("Entry Denied.");
			progressQueue.Enqueue ("Likeliness: " + likeliness + "/" + wordLength);
		}

		// Check if tries is 0
		if (tries == 0) {
			EndGame (false);
			return;
		}

		// Check if hint was pressed (ie. buttonText is longer than 1 char and is punctuation)
		// If true, remove a non-answer or by a 20% chance, reset tries (but tries is reset only once)
		if (buttonText.Length > 1 && !isLetter) {
			progressQueue.Enqueue(buttonText);

			float chance = Random.Range (0.0f, 1.0f);
			if (chance > 0.2f) {
				// Randomly choose a dud to remove
				int dudRow = Random.Range(0, wordRows.Count);
				while (dudRow != answerRow) {
					dudRow = Random.Range(0, wordRows.Count);
				}

				// Find and deactivate the dud found in wordRows[dudRow]-th line
				foreach (Transform child in lines[ wordRows[dudRow] ].transform) {
					Text childText = child.GetComponentInChildren<Text> ();
					if ( System.Char.IsLetter(childText.text [0]) ) {
						Button childButton = child.GetComponent<Button> ();
						DeactivateButton (childButton);

						wordRows.RemoveAt (dudRow);
					}
				}
				progressQueue.Enqueue ("Dud Removed.");
			}
			else {
				if (!triesReset) {
					tries = maxTries;
					progressQueue.Enqueue ("Tries Reset.");
					triesReset = true;
				}
			}
		}

		// Make pressed button unclickable
		Button clickedButton = clickedString.GetComponent<Button> ();
		if (buttonText.Length > 1) { // Nothing happens if you press on a single symbol
			DeactivateButton (clickedButton);
		}

		UpdateProgressPanelText ();
	}

	void DeactivateButton(Button toDeactivate){
		toDeactivate.interactable = false;

		Text clickedStringText = toDeactivate.transform.GetChild(0).transform.GetComponentInChildren<Text>();
		string buttonText = clickedStringText.text;

		// Replace the pressed button's text with underscore
		string underscore = "";
		for (int c = 0; c < buttonText.Length; c++) {
			underscore += "_";
		}
		clickedStringText.text = underscore;
	}

	void UpdateProgressPanelText(){
		Text GameProgress = ProgressPanel.transform.GetChild(0).transform.GetComponentInChildren<Text>();

		// Dequeue until progressQueue's size is equal to numLine
		while(progressQueue.Count > numLine){
			progressQueue.Dequeue ();
		}

		Queue<string> progressQueueClone = new Queue<string> (progressQueue);

		string progressText = "";
		while(progressQueueClone.Count > 0) {
			string temp = ">" + progressQueueClone.Dequeue() + "\n";
			progressText += temp;
		}

		string triesText = "Tries: ";
		triesText += tries + "/" + maxTries + "\n";

		GameProgress.text = triesText + progressText;
	}

	public void OnDifficultySelected(Button button){
		difficulty = button.name;

		// Clear everything in FullPanel
		fullPanel = GameObject.Find("FullPanel");
		foreach (Transform child in fullPanel.transform) {
			Destroy (child.gameObject);
		}

		fullPanel.SetActive (false);
		SetupGame ();
	}

	public void OnRestartClicked() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void EndGame(bool victory){
		// Clear all buttons
		for (int l = 0; l < lines.Length; l++) {
			foreach (Transform child in lines[ l ].transform) {
				Destroy (child.gameObject);
			}
		}

		leftPanel.SetActive (false);
		rightPanel.SetActive (false);

		fullPanel.SetActive (true);

		string result = "";

		if (victory) {
			result = "Access Granted!";
		} else {
			result = "Access Denied";
		}

		GameObject resultText = new GameObject ("resultText");
		resultText.transform.SetParent (fullPanel.transform);

		Text txt = resultText.AddComponent<Text> ();
		txt.text = result;
		txt.font = Resources.Load<Font> ("Font/Consolas");
		txt.fontSize = 20;
		txt.color = Color.green;
		txt.resizeTextForBestFit = true;
		txt.horizontalOverflow = HorizontalWrapMode.Overflow;
		txt.alignment = TextAnchor.MiddleCenter;

		// Add restart button
		GameObject restart = (GameObject)Instantiate(Resources.Load("String"));
		restart.name = "restart";
		Button restartButton = restart.GetComponent<Button> ();
		restart.transform.SetParent (fullPanel.transform);
		Text restartText = restart.transform.GetChild(0).transform.GetComponentInChildren<Text>();
		restartText.text = "Restart";

		restartButton.onClick.AddListener (() => OnRestartClicked() );
	}
}
