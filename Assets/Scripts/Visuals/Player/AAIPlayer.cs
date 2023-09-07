using System.Collections.Generic;

public abstract class AAIPlayer : APlayer
{
    protected bool TryFindWinningMove(List<KeyValuePair<Piece, EGrid>> validMoves, ref Piece piece, ref EGrid tile)
    {
        BoardTile[] originalBoard = m_GameLogic.m_BoardTiles;

        for (int i = 0; i < validMoves.Count; i++)
        {
            BoardTile[] checkBoard = originalBoard.CloneBoard();

            Piece currentPiece = validMoves[i].Key;
            EPiece pieceType = currentPiece.PieceType;
            EGrid currentTile = validMoves[i].Value;

            int tileIndex = (int)currentTile;
            checkBoard[tileIndex] = new(PlayerColour, pieceType);

            if (BoardTileHelper.CheckWinner(checkBoard) == PlayerColour)
            {
                piece = currentPiece;
                tile = currentTile;
                return true;
            }
        }

        return false;
    }

    // Go thorugh the list of active pieces and check all valid moves
    protected List<KeyValuePair<Piece, EGrid>> CheckPossibleMoves(List<Piece> pieces)
    {
        List<KeyValuePair<Piece, EGrid>> validMoves = new();

        for (int i = 0; i < pieces.Count; i++)
        {
            Piece currentPiece = pieces[i];
            EPiece pieceType = currentPiece.PieceType;
            List<EGrid> validTiles = m_GameLogic.CheckValidTiles(pieceType);

            for (int j = 0; j < validTiles.Count; j++)
            {
                EGrid currentGridID = validTiles[j];
                validMoves.Add(new KeyValuePair<Piece, EGrid>(currentPiece, currentGridID));
            }
        }

        return validMoves;
    }
}
