using Unity.Mathematics;
using UnityEngine;

public class VFXForceField : MonoBehaviour
{
    [Header("Shape")]
    public float StartRange;
    public float EndRange;

    [Header("Direction")]
    public float X;
    public float Y;
    public float Z;

    [Header("Gravity")]
    public AnimationCurve Strength = AnimationCurve.Linear(0, 1, 1, 1);
    public float GravityFocus;

    [Header("Rotation")]
    public float Speed;
    public float Attraction;
    public Vector2 RotationRandomness;

    public ForceFieldData GetData() => new ForceFieldData()
    {
        Center = this.transform.position,
        StartRange = this.StartRange,
        EndRange = this.EndRange,
        Direction = new float3(X, Y, Z),
        Strength = new float3x3(Strength.Evaluate(0), Strength.Evaluate(0.125f), Strength.Evaluate(0.25f),
                                Strength.Evaluate(0.375f), Strength.Evaluate(0.5f), Strength.Evaluate(0.625f), 
                                Strength.Evaluate(0.75f), Strength.Evaluate(0.875f), Strength.Evaluate(1f)),
        GravityFocus = Mathf.Clamp01(this.GravityFocus),
        Speed = this.Speed,
        Attraction = Mathf.Clamp01(this.Attraction),
        RotationRandomness = new float2(Mathf.Clamp01(RotationRandomness.x), Mathf.Clamp01(RotationRandomness.y))
    };

    private void OnDrawGizmos()
    {
        var position = transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, StartRange);
        Gizmos.DrawWireSphere(position, EndRange);
    }
}

public struct ForceFieldData
{
    public float3 Center;
    public float StartRange;
    public float EndRange;

    public float3 Direction;

    public float3x3 Strength;
    public float GravityFocus;

    public float Speed;
    public float Attraction;
    public float2 RotationRandomness;
}