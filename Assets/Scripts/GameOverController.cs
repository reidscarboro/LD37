using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour {

    public Text gameOverText;

	void Start() {
        int days = GameManager.GetInstance().levelIndex - 1;
        gameOverText.text = "You survived " + days + " days";
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) GameManager.NewGame();
    }
}
