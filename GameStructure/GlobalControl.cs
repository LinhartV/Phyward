using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Class for controlling entire Phyward
/// </summary>
public static class GlobalControl
{
    private static Dictionary<string, GameControl> games = new Dictionary<string, GameControl>();
    /// <summary>
    /// Game of the selected account
    /// </summary>
    public static GameControl game;
    /// <summary>
    /// Adds new player account
    /// </summary>
    /// <param name="playerName">Unique name of the player</param>
    /// <returns>Whether the new account was created successfully</returns>
    private static bool AddGame(string playerName)
    {
        if (!games.ContainsKey(playerName))
        {
            games.Add(playerName, new GameControl(playerName));
            game = games[playerName];
            ToolsGame.CreateGame();
            
            //ToolsSystem.SaveGame(games[playerName]);

            return true;
        }
        else
            return false;
    }
    /// <summary>
    /// Loads game from save file or creates new when file is not found.
    /// </summary>
    /// <param name="playerName">Doesn't really matter... (idea was adding possibility to create more accounts... I realized it would be useless)</param>
    public static void StartGame(string playerName)
    {
        if (!games.ContainsKey(playerName))
        {
            if (!ToolsSystem.LoadGame(playerName, out game))
            {
                AddGame(playerName);
            }
        }
        else
        {
            game = games[playerName];
        }

    }
}
