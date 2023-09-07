using System;
using System.Collections.Generic;

public static class BoardTileHelper
{
    private static readonly Random rng = new();

    public static BoardTile[] CloneBoard(this BoardTile[] originalBoard)
    {
        BoardTile[] clonedBoard = new BoardTile[originalBoard.Length];

        for (int i = 0; i < originalBoard.Length; i++)
        {
            BoardTile currentTile = originalBoard[i];
            clonedBoard[i] = currentTile;
        }

        return clonedBoard;
    }

    // Check if there's a winner in any of the rows/columns/diagonals
    public static EPlayerColour CheckWinner(BoardTile[] board)
    {
        bool isWinner;
        EPlayerColour winner;

        // Check all the rows for winner
        isWinner = CheckRowForWinner(0, board, out winner);
        isWinner = isWinner || CheckRowForWinner(1, board, out winner);
        isWinner = isWinner || CheckRowForWinner(2, board, out winner);

        // Check all the columns for winner
        isWinner = isWinner || CheckColumnForWinner(0, board, out winner);
        isWinner = isWinner || CheckColumnForWinner(1, board, out winner);
        isWinner = isWinner || CheckColumnForWinner(2, board, out winner);
        
        // Check both diagonals for winner
        isWinner = isWinner || CheckDiagonalForWinner(true, board, out winner);
        isWinner = isWinner || CheckDiagonalForWinner(false, board, out winner);

        return winner;
    }

    private static bool CheckRowForWinner(int rowNum, BoardTile[] board, out EPlayerColour winner)
    {
        int startingTile = 3 * rowNum;
        return CheckTilesForWinner(startingTile, startingTile + 1, startingTile + 2, board, out winner);
    }

    private static bool CheckColumnForWinner(int columnNum, BoardTile[] board, out EPlayerColour winner)
    {
        return CheckTilesForWinner(columnNum, columnNum + 3, columnNum + 6, board, out winner);
    }

    private static bool CheckDiagonalForWinner(bool isLeftDiagonal, BoardTile[] board, out EPlayerColour winner)
    {
        if (isLeftDiagonal)
        {
            return CheckTilesForWinner(0, 4, 8, board, out winner);
        }
        else
        {
            return CheckTilesForWinner(2, 4, 6, board, out winner);
        }
    }

    private static bool CheckTilesForWinner(int tile1, int tile2, int tile3, BoardTile[] board, out EPlayerColour winner)
    {
        EPlayerColour tileContent = board[tile1].Player;

        // Check if and if yes who is the winner
        if (!board[tile1].IsEmpty() && board[tile2].Player == tileContent && board[tile3].Player == tileContent)
        {
            winner = tileContent;
            return true;
        }

        winner = EPlayerColour.Invalid;
        return false;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

