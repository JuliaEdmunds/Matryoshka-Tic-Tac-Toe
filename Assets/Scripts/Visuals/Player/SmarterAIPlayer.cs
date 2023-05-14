public class SmarterAIPlayer : AAIPlayer
{
    // TODO:
    // 1) Gather all available moves
    // 2) If there's guaranteed move to win then play it 
    // 3) If possible place piece on the opponents piece otherwise place weakest piece in random position

    // For smarter AI, there is a BoardTiles[] (array) required with the current state of the game
    // + need to move CheckWInner functions from GameLogic to a helper class which takes BoardTiles[]
    // GameLogic still needs to call the functions (the ones in HelperClass) to check if there's a winner
    // helper class serves GameLogic to decide on score and support SmartAI to make the "more sensible" move
    // helper class also needs to have BoardTile[] Clone(BoardTiles[] originalTiles) to determine a move at the turn's start 

    public override void EndTurn()
    {
        throw new System.NotImplementedException();
    }

    public override void StartTurn()
    {
        throw new System.NotImplementedException();
    }
}