using UnityEngine;
using UnityEngine.InputSystem;

// Source : https://envimmersif-cegepvicto.github.io/exercice_raycasting_peinture3d/
// Inspiré de : https://envimmersif-cegepvicto.github.io/reference_input_actions/ (performed, génération C#)
// Inspiré de : https://envimmersif-cegepvicto.github.io/raycasting_3d_theorie/ (ScreenPointToRay, RaycastHit, LayerMask)

public class PlacementRaycast : MonoBehaviour
{
    [Header("Prefab et placement")]
    [SerializeField] private GameObject cubePrefab;

    [Header("Filtre de raycast (décocher Ignorable dans l'inspector)")]
    [SerializeField] private LayerMask masqueRaycast;

    [Header("Couleur active")]
    [SerializeField] private Color couleurActive = Color.red;

    private PlayerInputActions actionsEntree;

    private void Awake()
    {
        actionsEntree = new PlayerInputActions();
    }

    private void OnEnable()
    {
        actionsEntree.Enable();

        // Actions principales (événement performed)
        actionsEntree.Player.Placer.performed += QuandPlacer;
        actionsEntree.Player.Supprimer.performed += QuandSupprimer;

        // Couleurs (5 touches)
        actionsEntree.Player.Couleur1.performed += _ => ChangerCouleur(Color.red);
        actionsEntree.Player.Couleur2.performed += _ => ChangerCouleur(Color.green);
        actionsEntree.Player.Couleur3.performed += _ => ChangerCouleur(Color.blue);
        actionsEntree.Player.Couleur4.performed += _ => ChangerCouleur(Color.yellow);
        actionsEntree.Player.Couleur5.performed += _ => ChangerCouleur(Color.magenta);
    }

    private void OnDisable()
    {
        // Important : se désabonner pour éviter les doubles abonnements si l’objet est réactivé
        actionsEntree.Player.Placer.performed -= QuandPlacer;
        actionsEntree.Player.Supprimer.performed -= QuandSupprimer;

        actionsEntree.Disable();
    }

    private void ChangerCouleur(Color nouvelleCouleur)
    {
        couleurActive = nouvelleCouleur;
    }

    private void QuandPlacer(InputAction.CallbackContext contexte)
    {
        if (cubePrefab == null)
        {
            Debug.LogError("CubePrefab n'est pas assigné dans l'Inspector.");
            return;
        }

        if (Camera.main == null)
        {
            Debug.LogError("Aucune caméra avec le tag MainCamera (Camera.main est null).");
            return;
        }

        // Lire la position du pointeur via Input Actions (Vector2)
        Vector2 positionPointeur = actionsEntree.Player.PositionPointeur.ReadValue<Vector2>();

        Ray rayon = Camera.main.ScreenPointToRay(positionPointeur);
        if (Physics.Raycast(rayon, out RaycastHit impact, Mathf.Infinity, masqueRaycast))
        {
            GameObject nouveauCube = Instantiate(cubePrefab, impact.point, Quaternion.identity);

            // Appliquer la couleur active (si l’objet a un Renderer)
            Renderer rendu = nouveauCube.GetComponent<Renderer>();
            if (rendu != null)
            {
                rendu.material.color = couleurActive;
            }
        }
    }

    private void QuandSupprimer(InputAction.CallbackContext contexte)
    {
        if (Camera.main == null)
        {
            Debug.LogError("Aucune caméra avec le tag MainCamera (Camera.main est null).");
            return;
        }

        Vector2 positionPointeur = actionsEntree.Player.PositionPointeur.ReadValue<Vector2>();

        Ray rayon = Camera.main.ScreenPointToRay(positionPointeur);
        if (Physics.Raycast(rayon, out RaycastHit impact, Mathf.Infinity, masqueRaycast))
        {
            GameObject objetTouche = impact.collider.gameObject;

            // L'énoncé suggère d'utiliser les tags pour reconnaître un cube
            if (objetTouche.CompareTag("CubePeinture"))
            {
                Destroy(objetTouche);
            }
        }
    }

    private void Update()
    {
        // Option utile pour la remise : visualiser le rayon dans la vue Scene
        // (visible dans Scene, pas dans Game)
        if (Camera.main == null) return;

        Vector2 positionPointeur = actionsEntree.Player.PositionPointeur.ReadValue<Vector2>();
        Ray rayon = Camera.main.ScreenPointToRay(positionPointeur);

        Debug.DrawRay(rayon.origin, rayon.direction * 100f, Color.red, 0f);
    }
}
