
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class SplineElectricityWire : MonoBehaviour
{

    public enum ElectricityState
    {
        Off,
        Starting,
        On,
        Stopping
    }

    [Header("Spline")]
    [SerializeField] private SplineContainer splineContainer;
    [Min(2)]
    [SerializeField] private int segmentDensity = 100;

    [Header("Wire Geometry")]
    [Min(0.001f)]
    [SerializeField] private float wireRadius = 0.025f;
    [Range(3, 32)]
    [SerializeField] private int radialResolution = 3;

    [Header("Electricity Animation")]
    [Min(0f)]
    [SerializeField] private float flowSpeed = 2f;
    [Min(0.1f)]
    [SerializeField] private float tileCount = 5f;

    [Header("Electrical Jitter")]
    [SerializeField] private bool enableJitter = true;
    [Min(0f)]
    [SerializeField] private float jitterFrequency = 12f;
    [Min(0f)]
    [SerializeField] private float jitterAmplitude = 0.008f;

    [Header("Rendering")]
    [SerializeField] private Color emissionColor =
        new Color(0.1f, 0.8f, 1f, 1f);

    [Min(0f)]
    [SerializeField] private float emissionIntensity = 5f;

    
    [Header("Progressive Electrical Flow")]

    [Min(0.1f)]
    [SerializeField] private float flowDuration = 5f;

    [SerializeField] private bool loopFlow = false;

    [Range(0.01f, 1.1f)]
    [SerializeField] private float trailLength = 1.1f;

    [Range(0.001f, 0.1f)]
    [SerializeField] private float frontWidth = 0.02f;

    [Header("Electrical Controls")]

    [Min(0.1f)]
    [SerializeField] private float shutdownDuration = 3f;

    [SerializeField] private ElectricityState currentState =
        ElectricityState.Off;

    public ElectricityState CurrentState => currentState;
    public bool IsOn => currentState == ElectricityState.On;
    // Returns TRUE only when the electricity animation has completely finished stopping
    public bool IsOff => currentState == ElectricityState.Off;
    // Returns TRUE while electricity is actively flowing, starting, or shutting down
    public bool IsActive => currentState != ElectricityState.Off;
    // Returns TRUE during the shutdown fade-out animation phase
    public bool IsStopping => currentState == ElectricityState.Stopping;
    public float ElectricalProgress => electricalProgress;

    [Header("Electrical Direction")]
    public bool reverseDirection = false;

    private float electricalProgress;
    private float shutdownProgress;

    private float flowStartTime;


    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh generatedMesh;

    private MaterialPropertyBlock propertyBlock;

    private Vector3[] previousPositions;

    private static readonly int FlowSpeedID =
        Shader.PropertyToID("_FlowSpeed");

    private static readonly int TileCountID =
        Shader.PropertyToID("_TileCount");

    private static readonly int JitterFrequencyID =
        Shader.PropertyToID("_JitterFrequency");

    private static readonly int JitterAmplitudeID =
        Shader.PropertyToID("_JitterAmplitude");

    private static readonly int EmissionColorID =
        Shader.PropertyToID("_EmissionColor");

    private static readonly int EmissionIntensityID =
        Shader.PropertyToID("_EmissionIntensity");


    private static readonly int ProgressID =
        Shader.PropertyToID("_Progress");

    private static readonly int TrailLengthID =
        Shader.PropertyToID("_TrailLength");

    private static readonly int FrontWidthID =
        Shader.PropertyToID("_FrontWidth");



    private static readonly int OffProgressID =
        Shader.PropertyToID("_OffProgress");


    private void OnEnable()
    {
        Initialise();
        RebuildMesh();
        UpdateMaterialProperties();
        flowStartTime = Time.time;

        electricalProgress = 0f;
        shutdownProgress = 1f;
        currentState = ElectricityState.Off;
    }

    private void Initialise()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (currentState == ElectricityState.Off && !transform.hasChanged) return;
        if (splineContainer == null)
            return;

        if (meshFilter == null || meshRenderer == null)
            Initialise();

        if (MeshNeedsRebuilding())
            RebuildMesh();

        UpdateElectricityState();
        UpdateMaterialProperties();
    }

    private void OnValidate()
    {
        segmentDensity = Mathf.Max(2, segmentDensity);
        radialResolution = Mathf.Clamp(radialResolution, 3, 32);
        wireRadius = Mathf.Max(0.001f, wireRadius);

        if (!isActiveAndEnabled)
            return;

        Initialise();
        RebuildMesh();
        UpdateMaterialProperties();
    }

    private bool MeshNeedsRebuilding()
    {
        if (splineContainer == null)
            return false;

        if (generatedMesh == null ||
            meshFilter.sharedMesh != generatedMesh)
            return true;

        int sampleCount = segmentDensity + 1;

        if (previousPositions == null ||
            previousPositions.Length != sampleCount)
            return true;

        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            return true;
        }

        // Compare sampled spline positions to detect changes.
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / segmentDensity;

            if (!splineContainer.Evaluate(
                t,
                out float3 position,
                out float3 tangent,
                out float3 up))
                return true;

            Vector3 currentPosition = transform.InverseTransformPoint(
                new Vector3(position.x, position.y, position.z));

            if ((currentPosition - previousPositions[i]).sqrMagnitude
                > 0.0000001f)
                return true;
        }

        return false;
    }

    private void RebuildMesh()
    {
        if (splineContainer == null)
            return;

        if (meshFilter == null)
            Initialise();

        int rings = segmentDensity + 1;
        int sides = radialResolution;

        Vector3[] vertices = new Vector3[rings * (sides + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];

        int[] triangles = new int[
            segmentDensity * sides * 6];

        previousPositions = new Vector3[rings];

        float[] distances = new float[rings];

        Vector3 previousWorldPosition = Vector3.zero;

        // First sample: calculate the path positions and lengths.
        for (int i = 0; i < rings; i++)
        {
            float t = (float)i / segmentDensity;

            if (!splineContainer.Evaluate(
                t,
                out float3 position,
                out float3 tangent,
                out float3 up))
                return;

            Vector3 worldPosition = new Vector3(
                position.x, position.y, position.z);

            Vector3 localPosition =
                transform.InverseTransformPoint(worldPosition);

            previousPositions[i] = localPosition;

            if (i > 0)
                distances[i] = distances[i - 1] +
                    Vector3.Distance(previousWorldPosition, worldPosition);

            previousWorldPosition = worldPosition;
        }

        float totalLength = Mathf.Max(
            distances[rings - 1], 0.0001f);

        // Generate the tube rings.
        for (int i = 0; i < rings; i++)
        {
            float t = (float)i / segmentDensity;

            splineContainer.Evaluate(
                t,
                out float3 position,
                out float3 tangent,
                out float3 up);

            Vector3 worldPosition = new Vector3(
                position.x, position.y, position.z);

            Vector3 centre =
                transform.InverseTransformPoint(worldPosition);

            Vector3 direction =
                transform.InverseTransformVector(
                    new Vector3(tangent.x, tangent.y, tangent.z))
                .normalized;

            Vector3 upDirection =
                transform.InverseTransformVector(
                    new Vector3(up.x, up.y, up.z))
                .normalized;

            if (direction.sqrMagnitude < 0.0001f)
                direction = Vector3.forward;

            // Construct a perpendicular frame.
            Vector3 right = Vector3.Cross(
                upDirection, direction).normalized;

            if (right.sqrMagnitude < 0.0001f)
                right = Vector3.Cross(
                    Vector3.up, direction).normalized;

            Vector3 normal = Vector3.Cross(
                direction, right).normalized;

            // float uvX = distances[i] / totalLength;
            float uvX = distances[i] / totalLength;
            if (reverseDirection)
            {
                uvX = 1f - uvX;
            }

            for (int j = 0; j <= sides; j++)
            {
                float angle = (float)j / sides *
                    Mathf.PI * 2f;

                Vector3 radial =
                    right * Mathf.Cos(angle) +
                    normal * Mathf.Sin(angle);

                int index = i * (sides + 1) + j;

                vertices[index] =
                    centre + radial * wireRadius;

                uvs[index] = new Vector2(
                    uvX,
                    (float)j / sides);

                // Tangent direction points along the spline.
                tangents[index] = new Vector4(
                    direction.x,
                    direction.y,
                    direction.z,
                    1f);
            }
        }

        // Connect adjacent rings.
        int triangleIndex = 0;

        for (int i = 0; i < segmentDensity; i++)
        {
            for (int j = 0; j < sides; j++)
            {
                int a = i * (sides + 1) + j;
                int b = (i + 1) * (sides + 1) + j;
                int c = b + 1;
                int d = a + 1;

                // Outward-facing triangle winding.
                triangles[triangleIndex++] = a;
                triangles[triangleIndex++] = c;
                triangles[triangleIndex++] = b;

                triangles[triangleIndex++] = a;
                triangles[triangleIndex++] = d;
                triangles[triangleIndex++] = c;
            }
        }

        if (generatedMesh == null)
        {
            generatedMesh = new Mesh();
            generatedMesh.name = "Generated Electricity Wire";
        }

        generatedMesh.Clear();

        if (vertices.Length > 65535)
            generatedMesh.indexFormat =
                UnityEngine.Rendering.IndexFormat.UInt32;

        generatedMesh.vertices = vertices;
        generatedMesh.uv = uvs;
        generatedMesh.tangents = tangents;
        generatedMesh.triangles = triangles;

        generatedMesh.RecalculateNormals();
        generatedMesh.RecalculateBounds();

        meshFilter.sharedMesh = generatedMesh;

        transform.hasChanged = false;
    }

    private void UpdateMaterialProperties()
    {
        if (meshRenderer == null)
            return;

        meshRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetFloat(
            FlowSpeedID, flowSpeed);

        propertyBlock.SetFloat(
            TileCountID, tileCount);

        propertyBlock.SetFloat(
            JitterFrequencyID, jitterFrequency);

        propertyBlock.SetFloat(
            JitterAmplitudeID,
            enableJitter ? jitterAmplitude : 0f);

        propertyBlock.SetColor(
            EmissionColorID, emissionColor);

        propertyBlock.SetFloat(
            EmissionIntensityID, emissionIntensity);


        propertyBlock.SetFloat(
            ProgressID, electricalProgress);

        propertyBlock.SetFloat(
            OffProgressID, shutdownProgress);

        propertyBlock.SetFloat(
            TrailLengthID, trailLength);

        propertyBlock.SetFloat(
            FrontWidthID, frontWidth);


        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    private void OnDisable()
    {
        // Do not destroy a mesh asset that belongs to another object.
        if (meshFilter != null &&
            meshFilter.sharedMesh == generatedMesh)
            meshFilter.sharedMesh = null;

        if (generatedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(generatedMesh);
            else
                DestroyImmediate(generatedMesh);
        }

        generatedMesh = null;
        previousPositions = null;
    }


    
    private void UpdateElectricityState()
    {
        if (!Application.isPlaying)
            return;

        float delta = Time.deltaTime;

        switch (currentState)
        {
            case ElectricityState.Off:

                electricalProgress = 0f;
                shutdownProgress = 1f;
                break;

            case ElectricityState.Starting:

                electricalProgress += delta / flowDuration;

                electricalProgress = Mathf.Clamp01(
                    electricalProgress);

                if (electricalProgress >= 1f)
                {
                    currentState = ElectricityState.On;
                }

                break;

            case ElectricityState.On:

                electricalProgress = 1f;
                shutdownProgress = -0.1f;
                break;

            case ElectricityState.Stopping:

                shutdownProgress += delta / shutdownDuration;

                shutdownProgress = Mathf.Clamp01(
                    shutdownProgress);

                if (shutdownProgress >= 1f)
                {
                    currentState = ElectricityState.Off;
                    electricalProgress = 0f;
                }

                break;
        }
    }
    
    public void StartElectricity()
    {
        electricalProgress = 0f;
        shutdownProgress = -0.1f;

        currentState = ElectricityState.Starting;
    }

    public void StopElectricity()
    {
        if (currentState == ElectricityState.Off ||
            currentState == ElectricityState.Stopping)
            return;

        // Begin switching off from the start of the wire.
        shutdownProgress = 0f;

        currentState = ElectricityState.Stopping;
    }

    public void TurnElectricityOffImmediately()
    {
        electricalProgress = 0f;
        shutdownProgress = 1f;

        currentState = ElectricityState.Off;
    }

    public void SetReverseDirection(bool reversed, bool immediateRebuild = false)
    {
        if (reverseDirection != reversed)
        {
            reverseDirection = reversed;
            if (immediateRebuild) {
                RebuildMesh();
                UpdateMaterialProperties();
            }
        }
    }

}
