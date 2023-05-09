using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject m_ExitButton;
    [SerializeField] private GameObject m_PlayButton;

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
        m_BlueCrown.SetActive(GameSettings.Winner == EPlayer.Blue);
        m_RedCrown.SetActive(GameSettings.Winner == EPlayer.Red);
    }

    private void OnCharacterTypeChanged(CharacterTypeHolder characterType, CharacterSlot characterSlot)
    {
        if (characterSlot.PlayerColour == EPlayer.Blue)
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
        if (GameSettings.BluePlayer != ECharacterType.None && GameSettings.RedPlayer != ECharacterType.None)
        {
            m_PlayButton.SetActive(true);
        }
        else
        {
            m_PlayButton.SetActive(false);
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

