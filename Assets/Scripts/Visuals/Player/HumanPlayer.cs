using System.Collections.Generic;

public class HumanPlayer : APlayer
{
    private List<Piece> m_ActivePieces;

    public override void StartTurn(List<Piece> activePieces)
    {
        m_ActivePieces = activePieces;
        m_ActivePieces.ForEach(piece => { piece.OnGridOccupied += OnGridOccupied; piece.OnPieceGrabbed += OnPieceGrabbed; piece.OnPieceReleased += OnPieceReleased; });
    }

    private void OnPieceGrabbed(Piece piece)
    {
        m_VisualGameManager.RequestStartMove(piece);
    }

    private void OnGridOccupied(Piece piece, Dropzone targetZone)
    {
        m_VisualGameManager.RequestFinishMove(piece, targetZone);
    }

    private void OnPieceReleased()
    {
        m_VisualGameManager.RequestCancelMove();
    }


    public override void EndTurn()
    {
        m_ActivePieces.ForEach(piece => { piece.OnGridOccupied -= OnGridOccupied; piece.OnPieceGrabbed -= OnPieceGrabbed; piece.OnPieceReleased -= OnPieceReleased; });
    }
}
