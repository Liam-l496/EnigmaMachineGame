
using UnityEngine;
using GogoGaga.OptimizedRopesAndCables;

[ExecuteAlways]
[RequireComponent(typeof(Rope))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RopeElectricityWire : MonoBehaviour
{
    public enum ElectricityState
    {
        Off,
        Starting,
        On,
        Stopping
    }

    [Header("Rope Reference")]
    [SerializeField] private Rope rope;

    [Header("Wire Geometry")]
    [Min(0.001f)]
    [SerializeField] private float wireRadius = 0.025f;

    [Range(3, 32)]
    [SerializeField] private int radialResolution = 3;

    [Min(1)]
    [SerializeField] private int segmentsPerRopePoint = 5;

    [Header("Electricity Animation")]
    [Min(0.1f)]
    [SerializeField] private float flowDuration = 5f;

    [Min(0.1f)]
    [SerializeField] private float shutdownDuration = 3f;

    [Range(0.001f, 0.1f)]
    [SerializeField] private float frontWidth = 0.02f;

    [Range(0.01f, 1.1f)]
    [SerializeField] private float trailLength = 1.1f;

    [Header("Electrical Appearance")]
    [Min(0f)]
    [SerializeField] private float flowSpeed = 2f;

    [Min(0.1f)]
    [SerializeField] private float tileCount = 5f;

    [Min(0f)]
    [SerializeField] private float jitterFrequency = 12f;

    [Min(0f)]
    [SerializeField] private float jitterAmplitude = 0.008f;

    [SerializeField] private Color emissionColor =
        new Color(0.1f, 0.8f, 1f, 1f);

    [Min(0f)]
    [SerializeField] private float emissionIntensity = 5f;

    [Header("Rope Renderer")]
    [SerializeField] private bool hideOriginalLineRenderer = true;

    [Header("Current State")]
    [SerializeField] private ElectricityState currentState =
        ElectricityState.Off;

    public ElectricityState CurrentState => currentState;
    public bool IsOn => currentState == ElectricityState.On;
    public bool IsOff => currentState == ElectricityState.Off;
    // Returns TRUE while electricity is actively flowing, starting, or shutting down
    public bool IsActive => currentState != ElectricityState.Off;
    // Returns TRUE during the shutdown fade-out animation phase
    public bool IsStopping => currentState == ElectricityState.Stopping;

    public float ElectricalProgress => electricalProgress;

    [Header("Electrical Direction")]
    public bool reverseDirection = false;

    private float electricalProgress;
    private float shutdownProgress = 1f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private LineRenderer originalLineRenderer;

    private Mesh generatedMesh;
    private MaterialPropertyBlock propertyBlock;

    private Vector3[] vertices;
    private Vector2[] uvs;
    private Vector4[] tangents;
    private Vector3[] samplePositions;
    private float[] distances;
    private int[] triangles;

    private int rings;
    private int sides;

    private bool geometryInitialised;

    private static readonly int ProgressID =
        Shader.PropertyToID("_Progress");

    private static readonly int OffProgressID =
        Shader.PropertyToID("_OffProgress");

    private static readonly int TrailLengthID =
        Shader.PropertyToID("_TrailLength");

    private static readonly int FrontWidthID =
        Shader.PropertyToID("_FrontWidth");

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


    private void OnEnable()
    {
        Initialise();

        electricalProgress = 0f;
        shutdownProgress = 1f;

        currentState = ElectricityState.Off;

        CreateMesh();
        UpdateMaterialProperties();
    }


    private void Initialise()
    {
        rope = GetComponent<Rope>();

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        originalLineRenderer = GetComponent<LineRenderer>();

        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        if (hideOriginalLineRenderer &&
            originalLineRenderer != null)
        {
            originalLineRenderer.enabled = false;
        }
    }


    private void Update()
    {
        if (!Application.isPlaying)
            return;
        
        if (currentState == ElectricityState.Off && !transform.hasChanged) return;

        UpdateElectricityState();
        UpdateMaterialProperties();
    }


    private void LateUpdate()
    {
        if (!HasValidEndpoints() || !enabled)
            return;

        if (!rope.AreEndPointsValid()) return;

        if (!geometryInitialised)
            CreateMesh();

        UpdateRopeMesh();
    }

    private bool HasValidEndpoints()
        {
            return rope != null && rope.AreEndPointsValid();
        }


    private void OnValidate()
    {
        // wireRadius = Mathf.Max(0.001f, wireRadius);

        // radialResolution = Mathf.Clamp(
        //     radialResolution, 3, 32);

        // segmentsPerRopePoint = Mathf.Max(
        //     1, segmentsPerRopePoint);

        // if (!isActiveAndEnabled)
        //     return;

        // if (meshFilter == null)
        //     Initialise();

        // geometryInitialised = false;

        // if (!Application.isPlaying)
        // {
        //     CreateMesh();
        //     UpdateRopeMesh();
        // }

        // UpdateMaterialProperties();
        wireRadius = Mathf.Max(0.001f, wireRadius);
        radialResolution = Mathf.Clamp(radialResolution, 3, 32);
        segmentsPerRopePoint = Mathf.Max(1, segmentsPerRopePoint);

        if (!isActiveAndEnabled)
            return;

        Initialise();
        geometryInitialised = false;

        if (!Application.isPlaying)
        {
            CreateMesh();
            UpdateRopeMesh();
        }

        UpdateMaterialProperties();
    }


    // ----------------------------------
    // MESH CREATION
    // ----------------------------------

    private void CreateMesh()
    {
        if (!HasValidEndpoints())
        {
            geometryInitialised = false;
            return;
        }
        if (meshFilter == null)
            Initialise();

        rings = Mathf.Max(
            2, rope.linePoints * segmentsPerRopePoint) + 1;

        sides = radialResolution;

        int vertexCount = rings * (sides + 1);

        vertices = new Vector3[vertexCount];
        uvs = new Vector2[vertexCount];
        tangents = new Vector4[vertexCount];

        samplePositions = new Vector3[rings];
        distances = new float[rings];

        triangles = new int[
            (rings - 1) * sides * 6];

        int triangleIndex = 0;

        for (int i = 0; i < rings - 1; i++)
        {
            for (int j = 0; j < sides; j++)
            {
                int a = i * (sides + 1) + j;
                int b = (i + 1) * (sides + 1) + j;
                int c = b + 1;
                int d = a + 1;

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

            generatedMesh.name =
                "Generated Electrical Rope";

            generatedMesh.MarkDynamic();
        }

        generatedMesh.Clear();

        generatedMesh.vertices = vertices;
        generatedMesh.uv = uvs;
        generatedMesh.tangents = tangents;
        generatedMesh.triangles = triangles;

        meshFilter.sharedMesh = generatedMesh;

        geometryInitialised = true;
    }


    // ----------------------------------
    // ROPE MESH UPDATES
    // ----------------------------------

    private void UpdateRopeMesh()
    {
        if (!HasValidEndpoints())
        {
            return;
        }
        if (rope == null || generatedMesh == null)
            return;

        // Sample positions along the GogoGaga rope.
        for (int i = 0; i < rings; i++)
        {
            float t = (float)i / (rings - 1);

            samplePositions[i] = rope.GetPointAt(t);

            if (i == 0)
            {
                distances[i] = 0f;
            }
            else
            {
                distances[i] = distances[i - 1] +
                    Vector3.Distance(
                        samplePositions[i - 1],
                        samplePositions[i]);
            }
        }

        float totalLength = Mathf.Max(
            distances[rings - 1], 0.0001f);

        // Generate tube geometry.
        for (int i = 0; i < rings; i++)
        {
            Vector3 centre = samplePositions[i];

            float t = (float)i / (rings - 1);

            float previousT = Mathf.Max(
                0f, t - 0.001f);

            float nextT = Mathf.Min(
                1f, t + 0.001f);

            Vector3 tangentWorld = (
                rope.GetPointAt(nextT) -
                rope.GetPointAt(previousT)
            ).normalized;

            if (tangentWorld.sqrMagnitude < 0.0001f)
                tangentWorld = Vector3.forward;

            // Build a perpendicular frame.
            Vector3 referenceUp = Vector3.up;

            if (Mathf.Abs(
                Vector3.Dot(referenceUp, tangentWorld)) > 0.98f)
            {
                referenceUp = Vector3.right;
            }

            Vector3 rightWorld = Vector3.Cross(
                referenceUp, tangentWorld).normalized;

            Vector3 normalWorld = Vector3.Cross(
                tangentWorld, rightWorld).normalized;

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
                    rightWorld * Mathf.Cos(angle) +
                    normalWorld * Mathf.Sin(angle);

                Vector3 worldVertex =
                    centre + radial * wireRadius;

                int index = i * (sides + 1) + j;

                vertices[index] =
                    transform.InverseTransformPoint(worldVertex);

                uvs[index] = new Vector2(
                    uvX,
                    (float)j / sides);

                Vector3 localTangent =
                    transform.InverseTransformDirection(
                        tangentWorld).normalized;

                tangents[index] = new Vector4(
                    localTangent.x,
                    localTangent.y,
                    localTangent.z,
                    1f);
            }
        }

        generatedMesh.vertices = vertices;
        generatedMesh.uv = uvs;
        generatedMesh.tangents = tangents;

        generatedMesh.RecalculateNormals();
        generatedMesh.RecalculateBounds();

        // Allow room for shader vertex displacement.
        Bounds bounds = generatedMesh.bounds;

        bounds.Expand(jitterAmplitude * 4f);

        generatedMesh.bounds = bounds;
    }


    // ----------------------------------
    // ELECTRICITY STATE CONTROL
    // ----------------------------------

    private void UpdateElectricityState()
    {
        float delta = Time.deltaTime;

        switch (currentState)
        {
            case ElectricityState.Off:

                electricalProgress = 0f;
                shutdownProgress = 1f;

                break;


            case ElectricityState.Starting:

                electricalProgress +=
                    delta / flowDuration;

                electricalProgress = Mathf.Clamp01(
                    electricalProgress);

                shutdownProgress = -0.1f;

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

                shutdownProgress +=
                    delta / shutdownDuration;

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

        shutdownProgress = 0f;

        currentState = ElectricityState.Stopping;
    }


    public void TurnElectricityOffImmediately()
    {
        electricalProgress = 0f;
        shutdownProgress = 1f;

        currentState = ElectricityState.Off;

        UpdateMaterialProperties();
    }


    // ----------------------------------
    // MATERIAL PROPERTIES
    // ----------------------------------

    private void UpdateMaterialProperties()
    {
        if (meshRenderer == null)
            return;

        meshRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetFloat(
            ProgressID, electricalProgress);

        propertyBlock.SetFloat(
            OffProgressID, shutdownProgress);

        propertyBlock.SetFloat(
            TrailLengthID, trailLength);

        propertyBlock.SetFloat(
            FrontWidthID, frontWidth);

        propertyBlock.SetFloat(
            FlowSpeedID, flowSpeed);

        propertyBlock.SetFloat(
            TileCountID, tileCount);

        propertyBlock.SetFloat(
            JitterFrequencyID, jitterFrequency);

        propertyBlock.SetFloat(
            JitterAmplitudeID, jitterAmplitude);

        propertyBlock.SetColor(
            EmissionColorID, emissionColor);

        propertyBlock.SetFloat(
            EmissionIntensityID, emissionIntensity);

        meshRenderer.SetPropertyBlock(propertyBlock);
    }


    // ----------------------------------
    // CLEANUP
    // ----------------------------------

    private void OnDisable()
    {
        if (originalLineRenderer != null)
            originalLineRenderer.enabled = true;

        if (meshFilter != null &&
            meshFilter.sharedMesh == generatedMesh)
        {
            meshFilter.sharedMesh = null;
        }

        if (generatedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(generatedMesh);
            else
                DestroyImmediate(generatedMesh);
        }

        generatedMesh = null;
        geometryInitialised = false;
    }


    // public void SetReverseDirection(bool reversed)
    // {
    //     reverseDirection = reversed;
    // }
        public void SetReverseDirection(bool reversed, bool immediateRebuild = false)
    {
        if (reverseDirection != reversed)
        {
            reverseDirection = reversed;
            if (immediateRebuild) {
                CreateMesh();
                UpdateMaterialProperties();
            }
        }
    }
}
