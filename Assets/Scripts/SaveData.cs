
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace Metroknight
{
  [System.Serializable]
  public struct SaveData
  {
    public static SaveData Instance;

    // Map stuff
    public HashSet<string> sceneNames;
    public int scenesCount;
    public HashSet<string> discoveredSceneNames;
    public int discoveredScenesCount;

    // Bench stuff
    public string benchSceneName;
    public Vector2 benchPos;

    // Player stuff
    public int playerHealth;
    public float playerMana;
    public bool playerHalfMana;
    public Vector2 playerPosition;
    public string lastScene;
    public bool unlockedWallJump;
    public bool unlockedDash;

    // Shade stuff
    public Vector2 shadePos;
    public string sceneWithShade;
    public Quaternion shadeRot;


    public void Initialize()
    {
      // Tests purpose
      // DeleteFile("/save.maps.data");
      // DeleteFile("/save.discovered_maps.data");

      Debug.Log("Initializing SaveData...");
      EnsureFileExists("/save.bench.data");
      EnsureFileExists("/save.player.data");
      EnsureFileExists("/save.shade.data");
      EnsureFileExists("/save.maps.data");
      EnsureFileExists("/save.discovered_maps.data");

      if (sceneNames == null)
      {
        sceneNames = new HashSet<string>();
      }

      if (discoveredSceneNames == null)
      {
        discoveredSceneNames = new HashSet<string>();
      }

      LoadMaps();
      LoadDiscoveredMaps();
    }

    private void EnsureFileExists(string filePath)
    {
      if (!File.Exists(Application.persistentDataPath + filePath))
      {
        BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + filePath));
      }
    }

    public void SavePlayer()
    {
      using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.data")))
      {
        playerHealth = PlayerController.Instance.Health;
        playerMana = PlayerController.Instance.Mana;
        playerHalfMana = PlayerController.Instance.halfMana;
        playerPosition = PlayerController.Instance.transform.position;
        lastScene = SceneManager.GetActiveScene().name;
        unlockedWallJump = PlayerController.Instance.unlockedWallJump;
        unlockedDash = PlayerController.Instance.unlockedDash;

        writer.Write(playerHealth);
        writer.Write(playerMana);
        writer.Write(playerHalfMana);
        writer.Write(playerPosition.x);
        writer.Write(playerPosition.y);
        writer.Write(lastScene);
        writer.Write(unlockedWallJump);
        writer.Write(unlockedDash);
      }
    }

    public void SaveBench()
    {
      EnsureFileExists("/save.bench.data");
      using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.bench.data")))
      {
        writer.Write(benchSceneName);
        writer.Write(benchPos.x);
        writer.Write(benchPos.y);
      }
    }

    public void LoadBench()
    {
      EnsureFileExists("/save.bench.data");
      string filePath = Application.persistentDataPath + "/save.bench.data";
      FileInfo fileInfo = new FileInfo(filePath);
      if (fileInfo.Length == 0)
      {
        Debug.Log("Bench save file is empty, not loading any bench data.");
        return;
      }
      using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
      {
        benchSceneName = reader.ReadString();
        benchPos.x = reader.ReadSingle();
        benchPos.y = reader.ReadSingle();
      }
    }

    public void LoadPlayer()
    {
      string filePath = Application.persistentDataPath + "/save.player.data";
      if (File.Exists(filePath))
      {
        FileInfo fileInfo = new FileInfo(filePath);
        if (fileInfo.Length == 0)
        {
          Debug.Log("Player save file is empty, not loading any player.");
          SetDefaultPlayerValues();
          return;
        }
        using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
        {
          playerHealth = reader.ReadInt32();
          playerMana = reader.ReadSingle();
          playerHalfMana = reader.ReadBoolean();
          playerPosition.x = reader.ReadSingle();
          playerPosition.y = reader.ReadSingle();
          lastScene = reader.ReadString();
          unlockedWallJump = reader.ReadBoolean();
          unlockedDash = reader.ReadBoolean();

          SceneManager.LoadScene(lastScene); // Load the last scene
          PlayerController.Instance.transform.position = playerPosition;
          PlayerController.Instance.Health = playerHealth;
          PlayerController.Instance.Mana = playerMana;
          PlayerController.Instance.halfMana = playerHalfMana;
          PlayerController.Instance.unlockedWallJump = unlockedWallJump;
          PlayerController.Instance.unlockedDash = unlockedDash;
        }
      }
      else
      {
        SetDefaultPlayerValues();
      }
    }

    public void SetDefaultPlayerValues()
    {
      // If no player data exists, set default values
      PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
      PlayerController.Instance.Mana = 0.5f;
      PlayerController.Instance.halfMana = false;
      PlayerController.Instance.unlockedWallJump = false;
      PlayerController.Instance.unlockedDash = false;
    }

    public void SaveShade()
    {
      EnsureFileExists("/save.shade.data");
      using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.shade.data")))
      {
        sceneWithShade = SceneManager.GetActiveScene().name;
        shadePos = Shade.Instance.transform.position;
        shadeRot = Shade.Instance.transform.rotation;
        writer.Write(sceneWithShade);

        writer.Write(shadePos.x);
        writer.Write(shadePos.y);

        writer.Write(shadeRot.x);
        writer.Write(shadeRot.y);
        writer.Write(shadeRot.z);
        writer.Write(shadeRot.w);
      }
    }

    public void LoadShade()
    {
      EnsureFileExists("/save.shade.data");
      string filePath = Application.persistentDataPath + "/save.shade.data";
      FileInfo fileInfo = new FileInfo(filePath);
      if (fileInfo.Length == 0)
      {
        Debug.Log("Shade save file is empty, not loading any shade.");
        return;
      }
      using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
      {
        sceneWithShade = reader.ReadString();
        shadePos.x = reader.ReadSingle();
        shadePos.y = reader.ReadSingle();

        float rotX = reader.ReadSingle();
        float rotY = reader.ReadSingle();
        float rotZ = reader.ReadSingle();
        float rotW = reader.ReadSingle();
        shadeRot = new Quaternion(rotX, rotY, rotZ, rotW);
      }
    }

    public void SaveMaps()
    {
      EnsureFileExists("/save.maps.data");
      using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.maps.data")))
      {
        scenesCount = sceneNames.Count;
        writer.Write(scenesCount);
        foreach (string sceneName in sceneNames)
        {
          writer.Write(sceneName);
        }
      }
    }

    public void SaveDiscoveredMaps()
    {
      EnsureFileExists("/save.discovered_maps.data");
      using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.discovered_maps.data")))
      {
        discoveredScenesCount = discoveredSceneNames.Count;
        writer.Write(discoveredScenesCount);
        foreach (string sceneName in discoveredSceneNames)
        {
          writer.Write(sceneName);
        }
      }
    }

    public void LoadMaps()
    {
      string filePath = Application.persistentDataPath + "/save.maps.data";
      if (File.Exists(filePath))
      {
        FileInfo fileInfo = new FileInfo(filePath);
        if (fileInfo.Length == 0)
        {
          Debug.Log("Maps save file is empty, not loading any maps.");
          return;
        }
        using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
        {
          scenesCount = reader.ReadInt32();
          sceneNames.Clear();
          for (int i = 0; i < scenesCount; i++)
          {
            string sceneName = reader.ReadString();
            sceneNames.Add(sceneName);
          }
        }
      }
      else
      {
        Debug.Log("No map file found.");
        sceneNames.Clear();
      }
    }

    public void LoadDiscoveredMaps()
    {
      string filePath = Application.persistentDataPath + "/save.discovered_maps.data";
      if (File.Exists(filePath))
      {
        FileInfo fileInfo = new FileInfo(filePath);
        if (fileInfo.Length == 0)
        {
          Debug.Log("Discovered maps save file is empty, not loading any discovered maps.");
          return;
        }
        using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
        {
          discoveredScenesCount = reader.ReadInt32();
          discoveredSceneNames.Clear();
          for (int i = 0; i < discoveredScenesCount; i++)
          {
            string sceneName = reader.ReadString();
            discoveredSceneNames.Add(sceneName);
          }
        }
      }
      else
      {
        Debug.Log("No discovered map file found.");
        discoveredSceneNames.Clear();
      }
    }

    public void DeleteFile(string _path)
    {
      string filePath = Application.persistentDataPath + _path;
      if (File.Exists(filePath))
      {
        Debug.Log("Deleting file: " + filePath);
        File.Delete(filePath);
      }
      else
      {
        Debug.LogWarning("File not found: " + filePath);
      }
    }
  }
}
