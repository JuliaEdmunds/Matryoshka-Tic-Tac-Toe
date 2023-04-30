using System;

[Serializable]
public class BoardTile
{
    public EPlayer Player;
    public EPiece Piece;

    public BoardTile(EPlayer player, EPiece piece)
    {
        Player = player;
        Piece = piece;
    }
}