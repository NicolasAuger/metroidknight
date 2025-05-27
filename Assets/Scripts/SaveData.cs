
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

    public void Initialize()
    {
      if (sceneNames == null)
      {
        sceneNames = new HashSet<string>();
      }
    }
  }
}
