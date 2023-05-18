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
    [SerializeField] private List<Character> m_CharacterTypes;
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
            currentCharSlot.OnCharacterTypeChanged += OnCharacterTypeChanged;
        }

        m_CharacterTypes.ForEach(character => { character.OnCharacterGrabbed += OnCharacterGrabbed; character.OnCharacterReleased += OnCharacterReleased; });

        CheckForCrown();
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
        SceneController.ChangeScene(EScene.Main);
    }

    private void OnDestroy()
    {
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

