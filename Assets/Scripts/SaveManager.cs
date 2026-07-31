using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string savePath;

    private SaveData data;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        savePath = Path.Combine(Application.persistentDataPath, "save.json");

        Load();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }


    public void Save()
    {
        string json = JsonUtility.ToJson(data, true);

        try
        {
            File.WriteAllText(savePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save failed: " + e.Message);
        }
    }

    public void Load()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                data = JsonUtility.FromJson<SaveData>(json);
            }
            catch
            {
                data = new SaveData();
                Save();
            }
        }
        else
        {
            data = new SaveData();
            Save();
        }

        if (data == null)
        {
            data = new SaveData();
        }
    }

    public void ResetSave()
    {
        data = new SaveData();
        Save();
    }


    // Coins

    public int GetCoins()
    {
        return data.coins;
    }

    public void SetCoins(int value)
    {
        data.coins = Mathf.Max(0, value);
        Save();
    }


    // High Score

    public int GetHighScore()
    {
        return data.highScore;
    }

    public void SetHighScore(int value)
    {
        if (value > data.highScore)
        {
            data.highScore = value;
            Save();
        }
    }


    // Settings

    public bool GetMuted()
    {
        return data.settings.muted;
    }

    public void SetMuted(bool value)
    {
        data.settings.muted = value;
        Save();
    }

    public bool GetNoHeadacheMode()
    {
        return data.settings.noHeadacheMode;
    }

    public void SetNoHeadacheMode(bool value)
    {
        data.settings.noHeadacheMode = value;
        Save();
    }


    // Tools

    public EquippedTool GetEquippedTool()
    {
        return data.tools.equipped;
    }

    public void SetEquippedTool(EquippedTool tool)
    {
        data.tools.equipped = tool;
        Save();
    }

    public bool GetSlowToolUnlocked()
    {
        return data.tools.unlockedSlow;
    }

    public void SetSlowToolUnlocked(bool value)
    {
        data.tools.unlockedSlow = value;
        Save();
    }

    public bool GetIndicatorToolUnlocked()
    {
        return data.tools.unlockedIndicator;
    }

    public void SetIndicatorToolUnlocked(bool value)
    {
        data.tools.unlockedIndicator = value;
        Save();
    }
}


public enum EquippedTool
{
    None = 0,
    Slow = 10,
    Indicator = 20
}


[System.Serializable]
public class SaveData
{
    public int coins;
    public int highScore;

    public SettingsData settings = new SettingsData();
    public ToolsData tools = new ToolsData();
}


[System.Serializable]
public class SettingsData
{
    public bool muted = false;
    public bool noHeadacheMode = false;
}


[System.Serializable]
public class ToolsData
{
    public EquippedTool equipped = EquippedTool.None;

    public bool unlockedSlow = false;
    public bool unlockedIndicator = false;
}