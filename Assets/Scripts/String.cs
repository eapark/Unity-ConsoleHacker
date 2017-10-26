using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonType {
	answer,
	dud,
	hint,
	none
}

public class Word {
	public ButtonType buttonType;
	public string buttonText;
	public GameObject wordButton;

	public Word(ButtonType wordType, string wordText) {
		buttonType = wordType;
		buttonText = wordText;

	}
}
