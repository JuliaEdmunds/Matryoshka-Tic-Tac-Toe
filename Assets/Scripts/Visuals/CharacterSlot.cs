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

    [SerializeField] private List<CharacterTypeHolder> m_CharacterTypes;

    private GameObject m_CurrentOpponent;

    public static event Action<EPlayerType, CharacterSlot> OnCharacterTypeChanged;

    private void OnTriggerEnter(Collider opponent)
    {
        // Get type of character from dragged opponent
        GameObject draggedOpponent = opponent.gameObject;

        Character draggedCharacter = draggedOpponent.GetComponent<Character>();

        if (draggedCharacter == null)
        {
            return;
        }

        EPlayerType draggedOpponentType = draggedCharacter.CharacterType;

        if (m_CurrentOpponent != null)
        {
            m_CurrentOpponent.SetActive(false);
        }

        // TODO: think how to move merge this chunk of code with EnableCharacterInSlot()
        for (int i = 0; i < m_CharacterTypes.Count; i++)
        {
            CharacterTypeHolder currentOpponent = m_CharacterTypes[i];
            EPlayerType currentOpponentType = currentOpponent.CharacterType;

            if (draggedOpponentType == currentOpponentType)
            {
                m_CurrentOpponent = currentOpponent.gameObject;
                m_CurrentOpponent.SetActive(true);
                OnCharacterTypeChanged(currentOpponentType, this);
            }
        }
    }

    public void LoadCharacter()
    {
        EPlayerType currentCharacterType;

        if (this.PlayerColour == EPlayerColour.Blue && GameSettings.BluePlayer != EPlayerType.Invalid && GameSettings.BluePlayer != EPlayerType.Tutorial)
        {
            currentCharacterType = GameSettings.BluePlayer;

            EnableCharacterInSlot(currentCharacterType);
        }
        else if (this.PlayerColour == EPlayerColour.Red && GameSettings.RedPlayer != EPlayerType.Invalid && GameSettings.RedPlayer != EPlayerType.Tutorial)
        {
            currentCharacterType = GameSettings.RedPlayer;

            EnableCharacterInSlot(currentCharacterType);
        }
    }

    private void EnableCharacterInSlot(EPlayerType characterType)
    {
        for (int i = 0; i < m_CharacterTypes.Count; i++)
        {
            CharacterTypeHolder currentCharacter = m_CharacterTypes[i];

            EPlayerType currentCharacterType = currentCharacter.CharacterType;

            if (characterType == currentCharacterType)
            {
                m_CurrentOpponent = currentCharacter.gameObject;
                GameObject characterToShow = currentCharacter.gameObject;
                characterToShow.SetActive(true);
            }
        }
    }
}
