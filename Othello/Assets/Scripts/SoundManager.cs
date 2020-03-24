using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    public enum TYPESOUND { CREATEPIECE, DESTROYPIECE };
    public enum TYPESOUNDMENU { BUTTON };
    AudioSource source;
    [SerializeField] AudioClip[] soundsGame;
    [SerializeField] AudioClip[] soundsMenu;
    // Use this for initialization

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlaySound(TYPESOUND typeSound)
    {
        if (source == null)
        {
            source = Camera.main.GetComponent<AudioSource>();
        }
        if ((int)typeSound < soundsGame.Length)
            source.PlayOneShot(soundsGame[(int)typeSound], 0.5f);
    }
    public void PlaySoundMenu(TYPESOUNDMENU typeSound)
    {
        if (source == null)
        {
            source = Camera.main.GetComponent<AudioSource>();
        }
        if ((int)typeSound < soundsMenu.Length)
            source.PlayOneShot(soundsMenu[(int)typeSound]);
    }

    public void PlayButtonSound()
    {
        if (source == null)
        {
            source = Camera.main.GetComponent<AudioSource>();
        }
        if ((int)TYPESOUNDMENU.BUTTON < soundsMenu.Length)
            source.PlayOneShot(soundsMenu[(int)TYPESOUNDMENU.BUTTON]);
    }

}
