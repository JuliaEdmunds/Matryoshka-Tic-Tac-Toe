using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAIPlayer : AAIPlayer
{
    // TODO:
    // 1) Gather all available moves
    // 2) Randomly pick and play one

    private List<Piece> m_ActivePieces;

    private List<KeyValuePair<Piece, EGrid>> m_AllValidMoves = new();

    public override void StartTurn(List<Piece> activePieces)
    {
        m_AllValidMoves.Clear();

        m_ActivePieces = activePieces;

        CheckPossibleMoves();

        CoroutineManager.Instance.StartCoroutine(PlayPiece());
    }

    private IEnumerator PlayPiece()
    {
        // Get random piece to move and the grid ID for the valid move
        KeyValuePair<Piece, EGrid>  moveToMake = SelectRandomMove();
        Piece piece = moveToMake.Key;
        EGrid gridID = moveToMake.Value;

        // Light up the board & wait to make move look more human
        m_VisualGameManager.RequestStartMove(piece);

        yield return new WaitForSeconds(1.5f);

        // Get the dropzone from gridID
        Dropzone targetZone = m_VisualGameManager.GetDropzoneFromGridID(gridID);

        // Place the piece on board
        m_VisualGameManager.RequestFinishMove(piece, targetZone);
    }

    private KeyValuePair<Piece, EGrid> SelectRandomMove()
    {
        int index = UnityEngine.Random.Range(0, m_AllValidMoves.Count);
        return m_AllValidMoves[index];
    }

    // Go thorugh the list of active pieces and check all valid moves
    private void CheckPossibleMoves()
    {
        for (int i = 0; i < m_ActivePieces.Count; i++)
        {
            Piece currentPiece = m_ActivePieces[i];
            EPiece pieceType = currentPiece.PieceType;
            List<EGrid> validTiles = m_GameLogic.CheckValidTiles(pieceType);

            for (int j = 0; j < validTiles.Count; j++)
            {
                EGrid currentGridID = validTiles[j];
                m_AllValidMoves.Add(new KeyValuePair<Piece, EGrid>(currentPiece, currentGridID));
            }
        }
    }

    public override void EndTurn()
    {
        Debug.Log("AI finished move");
    }
}