using UnityEngine;

namespace Metroknight
{
    public class ItemDescription : MonoBehaviour
    {
        public GameObject itemDescription;

        void Start()
        {
            itemDescription.SetActive(false);
        }

        public void Show()
        {
            itemDescription.SetActive(true);
        }

        public void Hide()
        {
            itemDescription.SetActive(false);
        }
    }
}
