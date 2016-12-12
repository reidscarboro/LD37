using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour {

    public GameObject peasant;
    public GameObject knight;
    public GameObject archer;
    public GameObject rogue;

    public Text numPeasant;
    public Text numKnight;
    public Text numArcher;
    public Text numRogue;

    public Text addPeasant;
    public Text addKnight;
    public Text addArcher;
    public Text addRogue;

    public Text storyText;

    public Text dayText;

    public List<string> story;

	void Start() {
        int levelIndex = GameManager.GetInstance().levelIndex;

        dayText.text = "Night " + levelIndex.ToString();

        if (levelIndex < story.Count + 1) {
            storyText.text = story[levelIndex - 1];
            storyText.gameObject.SetActive(true);
        }

        switch (levelIndex) {
            case 1:
                peasant.SetActive(true);
                break;
            case 2:
                peasant.SetActive(true);
                knight.SetActive(true);
                break;
            case 3:
                peasant.SetActive(true);
                knight.SetActive(true);
                archer.SetActive(true);
                break;
            default:
                peasant.SetActive(true);
                knight.SetActive(true);
                archer.SetActive(true);
                rogue.SetActive(true);
                break;
        }

        numPeasant.text = GameManager.GetInstance().numPeasants.ToString();
        numKnight.text = GameManager.GetInstance().numKnights.ToString();
        numArcher.text = GameManager.GetInstance().numArchers.ToString();
        numRogue.text = GameManager.GetInstance().numRogues.ToString();

        if (GameManager.GetInstance().addPeasants > 0) {
            addPeasant.text = "+" + GameManager.GetInstance().addPeasants.ToString();
            addPeasant.gameObject.SetActive(true);
        }

        if (GameManager.GetInstance().addKnights > 0) {
            addKnight.text = "+" + GameManager.GetInstance().addKnights.ToString();
            addKnight.gameObject.SetActive(true);
        }

        if (GameManager.GetInstance().addArchers > 0) {
            addArcher.text = "+" + GameManager.GetInstance().addArchers.ToString();
            addArcher.gameObject.SetActive(true);
        }

        if (GameManager.GetInstance().addRogues > 0) {
            addRogue.text = "+" + GameManager.GetInstance().addRogues.ToString();
            addRogue.gameObject.SetActive(true);
        }

    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            GameManager.LoadGame();
        }
    }
}
