using UnityEngine;


public class Crasher : MonoBehaviour
{
    [SerializeField] private Piece m_CrusherPiece;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Piece>(out Piece crushedPiece))
        {
            GameplayAudioManager.Instance.PlayCharacterCrashed();
            m_CrusherPiece.CrashOpponent();
            crushedPiece.BeCrashed();
        }
    }
}
