﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BasicAIPlayer : AAIPlayer
{
    // TODO:
    // 1) Gather all available moves
    // 2) Randomly pick and play one
    // 3) If there's guaranteed move to win then play it

    private List<Piece> m_ActivePieces;

    private List<KeyValuePair<Piece, EGrid>> m_AllValidMoves;

    public override void StartTurn(List<Piece> activePieces)
    {
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
        Dropzone targetZone = m_VisualGameManager.GetDropzoneFromGridID(gridID);

        // Place the piece on board
        m_VisualGameManager.RequestFinishMove(piece, targetZone);
    }

    private KeyValuePair<Piece, EGrid> SelectRandomMove()
    {
        int index = UnityEngine.Random.Range(0, m_AllValidMoves.Count);
        return m_AllValidMoves[index];
    }

    public override void EndTurn()
    {
        Debug.Log("AI finished move");
    }
}