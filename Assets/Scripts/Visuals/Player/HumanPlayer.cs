using System.Collections.Generic;

public class HumanPlayer : APlayer
{
    // TODO:
    // 1) Move dragging and dropping of pieces from VM here

    private List<Piece> m_ActivePieces;

    public override void StartTurn(List<Piece> activePieces)
    {
        m_ActivePieces = activePieces;
        m_ActivePieces.ForEach(piece => { piece.OnGridOccupied += OnGridOccupied; piece.OnPieceGrabbed += OnPieceGrabbed; piece.OnPieceReleased += OnPieceReleased; });
    }

    private void OnGridOccupied(Piece piece, Dropzone targetZone)
    {
        m_VisualGameManager.RequestFinishMove(piece, targetZone);
    }

    private void OnPieceGrabbed(Piece piece)
    {
        m_VisualGameManager.RequestStartMove(piece);
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