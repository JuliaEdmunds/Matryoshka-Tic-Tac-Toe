using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterSlot : MonoBehaviour
{
    [SerializeField] private EPlayer m_PlayerColour;
    public EPlayer PlayerColour => m_PlayerColour;

    [SerializeField] private List<CharacterTypeHolder> m_OpponentTypes;

    private GameObject m_CurrentOpponent;

    public event Action<CharacterTypeHolder, CharacterSlot> OnCharacterTypeChanged;

    private void OnTriggerEnter(Collider opponent)
    {
        GameObject draggedOpponent = opponent.gameObject;
        ECharacterType draggedOpponentType = draggedOpponent.GetComponent<CharacterTypeHolder>().CharacterType;

        if (m_CurrentOpponent != null)
        {
            m_CurrentOpponent.SetActive(false);
        }

        // TODO: think how to move merge this chunk of code with EnableCharacterInSlot()
        for (int i = 0; i < m_OpponentTypes.Count; i++)
        {
            CharacterTypeHolder currentOpponent = m_OpponentTypes[i];

            ECharacterType currentOpponentType = currentOpponent.CharacterType;

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
        ECharacterType currentCharacterType;

        if (this.PlayerColour == EPlayer.Blue && GameSettings.BluePlayer != ECharacterType.None)
        {
            currentCharacterType = GameSettings.BluePlayer;

            EnableCharacterInSlot(currentCharacterType);
        }
        else if (this.PlayerColour == EPlayer.Red && GameSettings.RedPlayer != ECharacterType.None)
        {
            currentCharacterType = GameSettings.RedPlayer;

            EnableCharacterInSlot(currentCharacterType);
        }
    }

    private void EnableCharacterInSlot(ECharacterType characterType)
    {
        for (int i = 0; i < m_OpponentTypes.Count; i++)
        {
            CharacterTypeHolder currentCharacter = m_OpponentTypes[i];

            ECharacterType currentCharacterType = currentCharacter.CharacterType;

            if (characterType == currentCharacterType)
            {
                GameObject characterToShow = currentCharacter.gameObject;
                characterToShow.SetActive(true);
            }
        }
    }
}
