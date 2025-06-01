using UnityEngine;
using UnityEngine.UI;

namespace Metroknight
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] Image heartShards;
        [SerializeField] Image manaShards;
        [SerializeField] GameObject sideCast, upCast, downCast;
        [SerializeField] GameObject wallJump, dash, multipleJumps;

        private void OnEnable()
        {
            heartShards.fillAmount = PlayerController.Instance.heartShards * .25f;
            manaShards.fillAmount = PlayerController.Instance.orbShards * .34f;

            // In this sandbox, every spells are already unlocked for now
            sideCast.SetActive(true);
            upCast.SetActive(true);
            // downCast.SetActive(true); // ugly

            wallJump.SetActive(PlayerController.Instance.unlockedWallJump);
            dash.SetActive(PlayerController.Instance.unlockedDash);
            multipleJumps.SetActive(PlayerController.Instance.unlockedMultipleJumps);
        }
    }
}
