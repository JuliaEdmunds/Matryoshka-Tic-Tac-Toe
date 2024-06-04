using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmarterAIPlayer : AAIPlayer
{
    // Characteristics:
    // 1) Gather all available moves
    // 2) If there's guaranteed move to win then play it 
    // 3) If possible place piece on the opponents piece otherwise place weakest piece in random position

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
        // If no guaranteed win and if possible place piece on the opponent's piece
        // Otherwise place weakest piece in random position
        if (!TryFindWinningMove(m_AllValidMoves, ref piece, ref gridID))
        {
            if (!TryCrushOpponent(ref piece, ref gridID))
            {
                KeyValuePair<Piece, EGrid> moveToMake = SelectRandomMove();
                piece = moveToMake.Key;
                gridID = moveToMake.Value;
            }   
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

    private bool TryCrushOpponent(ref Piece piece, ref EGrid tile)
    {
        BoardTile[] originalBoard = m_GameLogic.m_BoardTiles;

        m_AllValidMoves.Shuffle();

        for (int i = 0; i < m_AllValidMoves.Count; i++)
        {
            BoardTile[] checkBoard = originalBoard.CloneBoard();

            Piece currentPiece = m_AllValidMoves[i].Key;
            EGrid currentTile = m_AllValidMoves[i].Value;

            int tileIndex = (int)currentTile;
            BoardTile currentBoardtile = checkBoard[tileIndex];
            
            // From valid move try to find a tile with opponent's piece on it
            if (!currentBoardtile.IsEmpty() && currentBoardtile.Player != PlayerColour)
            {
                piece = currentPiece;
                tile = currentTile;
                return true;
            }
        }

        return false;
    }

    private KeyValuePair<Piece, EGrid> SelectRandomMove()
    {
        int index = Random.Range(0, m_AllValidMoves.Count);
        return m_AllValidMoves[index];
    }

    public override void EndTurn()
    {
        m_CurrentTargetZone.DropzoneRingHelper.RingOff();
        m_CurrentTargetZone = null;
    }
}
