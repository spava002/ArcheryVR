using System;
using System.Collections;
using UnityEngine;

public class ButtonManager : MonoBehaviour {
    public enum Menu {
        Main,
        Map,
        PostMatch
    }

    [SerializeField] Menu currentMenu;
    [Tooltip("Place buttons in order from top to bottom, left to right for proper functionality. In the case of maps menu, place live match maps first and then practice maps last.")]
    [SerializeField] InteractableButton[] interactableButtons;

    [Header("Main Menu Actions")]
    [Tooltip("Place menus in order of maps, difficulties, and modes for the actions to work properly.")]
    [SerializeField] GameObject[] menuScreens;

    [Header("Map Menu Actions")]
    [Tooltip("Place first 3 live match maps, followed by the next 3 pratice maps. If no map available, use 'None'.")]
    [SerializeField] string[] maps;

    [Header("Post Match Menu Actions")]
    [SerializeField] bool[] continueGame;

    LevelLoader levelLoader;
    TurnManager turnManager;
    InterMatchController interMatchController;

    void Start() {
        levelLoader = FindAnyObjectByType<LevelLoader>();
        turnManager = FindAnyObjectByType<TurnManager>();
        interMatchController = FindAnyObjectByType<InterMatchController>();

        if (currentMenu == Menu.Main) {
            // Set the default menu screens that will be visible/invisible
            foreach (GameObject menuScreen in menuScreens) {
                menuScreen.SetActive(false);
            }
        }    
    }

    public void ApplyButtonAction(InteractableButton pressedInteractableButton) {
        int i = 0;
        foreach (InteractableButton interactableButton in interactableButtons) {
            if (interactableButton == pressedInteractableButton) {
                break;
            }
            i++;
        }

        if (currentMenu == Menu.Main) {
            if (i < 3) {
                LoadMenu(menuScreens[i]);
            }
            else if (i == 3) {
                // Disable the collider, so button cant be pressed more than once
                interactableButtons[interactableButtons.Length - 1].GetComponent<Collider>().enabled = false;
                // Since we destroy duplicate level loaders on Awake, the reference may be lost, so we need to check if its null and replace it if so
                if (!levelLoader) {
                    levelLoader = FindAnyObjectByType<LevelLoader>();
                }
                levelLoader.LoadChosenLevel();
            }
            else {
                // Last menu button exits the player out of the game
                Application.Quit();
            }
        }
        else if (currentMenu == Menu.Map) {
            PlayerPrefs.SetString("Map", maps[i]);
        }
        else if (currentMenu == Menu.PostMatch) {
            if (continueGame[i]) {
                // Since the interMatchController disable the Post Match menu, the button audio is never heard
                // To allow the audio to play AND THEN disable the menu, we use a coroutine that delays disabling until the audio stops
                StartCoroutine(WaitAndApplyAction(interactableButtons[i].GetComponent<AudioSource>()));
            }
            else {
                // Toggle the quiver's spawn arrow script to disable it
                FindAnyObjectByType<QuiverManager>().ToggleSpawnArrow();
                // Load back to the main menu
                PlayerPrefs.SetString("Map", "Main Menu");
                interactableButtons[interactableButtons.Length - 1].GetComponent<Collider>().enabled = false;
                levelLoader.LoadChosenLevel();
            }
        }
    }

    void LoadMenu(GameObject menu) {
        foreach (GameObject currentMenu in menuScreens) {
            if (currentMenu != menu) {
                currentMenu.SetActive(false);
            }
            else {
                currentMenu.SetActive(true);
            }
        }
    }

    IEnumerator WaitAndApplyAction(AudioSource buttonAudioSource) {
        while (buttonAudioSource.isPlaying) {
            yield return null;
        }

        // Continue the game by putting it into intermission
        // Only way to break out of intermission is by grabbing the bow
        interMatchController.InitiateIntermissionMatchSequence();
    }
}
