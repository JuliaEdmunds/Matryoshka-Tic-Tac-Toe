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
}
