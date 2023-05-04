using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject m_ExitButton;

    [Header("Audio")]
    [SerializeField] private AudioSource m_AudioSource;

    [Header("Characters")]
    [SerializeField] private List<CharacterSlot> m_OpponentSlots;

    private void Start()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        const bool IS_EXIT_BUTTON_VISIBLE = true;
#else
        const bool IS_EXIT_BUTTON_VISIBLE = false;
#endif

        m_ExitButton.SetActive(IS_EXIT_BUTTON_VISIBLE);

        for (int i = 0; i < m_OpponentSlots.Count; i++)
        {
            CharacterSlot currentCharSlot = m_OpponentSlots[i];
            currentCharSlot.OnCharacterTypeChanged += OnCharacterTypeChanged;
        }
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

