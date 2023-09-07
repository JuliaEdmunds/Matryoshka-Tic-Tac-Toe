using System;

[Serializable]
public struct BoardTile
{
    public EPlayerColour Player;
    public EPiece Piece;

    public BoardTile(EPlayerColour player, EPiece piece)
    {
        Player = player;
        Piece = piece;
    }

    public bool IsEmpty() => Player == EPlayerColour.Invalid;
}
