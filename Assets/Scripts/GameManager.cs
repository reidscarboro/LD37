using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public int levelIndex = 0;

    public int numRogues = 0;
    public int numKnights = 0;
    public int numPeasants = 0;
    public int numArchers = 0;

    public int addRogues = 0;
    public int addKnights = 0;
    public int addPeasants = 0;
    public int addArchers = 0;

    public int numZombies = 0;
    public int numFastZombies = 0;

    public static GameManager instance;

	void Awake() {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        if (instance.levelIndex == 0) {
            LoadNextLevel();
        }
    }
    
    public static void LoadNextLevel() {

        instance.numRogues = FindObjectsOfType(typeof(UnitRogue)).Length;
        instance.numKnights = FindObjectsOfType(typeof(UnitKnight)).Length;
        instance.numPeasants = FindObjectsOfType(typeof(UnitPeasant)).Length;
        instance.numArchers = FindObjectsOfType(typeof(UnitArcher)).Length;

        instance.addPeasants = 0;
        instance.addKnights = 0;
        instance.addArchers = 0;
        instance.addRogues = 0;

        instance.levelIndex++;
        switch (instance.levelIndex) {
            case 1:
                instance.addPeasants = 3;
                break;
            case 2:
                instance.addKnights = 1;
                instance.addPeasants = Random.Range(0, 3);
                break;
            case 3:
                instance.addArchers = 1;
                instance.addPeasants = Random.Range(0, 3);
                break;
            case 4:
                instance.addRogues = 2;
                break;
            default:

                if (instance.numPeasants < 3) {
                    if (Random.Range(0.0f, 1.0f) > 0.3f) instance.addPeasants += 1;
                    if (Random.Range(0.0f, 1.0f) > 0.7f) instance.addPeasants += 1;
                }
                if (instance.numKnights < 3) {
                    if (Random.Range(0.0f, 1.0f) > 0.7f) instance.addKnights += 1;
                    if (Random.Range(0.0f, 1.0f) > 0.85f) instance.addKnights += 1;
                }
                if (instance.numArchers < 3) {
                    if (Random.Range(0.0f, 1.0f) > 0.5f) instance.addArchers += 1;
                    if (Random.Range(0.0f, 1.0f) > 0.7f) instance.addArchers += 1;
                }
                if (instance.numRogues < 3) {
                    if (Random.Range(0.0f, 1.0f) > 0.7f) instance.addRogues += 1;
                    if (Random.Range(0.0f, 1.0f) > 0.85f) instance.addRogues += 1;
                }

                break;
        }

        SceneManager.LoadScene("transition");
    }

    public static void LoadGame() {
        instance.numPeasants += instance.addPeasants;
        instance.addPeasants = 0;

        instance.numKnights += instance.addKnights;
        instance.addKnights = 0;

        instance.numArchers += instance.addArchers;
        instance.addArchers = 0;

        instance.numRogues += instance.addRogues;
        instance.addRogues = 0;

        SceneManager.LoadScene("game");
    }


    public static void GameOver() {
        SceneManager.LoadScene("gameOver");
    }

    public static void NewGame() {
        instance.levelIndex = 0;
        LoadNextLevel();
    }

    public static GameManager GetInstance() {
        return instance;
    }
}
