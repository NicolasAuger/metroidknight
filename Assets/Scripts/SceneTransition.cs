using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroknight {
public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string transitionTo;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector2 exitDirection;
    [SerializeField] private float exitTime;

    private void Start()
    {
        if (transitionTo == GameManager.Instance.transitionedFromScene) {
            PlayerController.Instance.transform.position = startPoint.position;
            // Fix weird inverted direction while changing scenes loading
            StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection, exitTime));
        }
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
    }


    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player")) {
            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;
            PlayerController.Instance.pState.cutscene = true;
            PlayerController.Instance.pState.invincible = true;
            StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
        }
    }
}
}
