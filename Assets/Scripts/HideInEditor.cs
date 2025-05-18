using UnityEngine;

namespace Metroknight {
[ExecuteInEditMode]
public class HideInEditor : MonoBehaviour
{
    public bool hideInEditor = true;
    
    private Canvas canvas;
    private bool isPlaying;
    
    void OnEnable()
    {
        canvas = GetComponent<Canvas>();
        isPlaying = Application.isPlaying;
        UpdateVisibility();
    }
    
    void Update()
    {
        // Détecte si on passe du mode édition au mode jeu ou vice versa
        if (isPlaying != Application.isPlaying)
        {
            isPlaying = Application.isPlaying;
            UpdateVisibility();
        }
    }
    
    void UpdateVisibility()
    {
        if (canvas == null) return;
        
        if (hideInEditor && !Application.isPlaying)
        {
            // Cache le Canvas en mode édition
            canvas.enabled = false;
        }
        else
        {
            // Affiche le Canvas en mode jeu
            canvas.enabled = true;
        }
    }
    
    void OnValidate()
    {
        // Appelé quand une valeur est modifiée dans l'inspecteur
        UpdateVisibility();
    }
}
}