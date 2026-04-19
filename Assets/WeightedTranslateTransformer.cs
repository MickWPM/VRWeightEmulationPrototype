using UnityEngine;
using Oculus.Interaction;

public class WeightedTranslateTransformer : MonoBehaviour, ITransformer
{
    [SerializeField, Range(0, 1)] private float _verticalWeight = 0.5f;
    private IGrabbable _grabbable;
    private Pose _initialObjectPose;
    private Pose _initialGrabPose;

    public void Initialize(IGrabbable grabbable) => _grabbable = grabbable;

    public void BeginTransform()
    {
        // Meta Extension: GetPose() requires 'using Oculus.Interaction;'
        _initialObjectPose = _grabbable.Transform.GetPose();

        // GrabPoints[0] is the Pose of the hand/controller currently grabbing
        _initialGrabPose = _grabbable.GrabPoints[0];
    }

    public void UpdateTransform()
    {
        Pose currentGrabPose = _grabbable.GrabPoints[0];

        // Calculate raw hand movement
        Vector3 delta = currentGrabPose.position - _initialGrabPose.position;

        // Apply vertical weight (Heavy feel)
        delta.y *= _verticalWeight;

        // Update position
        _grabbable.Transform.position = _initialObjectPose.position + delta;

        // Keep rotation 1:1
        Quaternion rotationDelta = currentGrabPose.rotation * Quaternion.Inverse(_initialGrabPose.rotation);
        _grabbable.Transform.rotation = rotationDelta * _initialObjectPose.rotation;
    }

    public void EndTransform() { }
}