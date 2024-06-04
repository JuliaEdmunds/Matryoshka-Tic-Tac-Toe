using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAIPlayer : AAIPlayer
{
    // BasicAI
    // 1) Gathers all available moves
    // 2) If there's guaranteed move to win then BasicAI plays it
    // 3) If not then randomly picks and plays one of the available moves

    private DropZone m_CurrentTargetZone;
    private List<KeyValuePair<Piece, EGrid>> m_AllValidMoves;

    public override void StartTurn(List<Piece> activePieces)
    {
        for (int i = 0; i < activePieces.Count; i++)
        {
            Piece currentPiece = activePieces[i];
            currentPiece.DisableDrag();
        }

        m_AllValidMoves = CheckPossibleMoves(activePieces);

        CoroutineManager.Instance.StartCoroutine(PlayPiece());
    }

    private IEnumerator PlayPiece()
    {
        Piece piece = default;
        EGrid gridID = default;

        // Check if there's guaranteed move to win and if yes play it
        // Otherwise, get random piece to move and the grid ID for the valid move
        if (!TryFindWinningMove(m_AllValidMoves, ref piece, ref gridID))
        {
            KeyValuePair<Piece, EGrid> moveToMake = SelectRandomMove();
            piece = moveToMake.Key;
            gridID = moveToMake.Value;
        }

        yield return new WaitForSeconds(2);

        // Light up the board & wait to make move to look more human
        m_VisualGameManager.RequestStartMove(piece);

        yield return new WaitForSeconds(1.5f);

        // Get the dropzone from gridID
        m_CurrentTargetZone = m_VisualGameManager.GetDropzoneFromGridID(gridID);

        // Place the piece on board
        m_VisualGameManager.RequestFinishMove(piece, m_CurrentTargetZone);
    }

    private KeyValuePair<Piece, EGrid> SelectRandomMove()
    {
        int index = UnityEngine.Random.Range(0, m_AllValidMoves.Count);
        return m_AllValidMoves[index];
    }

    public override void EndTurn()
    {
        m_CurrentTargetZone.DropzoneRingHelper.RingOff();
        m_CurrentTargetZone = null;
    }
}
