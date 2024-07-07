//#define LOAD

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using JsonNet.ContractResolvers;
using Newtonsoft.Json.Linq;

public static class ToolsSystem
{
    /// <summary>
    /// When the key is active
    /// </summary>
    /// 
    public enum PauseType {
        ///<summary>This keyaction can be triggered when inventory is open</summary>
        Inventory,
        ///<summary>This keyaction can be triggered during gameplay</summary>
        InGame,
        ///<summary>This keyaction can be triggered while animation is being played</summary>
        Animation,
        ///<summary>This keyaction can be triggered in main menu</summary>
        Menu,
        ///<summary>This keyaction can be triggered in main menu</summary>
        All

    };

    public static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.All,
        Formatting = Newtonsoft.Json.Formatting.Indented,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        ContractResolver = new PrivateSetterContractResolver()
    };


    /// <summary>
    /// Loads game from save file or creates new when file is not found.
    /// </summary>
    /// <param name="playerName">Doesn't really matter... (idea was adding possibility to create more accounts... I realized it would be useless)</param>
    public static void StartGame(string playerName)
    {
        if (!GCon.games.ContainsKey(playerName))
        {
            if (!LoadGame(playerName, out GCon.game))
            {
                AddGame(playerName);
            }
        }
        else
        {
            GCon.game = GCon.games[playerName];
        }
    }
    /// <summary>
    /// Adds new player account
    /// </summary>
    /// <param name="playerName">Unique name of the player</param>
    /// <returns>Whether the new account was created successfully</returns>
    private static bool AddGame(string playerName)
    {
        if (!GCon.games.ContainsKey(playerName))
        {
            GCon.games.Add(playerName, new GameControl(playerName));
            GCon.game = GCon.games[playerName];
            //TODO mazat při nahrání jiné hry
            GCon.game.ActivateThisGame();
            ToolsGame.CreateGame();
            return true;
        }
        else
            return false;
    }
    public static void ReleaseAllKeys()
    {
        foreach (var key in KeyController.registeredKeys.Values)
        {
            if (key.Peek().Pressed && key.Peek().pauseTypeKeys.Contains(ToolsSystem.PauseType.InGame))
            {
                key.Peek().KeyUp();
            }
        }
    }
    public static void SaveGame(GameControl game)
    {
        if (!Directory.Exists("./SavedGames"))
        {
            Directory.CreateDirectory("./SavedGames");
        }
        string destination = "./SavedGames/" + game.PlayerName + ".json";

        //before saving simulate key release of all keys
        ReleaseAllKeys();
        foreach (var item in game.Items.Values)
        {
            item.SaveItem();
        }

        string jsonString = JsonConvert.SerializeObject(game, jsonSerializerSettings);
        File.WriteAllText(destination, jsonString);
    }
    /// <summary>
    /// Loads saved game
    /// </summary>
    /// <param name="playerName">Name of player's account</param>
    /// <returns>If load was successfull</returns>
    public static bool LoadGame(string playerName, out GameControl game)
    {
#if LOAD
        string destination = "./SavedGames/" + playerName + ".json";
        string jsonString;

        if (File.Exists(destination))
            jsonString = File.ReadAllText(destination);
        else
        {
            game = null;
            return false;
        }
        try
        {
            game = JsonConvert.DeserializeObject<GameControl>(jsonString, jsonSerializerSettings);
            ToolsUI.SetupUI();
            ToolsUI.LoadUI();
            foreach (var item in game.Items.Values)
            {
                item.AssignPrefab();
            }
            foreach (var item in GCon.game.Player.PlayerControl.backpack)
            {
                item.AssignPrefab();
            }
            foreach (var item in GCon.game.Player.PlayerControl.inBase)
            {
                item.AssignPrefab();
            }
            ToolsGame.AddBioms();
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            game = null;
            return false;
        }
        finally
        {
        }
#else
        game = null;
        return false;
#endif
    }

    public static void DeleteSaveFile(string playerName)
    {
        string destination = "./SavedGames/" + playerName + ".json";
        File.Delete(destination);
    }

    public static bool ScrambledEquals(List<PreUnit> list1, List<PreUnit> list2)
    {
        
        var cnt = new Dictionary<string, int>();
        foreach (PreUnit s in list1)
        {
            if (cnt.Keys.Any(x => x == s.Name))
            {
                cnt[s.Name]++;
            }
            else
            {
                cnt.Add(s.Name, 1);
            }
        }
        foreach (PreUnit s in list2)
        {
            if (cnt.Keys.Any(x => x == s.Name))
            {
                cnt[s.Name]--;
            }
            else
            {
                return false;
            }
        }
        return cnt.Values.All(c => c == 0);
    }


    /*
    public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(Item).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }

    public class BaseConverter : JsonConverter
    {
        static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Item));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            switch (jo["ObjType"].Value<int>())
            {
                case 1:
                    return JsonConvert.DeserializeObject<Exit>(jo.ToString(), SpecifiedSubclassConversion);
                case 2:
                    return JsonConvert.DeserializeObject<Player>(jo.ToString(), SpecifiedSubclassConversion);
                default:
                    throw new Exception();
            }
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }*/
}

