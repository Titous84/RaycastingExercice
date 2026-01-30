using UnityEngine;

// Source : https://envimmersif-cegepvicto.github.io/exercice_raycasting_peinture3d/

public class MouvementSimple : MonoBehaviour
{
    [SerializeField] private float amplitude = 2f;
    [SerializeField] private float vitesse = 2f;

    private Vector3 positionInitiale;

    private void Start()
    {
        positionInitiale = transform.position;
    }

    private void Update()
    {
        // Aller-retour simple sur l’axe X (sinusoïde)
        float decalage = Mathf.Sin(Time.time * vitesse) * amplitude;
        transform.position = positionInitiale + new Vector3(decalage, 0f, 0f);
    }
}
