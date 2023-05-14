using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterSlot : MonoBehaviour
{
    [SerializeField] private EPlayerColour m_PlayerColour;
    public EPlayerColour PlayerColour => m_PlayerColour;

    [SerializeField] private List<CharacterTypeHolder> m_OpponentTypes;

    private GameObject m_CurrentOpponent;

    public event Action<CharacterTypeHolder, CharacterSlot> OnCharacterTypeChanged;

    private void OnTriggerEnter(Collider opponent)
    {
        GameObject draggedOpponent = opponent.gameObject;
        EPlayerType draggedOpponentType = draggedOpponent.GetComponent<CharacterTypeHolder>().CharacterType;

        if (m_CurrentOpponent != null)
        {
            m_CurrentOpponent.SetActive(false);
        }

        // TODO: think how to move merge this chunk of code with EnableCharacterInSlot()
        for (int i = 0; i < m_OpponentTypes.Count; i++)
        {
            CharacterTypeHolder currentOpponent = m_OpponentTypes[i];

            EPlayerType currentOpponentType = currentOpponent.CharacterType;

            if (draggedOpponentType == currentOpponentType)
            {
                m_CurrentOpponent = currentOpponent.gameObject;
                m_CurrentOpponent.SetActive(true);
                OnCharacterTypeChanged(currentOpponent, this);
            }
        }
    }

    public void LoadCharacter()
    {
        EPlayerType currentCharacterType;

        if (this.PlayerColour == EPlayerColour.Blue && GameSettings.BluePlayer != EPlayerType.Invalid)
        {
            currentCharacterType = GameSettings.BluePlayer;

            EnableCharacterInSlot(currentCharacterType);
        }
        else if (this.PlayerColour == EPlayerColour.Red && GameSettings.RedPlayer != EPlayerType.Invalid)
        {
            currentCharacterType = GameSettings.RedPlayer;

            EnableCharacterInSlot(currentCharacterType);
        }
    }

    private void EnableCharacterInSlot(EPlayerType characterType)
    {
        for (int i = 0; i < m_OpponentTypes.Count; i++)
        {
            CharacterTypeHolder currentCharacter = m_OpponentTypes[i];

            EPlayerType currentCharacterType = currentCharacter.CharacterType;

            if (characterType == currentCharacterType)
            {
                GameObject characterToShow = currentCharacter.gameObject;
                characterToShow.SetActive(true);
            }
        }
    }
}
