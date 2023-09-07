using System.Collections.Generic;

public abstract class APlayer
{
    protected GameLogic m_GameLogic { get; private set; }
    protected VisualGameManager m_VisualGameManager { get; private set; }

    public EPlayerColour PlayerColour { get; private set; }

    public void Init(VisualGameManager visualGameManager, GameLogic gameLogic, EPlayerColour playerColour)
    {
        m_VisualGameManager = visualGameManager;
        m_GameLogic = gameLogic;
        PlayerColour = playerColour;
    }

    public abstract void StartTurn(List<Piece> activePieces);

    public abstract void EndTurn();
}
