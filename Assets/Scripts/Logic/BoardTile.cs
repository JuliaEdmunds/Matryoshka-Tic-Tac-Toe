using System;

[Serializable]
public class BoardTile
{
    public EPlayerColour Player;
    public EPiece Piece;

    public BoardTile(EPlayerColour player, EPiece piece)
    {
        Player = player;
        Piece = piece;
    }
}