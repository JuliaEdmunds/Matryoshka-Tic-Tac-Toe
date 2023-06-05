using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class GameLogic
{
    public event Action<EPlayerColour, List<EPiece>> OnTurnStarted;
    public event Action<EPlayerColour> OnTurnEnded;
    public event Action<EPlayerColour> OnGameEnded;

    public EPlayerColour CurrentPlayer { get; private set; }

    private List<EPiece> m_PlayerBlueValidPieces = new();
    private List<EPiece> m_PlayerRedValidPPieces = new();

    public BoardTile[] m_BoardTiles { get; private set; }

    private EGameState m_GameState;

    public void StartGame()
    {
        m_PlayerBlueValidPieces.Clear();
        m_PlayerRedValidPPieces.Clear();
        m_PlayerBlueValidPieces.AddRange(Enum.GetValues(typeof(EPiece)));
        m_PlayerRedValidPPieces.AddRange(Enum.GetValues(typeof(EPiece)));

        m_BoardTiles = new BoardTile[Enum.GetNames(typeof(EGrid)).Length];

        m_GameState = EGameState.Playing;

        CurrentPlayer = EPlayerColour.Blue;

        StartTurn();
    }

    private void StartTurn()
    {
        // Check if there are valid moves or if it's a draw
        CheckIfDraw();

        if (m_GameState == EGameState.Playing)
        {
            // Raises an event
            OnTurnStarted(CurrentPlayer, CurrentPlayerValidPieces);
        }
    }

    public List<EPiece> CurrentPlayerValidPieces => CurrentPlayer == EPlayerColour.Blue ? m_PlayerBlueValidPieces : m_PlayerRedValidPPieces;

    // Call from VM
    public List<EGrid> CheckValidTiles(EPiece piece)
    {
        List<EGrid> validTiles = new();
        validTiles.AddRange(Enum.GetValues(typeof(EGrid)));

        for (int i = 0; i < m_BoardTiles.Length; i++)
        {
            BoardTile currentTile = m_BoardTiles[i];
            if (currentTile.IsEmpty())
            {
                continue;
            }

            EPiece pieceOnBoard = currentTile.Piece;
            if (pieceOnBoard >= piece)
            {
                validTiles.Remove((EGrid)i);
            }
        }

        return validTiles;
    }

    // Called from Visual Manager
    public void SetPieceOnBoard(EPiece pieceType, EGrid tile)
    {
        int tileIndex = (int)tile;
        m_BoardTiles[tileIndex] = new(CurrentPlayer, pieceType);

        CurrentPlayerValidPieces.Remove(pieceType);

        EndTurn();
    }

    public bool IsTileEmpty(EGrid tile)
    {
        int tileIndex = (int)tile;
        BoardTile boardtile = m_BoardTiles[tileIndex];
        return boardtile.IsEmpty();
    }

    private void EndTurn()
    {
        OnTurnEnded?.Invoke(CurrentPlayer);

        // Determine if there is a winner and if yes end game
        GameSettings.Winner = BoardTileHelper.CheckWinner(m_BoardTiles);
        m_GameState = GameSettings.Winner != EPlayerColour.Invalid ? EGameState.Over : EGameState.Playing;

        if (m_GameState != EGameState.Over)
        {
            CurrentPlayer = CurrentPlayer == EPlayerColour.Red ? EPlayerColour.Blue : EPlayerColour.Red;
            StartTurn();
        }

        if (m_GameState == EGameState.Over)
        {
            EndGame();
        }
    }

    // Check for draw (either no more pieces for both players or neither has valid moves)
    private int m_NumSkipTurns = 0;
    private void CheckIfDraw()
    {
        // If there is already a winner early out
        if (GameSettings.Winner != EPlayerColour.Invalid)
        {
            return;
        }

        // If neither player has pieces left it's a draw
        if (!m_PlayerBlueValidPieces.Any() && !m_PlayerRedValidPPieces.Any())
        {
            GameSettings.Winner = EPlayerColour.Invalid;
            m_GameState = EGameState.Over;
            EndGame();
            return;
        }

        // Verify that the current player has at least 1 valid move (if so it's not a draw)
        bool hasValidMoves;
        for (int i = 0; i < CurrentPlayerValidPieces.Count; i++)
        {
            EPiece currentPiece = CurrentPlayerValidPieces[i];
            List<EGrid> currentPieceValidTiles = CheckValidTiles(currentPiece);
            hasValidMoves = currentPieceValidTiles.Any();

            if (hasValidMoves)
            {
                m_NumSkipTurns = 0;
                return;
            }
        }

        m_NumSkipTurns++;

        // If turns skipped consecutively by both player trigger a draw
        if (m_NumSkipTurns >= 2)
        {
            GameSettings.Winner = EPlayerColour.Invalid;
            m_GameState = EGameState.Over;
            EndGame();
            return;
        }

        // If no valid moves for current player skip their turn
        EndTurn();
    }

    private void EndGame()
    {
        OnGameEnded(GameSettings.Winner);
    }
}