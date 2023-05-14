using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject m_ExitButton;
    [SerializeField] private GameObject m_PlayButton;
    [SerializeField] private GameObject m_Instructions;

    [Header("Audio")]
    [SerializeField] private AudioSource m_AudioSource;

    [Header("Characters")]
    [SerializeField] private List<CharacterSlot> m_CharacterSlots;
    [SerializeField] private GameObject m_BlueCrown;
    [SerializeField] private GameObject m_RedCrown;

    private void Start()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        const bool IS_EXIT_BUTTON_VISIBLE = true;
#else
        const bool IS_EXIT_BUTTON_VISIBLE = false;
#endif

        m_ExitButton.SetActive(IS_EXIT_BUTTON_VISIBLE);

        for (int i = 0; i < m_CharacterSlots.Count; i++)
        {
            CharacterSlot currentCharSlot = m_CharacterSlots[i];
            LoadCharacter(currentCharSlot);
            currentCharSlot.OnCharacterTypeChanged += OnCharacterTypeChanged;
        }

        CheckForCrown();
    }

    private void LoadCharacter(CharacterSlot slot)
    {
        slot.LoadCharacter();
        EnablePlayButton();
    }

    private void CheckForCrown()
    {
        m_BlueCrown.SetActive(GameSettings.Winner == EPlayerColour.Blue);
        m_RedCrown.SetActive(GameSettings.Winner == EPlayerColour.Red);
    }

    private void OnCharacterTypeChanged(CharacterTypeHolder characterType, CharacterSlot characterSlot)
    {
        if (characterSlot.PlayerColour == EPlayerColour.Blue)
        {
            GameSettings.BluePlayer = characterType.CharacterType;
        }
        else
        {
            GameSettings.RedPlayer = characterType.CharacterType;
        }

        EnablePlayButton();
    }

    private void EnablePlayButton()
    {
        if (GameSettings.BluePlayer != EPlayerType.Invalid && GameSettings.RedPlayer != EPlayerType.Invalid)
        {
            m_PlayButton.SetActive(true);
            m_Instructions.SetActive(false);
        }
        else
        {
            m_PlayButton.SetActive(false);
            m_Instructions.SetActive(true);
        }
    }

    public void LoadScene()
    {
        SceneController.ChangeScene(EScene.Main);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); 
#endif
    }
}

