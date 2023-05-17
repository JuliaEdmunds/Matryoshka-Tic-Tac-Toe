using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ScriptedTutorialPlayer : APlayer
{
    private int m_RoundCounter = 0;
    private List<Piece> m_CurrentlyActivePieces;
    private Coroutine m_Coroutine;

    private List<EPiece> m_CurrentlyActivePiecesTypes = new();
    private List<EGrid> m_CurrentValidTiles = new();

    public override void StartTurn(List<Piece> activePieces)
    {
        m_CurrentlyActivePieces = activePieces;

        m_VisualGameManager.DisablePieces(activePieces);

        m_Coroutine = CoroutineManager.Instance.StartCoroutine(ExecuteTurn());
    }

    private IEnumerator ExecuteTurn()
    {
        yield return m_VisualGameManager.ShowNextTutorialText();

        if (PlayerColour == EPlayerColour.Blue)
        {
            m_CurrentlyActivePieces.ForEach(piece => { piece.OnGridOccupied += OnGridOccupied; piece.OnPieceGrabbed += OnPieceGrabbed; piece.OnPieceReleased += OnPieceReleased; });
            BlueMoves();
        }
        else
        {
            RedMoves();
        }
    }

    private void BlueMoves()
    {
        m_CurrentlyActivePiecesTypes.Clear();
        m_CurrentValidTiles.Clear();

        if (m_RoundCounter == 0)
        {
            m_CurrentlyActivePiecesTypes.Add(EPiece.Biggish);
            m_CurrentValidTiles.Add(EGrid.CentreLeft);

            m_VisualGameManager.SetActivePiecesForCurrentPlayer(m_CurrentlyActivePiecesTypes);
        }
        else if (m_RoundCounter == 1)
        {
            m_CurrentlyActivePiecesTypes.Add(EPiece.Smallish);
            m_CurrentValidTiles.Add(EGrid.CentreMiddle);

            m_VisualGameManager.SetActivePiecesForCurrentPlayer(m_CurrentlyActivePiecesTypes);
        }
        else if (m_RoundCounter == 2)
        {
            m_CurrentlyActivePiecesTypes.Add(EPiece.Biggest);
            m_CurrentValidTiles.Add(EGrid.CentreRight);

            m_VisualGameManager.SetActivePiecesForCurrentPlayer(m_CurrentlyActivePiecesTypes);
        }
        else
        {
            return;
        }
    }

    private void RedMoves()
    {
        EPiece pieceToPlay = default;
        EGrid tileToPlay = default;

        if (m_RoundCounter == 0)
        {
            // Play selected piece coroutine
            pieceToPlay = EPiece.Smallest;
            tileToPlay = EGrid.CentreMiddle;   
        }
        else if (m_RoundCounter == 1)
        {
            pieceToPlay = EPiece.Small;
            tileToPlay = EGrid.CentreRight;
        }
        else
        {
            return;
        }

        CoroutineManager.Instance.StartCoroutine(MakeAIMove(pieceToPlay, tileToPlay));
    }

    private void OnPieceGrabbed(Piece piece)
    {
        m_VisualGameManager.RequestStartMove(piece);
        m_VisualGameManager.DisableTiles(m_CurrentValidTiles);
    }

    private void OnPieceReleased()
    {
        m_VisualGameManager.SetActivePiecesForCurrentPlayer(m_CurrentlyActivePiecesTypes);
        m_VisualGameManager.DisableTiles(m_CurrentValidTiles);
    }

    private void OnGridOccupied(Piece piece, Dropzone targetZone)
    {
        m_VisualGameManager.RequestFinishMove(piece, targetZone);
    }

    private IEnumerator MakeAIMove(EPiece pieceType, EGrid gridID)
    {
        m_CurrentlyActivePiecesTypes.Clear();
        m_CurrentlyActivePiecesTypes.Add(pieceType);
        m_VisualGameManager.SetActivePiecesForCurrentPlayer(m_CurrentlyActivePiecesTypes);

        yield return new WaitForSeconds(1.5f);
        // Light up the board & wait to make move to look more human
        Piece piece = m_VisualGameManager.GetPieceFromPieceType(pieceType);
        m_VisualGameManager.RequestStartMove(piece);

        // Get the dropzone from gridID
        Dropzone targetZone = m_VisualGameManager.GetDropzoneFromGridID(gridID);

        // Place the piece on board
        m_VisualGameManager.RequestFinishMove(piece, targetZone);
    }

    public override void EndTurn()
    {
        if (PlayerColour == EPlayerColour.Blue)
        {
            m_CurrentlyActivePieces.ForEach(piece => { piece.OnGridOccupied -= OnGridOccupied; piece.OnPieceGrabbed -= OnPieceGrabbed; piece.OnPieceReleased -= OnPieceReleased; });
        }

        if (m_Coroutine != null)
        {
            CoroutineManager.Instance.StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }

        m_RoundCounter++;
        m_VisualGameManager.TutorialScreen.SetActive(false);
    }
}



