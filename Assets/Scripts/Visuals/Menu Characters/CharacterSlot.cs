using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSlot : MonoBehaviour
{
    [SerializeField] private EPlayerColour m_PlayerColour;
    public EPlayerColour PlayerColour => m_PlayerColour;

    [SerializeField] private List<CharacterTypeHolder> m_CharacterTypeFigures;

    [SerializeField] private List<Character> m_DragableCharacterTypes;
    public List<Character> DragableCharacterTypes => m_DragableCharacterTypes;

    [SerializeField] private TutorialManager m_TutorialManager;

    [SerializeField] private ParticleSystem m_ParticleSystem;

    private GameObject m_CurrentOpponent;

    public static event Action<EPlayerType, CharacterSlot> OnCharacterTypeChanged;

    private Character m_CurrentlyDraggedCharacter;

    public CharacterSlot m_CurrentSlot;

    private void Start()
    {
        m_DragableCharacterTypes.ForEach(character => { character.OnCharacterGrabbed += OnCharacterGrabbed; character.OnCharacterReleased += OnCharacterReleased; });
    }

    private void SetDraggedCharacter(Character character)
    {
        m_CurrentlyDraggedCharacter = character;
    }

    private void OnCharacterGrabbed(Character character)
    {
        SetDraggedCharacter(character);
        SetCharacterRings(false);
        SetSlotRings(true);
    }

    private void OnCharacterReleased()
    {
        ChangeCharacter();
        SetCharacterRings(true);
        SetSlotRings(false);
        m_CurrentlyDraggedCharacter = null;
    }

    private void OnTriggerEnter(Collider opponent)
    {
        m_CurrentSlot = this;
    }

    private void OnTriggerExit(Collider other)
    {
        m_CurrentSlot = null;
    }

    private void ChangeCharacter()
    {
        if (m_CurrentSlot == null || m_CurrentlyDraggedCharacter == null)
        {
            return;
        }

        EPlayerType draggedOpponentType = m_CurrentlyDraggedCharacter.CharacterType;

        if (m_CurrentOpponent != null)
        {
            m_CurrentOpponent.SetActive(false);
        }

        LoadCharacterInSlot(draggedOpponentType);
        OnCharacterTypeChanged(draggedOpponentType, this);
    }

    private void SetCharacterRings(bool shouldTurnOn)
    {
        if (!TutorialHelper.HasCompletedTutorial)
        {
            Character currentCharacter = m_TutorialManager.CurrentTutorialCharacter;
            currentCharacter.ValidPieceRing.SetActive(shouldTurnOn);
            return;
        }

        for (int i = 0; i < m_DragableCharacterTypes.Count; i++)
        {
            Character currentCharacter = m_DragableCharacterTypes[i];
            currentCharacter.ValidPieceRing.SetActive(shouldTurnOn);
        }
    }

    private void SetSlotRings(bool shouldTurnOn)
    {
        if (shouldTurnOn)
        {
            m_ParticleSystem.Play();
        }
        else
        {
            m_ParticleSystem.Stop();
        }
    }

    public void LoadCharacter()
    {
        EPlayerType currentCharacterType;

        if (this.PlayerColour == EPlayerColour.Blue && GameSettings.BluePlayer != EPlayerType.Invalid && GameSettings.BluePlayer != EPlayerType.Tutorial)
        {
            currentCharacterType = GameSettings.BluePlayer;

            LoadCharacterInSlot(currentCharacterType);
        }
        else if (this.PlayerColour == EPlayerColour.Red && GameSettings.RedPlayer != EPlayerType.Invalid && GameSettings.RedPlayer != EPlayerType.Tutorial)
        {
            currentCharacterType = GameSettings.RedPlayer;

            LoadCharacterInSlot(currentCharacterType);
        }
    }

    private void LoadCharacterInSlot(EPlayerType characterType)
    {
        for (int i = 0; i < m_CharacterTypeFigures.Count; i++)
        {
            CharacterTypeHolder currentCharacter = m_CharacterTypeFigures[i];
            EPlayerType currentCharacterType = currentCharacter.CharacterType;

            if (characterType == currentCharacterType)
            {
                m_CurrentOpponent = currentCharacter.gameObject;
                m_CurrentOpponent.SetActive(true);
            }
        }
        m_CurrentSlot = null;
    }

    private void OnDestroy()
    {
        m_DragableCharacterTypes.ForEach(character => { character.OnCharacterGrabbed -= OnCharacterGrabbed; character.OnCharacterReleased -= OnCharacterReleased; });
    }
}
