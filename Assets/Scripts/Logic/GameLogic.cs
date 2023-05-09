using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class GameLogic
{
    public event Action<EPlayer, List<EPiece>> OnTurnStarted;
    public event Action<EPlayer> OnGameEnded;

    public EPlayer CurrentPlayer { get; private set; }

    private List<EPiece> m_PlayerBlueValidPieces = new();
    private List<EPiece> m_PlayerRedValidPPieces = new();

    private BoardTile[] m_BoardTiles;

    private EGameState m_GameState;

    private EPlayer m_Winner;

    public void StartGame()
    {
        m_PlayerBlueValidPieces.Clear();
        m_PlayerRedValidPPieces.Clear();
        m_PlayerBlueValidPieces.AddRange(Enum.GetValues(typeof(EPiece)));
        m_PlayerRedValidPPieces.AddRange(Enum.GetValues(typeof(EPiece)));

        m_BoardTiles = new BoardTile[Enum.GetNames(typeof(EGrid)).Length];

        m_GameState = EGameState.Playing;

        CurrentPlayer = EPlayer.Blue;

        StartTurn();
    }

    private void StartTurn()
    {
        // Raises an event
        OnTurnStarted(CurrentPlayer, CurrentPlayerValidPieces);
    }

    public List<EPiece> CurrentPlayerValidPieces => CurrentPlayer == EPlayer.Blue ? m_PlayerBlueValidPieces : m_PlayerRedValidPPieces;

    // Call from VM
    public List<EGrid> CheckValidTiles(EPiece piece)
    {
        List<EGrid> validTiles = new();
        validTiles.AddRange(Enum.GetValues(typeof(EGrid)));

        for (int i = 0; i < m_BoardTiles.Length; i++)
        {
            BoardTile currentTile = m_BoardTiles[i];
            if (currentTile == null)
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
    public void SetPieceOnBoard(EPiece pieceType, EGrid zone)
    {
        int tileIndex = (int)zone;
        m_BoardTiles[tileIndex] = new(CurrentPlayer, pieceType);

        CurrentPlayerValidPieces.Remove(pieceType);

        EndTurn();
    }


    private void EndTurn()
    {
        CheckWinner();

        if (m_GameState != EGameState.Over)
        {
            CurrentPlayer = CurrentPlayer == EPlayer.Red ? EPlayer.Blue : EPlayer.Red;
            StartTurn();
        }

        if (m_GameState == EGameState.Over)
        {
            EndGame();
        }
    }

    // Check if there's a winner in any of the rows/columns/diagonals
    private void CheckWinner()
    {
        CheckRowForWinner(0);
        CheckRowForWinner(1);
        CheckRowForWinner(2);

        CheckColumnForWinner(0);
        CheckColumnForWinner(1);
        CheckColumnForWinner(2);

        CheckDiagonalForWinner(true);
        CheckDiagonalForWinner(false);
    }

    private void CheckRowForWinner(int rowNum)
    {
        int startingTile = 3 * rowNum;
        CheckTilesForWinner(startingTile, startingTile + 1, startingTile + 2);
    }

    private void CheckColumnForWinner(int columnNum)
    {
        CheckTilesForWinner(columnNum, columnNum + 3, columnNum + 6);
    }

    private void CheckDiagonalForWinner(bool isLeftDiagonal)
    {
        if (isLeftDiagonal)
        {
            CheckTilesForWinner(0, 4, 8);
        }
        else
        {
            CheckTilesForWinner(2, 4, 6);
        }
    }

    private void CheckTilesForWinner(int tile1, int tile2, int tile3)
    {
        if (m_BoardTiles[tile1] == null || m_BoardTiles[tile2] == null || m_BoardTiles[tile3] == null)
        {
            return;
        }

        EPlayer? tileContent = m_BoardTiles[tile1].Player;

        // Check if and if yes who is the winner
        if (m_BoardTiles[tile2].Player == tileContent && m_BoardTiles[tile3].Player == tileContent)
        {
            m_Winner = tileContent == EPlayer.Blue ? EPlayer.Blue : EPlayer.Red;
            m_GameState = EGameState.Over;
        }
    }

    private void EndGame()
    {
        GameSettings.Winner = m_Winner;
        OnGameEnded(m_Winner);
    }
}