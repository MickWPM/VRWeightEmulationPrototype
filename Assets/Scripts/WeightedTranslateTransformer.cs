using UnityEngine;
using Oculus.Interaction;

public class WeightedTranslateTransformer : MonoBehaviour, ITransformer
{
    private IGrabbable _grabbable;
    private Pose _initialObjectPose;
    private Pose _initialGrabPose;

    [SerializeField]
    private Rigidbody grabbableRigidbody;
    private GlobalMassEmulation _globalMassEmulation;

    [SerializeField] private bool _overrideRBmass = false;
    [SerializeField] private float _overrideMassValue = 1.0f;

    private float Mass
    {
        get
        {
            return _overrideRBmass ? _overrideMassValue : grabbableRigidbody.mass;
        }
    }

    public void Initialize(IGrabbable grabbable)
    {
        _grabbable = grabbable;
    }

    private void Awake()
    {
        _globalMassEmulation =  GlobalMassEmulation.Instance;
        if (_globalMassEmulation == null)
        {
            Debug.LogError("No Global Mass Emulation instance. Mass emulation inactive.", gameObject);
            this.enabled = false;
        }

        SetupWeight();
    }

    private void SetupWeight()
    {
        if (_overrideRBmass)
            return;

        if (grabbableRigidbody == null)
        {
            grabbableRigidbody = GetComponent<Rigidbody>();
            if (grabbableRigidbody == null)
            {
                grabbableRigidbody = GetComponentInParent<Rigidbody>();
            }
            if (grabbableRigidbody == null)
            {
                grabbableRigidbody = GetComponentInChildren<Rigidbody>();
            }
        }
        if (grabbableRigidbody == null)
        {
            Debug.LogWarning("No rigidbody detected and override RB mass set to false. Using override mass value.", gameObject);
            _overrideRBmass = true;
        }
    }


    public void BeginTransform()
    {
        _initialObjectPose = _grabbable.Transform.GetPose();
        _initialGrabPose = _grabbable.GrabPoints[0];
    }

    public void UpdateTransform()
    {
        Pose currentGrabPose = _grabbable.GrabPoints[0];

        //We use total hand movement to then scale per paper's advanced approach
        Vector3 delta = currentGrabPose.position - _initialGrabPose.position;
        delta.y *= _globalMassEmulation.CalculateScalingFactor(Mass);
        _grabbable.Transform.position = _initialObjectPose.position + delta;

        Quaternion rotationDelta = currentGrabPose.rotation * Quaternion.Inverse(_initialGrabPose.rotation);
        _grabbable.Transform.rotation = rotationDelta * _initialObjectPose.rotation;
    }

    public void EndTransform() { }
}