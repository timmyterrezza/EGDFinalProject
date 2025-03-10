﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartMenu : MonoBehaviour {
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject initialText;
    [SerializeField] private GameObject checkBox;

    private RectTransform checkBoxTransform;

    private bool secondScreen;

    private float startTime;

    [SerializeField] private int mouseIndex;

    private void Start() {
        startTime = Time.time;
        mouseIndex = 0;
        checkBoxTransform = checkBox.GetComponent<RectTransform>();
        secondScreen = false;
    }

    private void Update() {
        if (Input.GetButtonDown(buttonName:"Submit")) {
            if (secondScreen) {
                if (mouseIndex == 0) {
                    GoContinue();
                }
                else if (mouseIndex == 1) {
                    GoNewGame();
                }
                else {
                    GoExit();
                }
            }
            else {
                continueButton.SetActive(true);
                continueButton.GetComponent<Button>();
                newGameButton.SetActive(true);
                exitButton.SetActive(true);
                checkBox.SetActive(true);
                initialText.SetActive(false);
                secondScreen = true;
            }
        }
        if (mouseIndex == 0) {
            if (Input.GetAxisRaw("Vertical") < 0 && Time.time - startTime > 0.5f) {
                mouseIndex = 1;
                startTime = Time.time;
            }
            else if (Input.GetAxisRaw("Vertical") > 0 && Time.time - startTime > 0.5f) {
                mouseIndex = 2;
                startTime = Time.time;
            }
            checkBoxTransform.anchorMin = new Vector2(0.8017539f,0.204f);
            checkBoxTransform.anchorMax = new Vector2(0.8402153f, 0.3047594f);
        }
        else if (mouseIndex == 1) {
            if (Input.GetAxisRaw("Vertical") < 0 && Time.time - startTime > 0.5f) {
                mouseIndex = 2;
                startTime = Time.time;
            }
            else if (Input.GetAxisRaw("Vertical") > 0 && Time.time - startTime > 0.5f) {
                mouseIndex = 0;
                startTime = Time.time;
            }
            checkBoxTransform.anchorMin = new Vector2(0.8017539f,0.1027469f);
            checkBoxTransform.anchorMax = new Vector2(0.84021532f, 0.204f);
        }
        else {
            if (Input.GetAxisRaw("Vertical") < 0 && Time.time - startTime > 0.5f) {
                mouseIndex = 0;
                startTime = Time.time;
            }
            else if (Input.GetAxisRaw("Vertical") > 0 && Time.time - startTime > 0.5f) {
                mouseIndex = 1;
                startTime = Time.time;
            }
            checkBoxTransform.anchorMin = new Vector2(0.8017539f,0.0f);
            checkBoxTransform.anchorMax = new Vector2(0.84021532f, 0.1027469f);
        }

    }

    public void GoContinue() {
        if (PlayerPrefs.GetInt("level") != 0) {
            Application.LoadLevel(PlayerPrefs.GetInt("level"));
        }
        else {
            Application.LoadLevel(1);
        }
    }

    public void GoNewGame() {
        Application.LoadLevel(1);
    }

    public void GoExit() {
        Application.Quit();
    }
}
