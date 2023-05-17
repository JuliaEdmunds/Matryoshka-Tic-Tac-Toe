using System;

public static class PlayerFactory
{
    public static APlayer CreatePlayer(EPlayerType characterType)
    {
        switch (characterType)
        {
            case EPlayerType.Human:
                return new HumanPlayer();

            case EPlayerType.BasicAI: 
                return new BasicAIPlayer();

            case EPlayerType.SmartAI:
                return new SmarterAIPlayer();

            case EPlayerType.Tutorial:
                return new ScriptedTutorialPlayer();

            default:
            case EPlayerType.Invalid:
                throw new NotSupportedException($"Unsupported: EPlayerType.{characterType}");
        }
    }
}
