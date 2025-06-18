using UnityEngine;

namespace Metroknight
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] GameObject[] maps;
        Bench bench;

        private void Start()
        {
            DisplaySavedMaps();
        }

        private void OnEnable()
        {
            bench = FindFirstObjectByType<Bench>();
            if (bench != null)
            {
                if (bench.interacted)
                {
                    UpdateMap();
                }
            }
        }

        void DisplaySavedMaps()
        {
            var savedScenes = SaveData.Instance.sceneNames;
            for (int i = 0; i < maps.Length; i++)
            {
                if (savedScenes.Contains(maps[i].name))
                {
                    maps[i].SetActive(true);
                }
                else
                {
                    maps[i].SetActive(false);
                }
            }
        }

        void UpdateMap()
        {
            var savedScenes = SaveData.Instance.sceneNames;
            var discoveredScenes = SaveData.Instance.discoveredSceneNames;

            for (int i = 0; i < maps.Length; i++)
            {
                if (discoveredScenes.Contains(maps[i].name))
                {
                    maps[i].SetActive(true);
                }
                else
                {
                    maps[i].SetActive(false);
                }
            }

            foreach (var map in discoveredScenes)
            {
                if (!savedScenes.Contains(map))
                {
                    SaveData.Instance.sceneNames.Add(map);
                    SaveData.Instance.scenesCount++;
                    SaveData.Instance.SaveMaps();
                }
            }
        }
    }
}
