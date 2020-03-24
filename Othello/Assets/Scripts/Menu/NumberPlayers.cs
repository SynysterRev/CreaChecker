using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberPlayers : MonoBehaviour
{
    [SerializeField] GameObject selectionPlayer;
    [SerializeField] GameObject selectionDifficulty;

    public void SaveNumberPlayer(int number)
    {
        PlayerPrefs.SetInt("IsTwoPlayers", number);
    }

    public void SaveDifficultyPlayer(int difficulty)
    {
        PlayerPrefs.SetInt("Difficulty", difficulty);
    }

    public void SelectionDifficulty()
    {
        SoundManager.instance.PlayButtonSound();
        selectionPlayer.SetActive(!selectionPlayer.activeSelf);
        selectionDifficulty.SetActive(!selectionDifficulty.activeSelf);
    }
}
