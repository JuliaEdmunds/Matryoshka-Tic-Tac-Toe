﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class GameSettings
{
    //TODO: Switch players to Invalid after testing
    public static EPlayerType BluePlayer = EPlayerType.Human;

    public static EPlayerType RedPlayer = EPlayerType.BasicAI;

    public static EPlayerColour Winner = EPlayerColour.Invalid;
}
