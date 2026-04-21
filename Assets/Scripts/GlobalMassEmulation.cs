using UnityEngine;

public class GlobalMassEmulation : MonoBehaviour
{
    [Tooltip("Wref - Reference weight; max weight or 2x mean weight")]
    [SerializeField] private float referenceW = 5.0f;

    [Tooltip("Scaling factor - strictly positive but defaults to 1")]
    [Range(0, 2f)]
    [SerializeField] private float scalingK = 1.0f;

    public static GlobalMassEmulation Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.LogError("GlobalMassEmulation Instance already present. This should never happen. This instance will remain dormant.", gameObject);
            return;
        }
    }

    public float CalculateScalingFactor(float mass)
    {
        float SF = 1 / (1 + scalingK * (mass / referenceW));
        return SF;
    }

    private void OnValidate()
    {
        GlobalMassEmulation[] massEmulationGOs = GameObject.FindObjectsByType<GlobalMassEmulation>(FindObjectsSortMode.None);
        if (massEmulationGOs.Length > 1)
        {
            Debug.LogError("Scene already contains GlobalMassEmulation.", massEmulationGOs[0]);
            Debug.LogError("This GlobalMassEmulation will be disabled.", gameObject);
            this.enabled = false;
        }

    }
}