using UnityEngine;

public class CharacterTypeHolder : MonoBehaviour
{
    [SerializeField] private EPlayerType m_CharacterType;
    public EPlayerType CharacterType => m_CharacterType;
}
