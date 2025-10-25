using System.IO;
using UnityEngine;

public static class ConfigLoader
{
    public static GameConfig Load()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "game_config.json");

        if (!File.Exists(path))
        {            
            return new GameConfig();
        }

        string json = File.ReadAllText(path);
        GameConfig config = JsonUtility.FromJson<GameConfig>(json);
        return config;
    }
}