using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject m_ExitButton;
    [SerializeField] private GameObject m_PlayButton;
    [SerializeField] private GameObject m_Instructions;

    [Header("Settings")]
    [SerializeField] private GameObject m_SettingsButton;
    [SerializeField] private GameObject m_SettingsScreen;

    [Header("Tutorial")]
    [SerializeField] private TutorialManager m_TutorialManager;

    [Header("Characters")]
    [SerializeField] private List<CharacterSlot> m_CharacterSlots;
    public List<CharacterSlot> CharacterSlots => m_CharacterSlots;

    [SerializeField] private List<Character> m_CharacterTypes;
    public List<Character> CharacterTypes => m_CharacterTypes;
    [SerializeField] private ParticleSystem m_BlueRingParticleSystem;
    [SerializeField] private ParticleSystem m_RedRingParticleSystem;
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
        }

        CharacterSlot.OnCharacterTypeChanged += OnCharacterTypeChanged;

        m_CharacterTypes.ForEach(character => { character.OnCharacterGrabbed += OnCharacterGrabbed; character.OnCharacterReleased += OnCharacterReleased; });

        CheckForCrown();

        if (!TutorialHelper.HasCompletedTutorial)
        {
            m_TutorialManager.StartTutorial();
        }
    }

    private void OnCharacterGrabbed(Character character)
    {
        SetCharacterRings(false);
        SetSlotRings(true);
    }

    private void OnCharacterReleased()
    {
        SetCharacterRings(true);
        SetSlotRings(false);
    }

    private void LoadCharacter(CharacterSlot slot)
    {
        slot.LoadCharacter();
        TryEnablePlayButton();
    }

    private void CheckForCrown()
    {
        if (GameSettings.BluePlayer == EPlayerType.Invalid || GameSettings.BluePlayer == EPlayerType.Tutorial || GameSettings.RedPlayer == EPlayerType.Invalid || GameSettings.RedPlayer == EPlayerType.Tutorial)
        {
            return;
        }

        m_BlueCrown.SetActive(GameSettings.Winner == EPlayerColour.Blue);
        m_RedCrown.SetActive(GameSettings.Winner == EPlayerColour.Red);
    }

    private void OnCharacterTypeChanged(EPlayerType characterType, CharacterSlot characterSlot)
    {
        if (characterSlot.PlayerColour == EPlayerColour.Blue)
        {
            GameSettings.BluePlayer = characterType;
        }
        else
        {
            GameSettings.RedPlayer = characterType;
        }

        TryEnablePlayButton();
    }

    private void TryEnablePlayButton()
    {
        if (GameSettings.BluePlayer != EPlayerType.Invalid && GameSettings.BluePlayer != EPlayerType.Tutorial && GameSettings.RedPlayer != EPlayerType.Invalid && GameSettings.RedPlayer != EPlayerType.Tutorial)
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

    private void SetCharacterRings(bool shouldTurnOn)
    {
        if (!TutorialHelper.HasCompletedTutorial)
        {
            Character currentCharacter = m_TutorialManager.CurrentTutorialCharacter;
            currentCharacter.ValidPieceRing.SetActive(shouldTurnOn);
            return;
        }

        for (int i = 0; i < m_CharacterTypes.Count; i++)
        {
            Character currentCharacter = m_CharacterTypes[i];
            currentCharacter.ValidPieceRing.SetActive(shouldTurnOn);
        }
    }

    private void SetSlotRings(bool shouldTurnOn)
    {
        if (shouldTurnOn)
        {
            m_BlueRingParticleSystem.Play();
            m_RedRingParticleSystem.Play();
        }
        else
        {
            m_BlueRingParticleSystem.Stop();
            m_RedRingParticleSystem.Stop();
        }
    }

    public void LoadScene()
    {
        m_TutorialManager.TutorialScreen.SetActive(false);
        SceneController.ChangeScene(EScene.Main);
    }

    public void OpenSettings()
    {
        m_SettingsScreen.SetActive(true);
    }

    private void OnDestroy()
    {
        CharacterSlot.OnCharacterTypeChanged -= OnCharacterTypeChanged;
        m_CharacterTypes.ForEach(character => { character.OnCharacterGrabbed -= OnCharacterGrabbed; character.OnCharacterReleased -= OnCharacterReleased; });
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

