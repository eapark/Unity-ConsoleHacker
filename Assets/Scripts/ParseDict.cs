using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class ParseDict {
	// Source for the list of passwords: https://pastebin.com/rgskRrgv

	// dictionary where key is length of word, key is list of all words of the length
	public Dictionary<int, List<string> > passwords;

	private string answer;

	public ParseDict(){
		passwords = new Dictionary<int, List<string> >();
		for (int i = 0; i <= 20; i++) {
			passwords [i] = new List<string> ();
		}

		// Read from file and parse
		string fileName = "Assets/Resources/falloutdict.txt";

		StreamReader reader = new StreamReader (fileName, Encoding.Default);

		while (!reader.EndOfStream) {
			string line = reader.ReadLine ();
			string[] splitResult = line.Split (' ');

			// Add to our dict passwords
			foreach (string word in splitResult) {
				passwords [word.Length].Add (word);
			}
		}
	}

	public List<string> SelectWords(string difficulty){
		int numWords = 15;
		int selectedWordLength = 2;

		// Difficulties and corresponding password lengths are from Fallout 4
		if (System.String.Compare (difficulty, "novice", true) == 1) {
			selectedWordLength = Random.Range (4, 5);
		}
		else if (System.String.Compare(difficulty, "advanced", true) == 1){
			selectedWordLength = Random.Range (6, 8);
		}
		else if(System.String.Compare(difficulty, "expert", true) == 1){
			selectedWordLength = Random.Range (9, 10);
		}
		else if(System.String.Compare(difficulty, "master", true) == 1){
			selectedWordLength = Random.Range (11, 12);
		}

		// All possible words of length 'selectedWordLength'
		List<string> words = new List<string> (passwords [selectedWordLength]);


		// Randomly select 15 and return
		// Make sure all words have at least 1 shared letter with the answer (first word)
		List<string> selectedWords = new List<string>();
		for (int i = 0; i < numWords; i++) {
			int wordIndex = Random.Range (0, words.Count);

			if (i == 0) {
				answer = words [wordIndex];
			}

			while( selectedWords.Contains( words[wordIndex] ) || !Similarity( words[wordIndex] ) ){
				wordIndex = Random.Range (0, words.Count);
			}
			selectedWords.Add (words[wordIndex]);
		}

		return selectedWords;
	}

	private bool Similarity(string word){
		for (int c = 0; c < word.Length; c++) {
			if (word[c] == answer [c]) {
				return true;
			}
		}
		return false;
	}
}
