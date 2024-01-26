using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using Klak.TestTools;
using MediaPipe.FaceMesh;

public sealed class Visualizer : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] ImageSource _source = null;
    [Space]
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] Shader _shader = null;
    [Space]
    [SerializeField] RawImage _mainUI = null;
    [SerializeField] RawImage _faceUI = null;
    [SerializeField] RawImage _leftEyeUI = null;
    [SerializeField] RawImage _rightEyeUI = null;

    public bool _faceSet = false;
    public float _mouthWidth, _mouthHeight;
    [Range(0, 1)]
    public float _similarity = 0f;

    #endregion

    #region Private members

    FacePipeline _pipeline;
    Material _material;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _pipeline = new FacePipeline(_resources);
        _material = new Material(_shader);
    }

    void Update() {
        var mouthTop = _pipeline.GetVertexPosition(13);
            var mouthBottom = _pipeline.GetVertexPosition(14);
            // get the distance between them to get if the user is smiling
            var mouthVerticalDistance = Vector3.Distance(mouthTop, mouthBottom);
            // get the mouth left and right points
            var mouthLeft = _pipeline.GetVertexPosition(61);
            var mouthRight = _pipeline.GetVertexPosition(291);
            // get the distance between them to get if the user is smiling
            var mouthHorizontalDistance = Vector3.Distance(mouthLeft, mouthRight);

        if (Input.GetKeyDown(KeyCode.Space)) {
            _mouthHeight = mouthVerticalDistance;
            _mouthWidth = mouthHorizontalDistance;
            _faceSet = true;
        }
    if (_faceSet)
    {
        // calculate the similarity between the mouth height and width averages against the user's mouth
        _similarity = (mouthVerticalDistance / _mouthHeight + mouthHorizontalDistance / _mouthWidth) / 2;
    }

    }

    void OnDestroy()
    {
        _pipeline.Dispose();
        Destroy(_material);
    }

    void LateUpdate()
    {
        // Processing on the face pipeline
        _pipeline.ProcessImage(_source.Texture);

        // UI update
        _mainUI.texture = _source.Texture;
        _faceUI.texture = _pipeline.CroppedFaceTexture;
        _leftEyeUI.texture = _pipeline.CroppedLeftEyeTexture;
        _rightEyeUI.texture = _pipeline.CroppedRightEyeTexture;
    }

    void OnRenderObject()
    {
        // Main view overlay
        var mv = float4x4.Translate(math.float3(-0.875f, -0.5f, 0));
        _material.SetBuffer("_Vertices", _pipeline.RefinedFaceVertexBuffer);
        _material.SetPass(1);
        Graphics.DrawMeshNow(_resources.faceLineTemplate, mv);

        // Face view
        // Face mesh
        var fF = MathUtil.ScaleOffset(0.5f, math.float2(0.125f, -0.5f));
        _material.SetBuffer("_Vertices", _pipeline.RefinedFaceVertexBuffer);
        _material.SetPass(0);
        Graphics.DrawMeshNow(_resources.faceMeshTemplate, fF);

        // Left eye
        var fLE = math.mul(fF, _pipeline.LeftEyeCropMatrix);
        _material.SetMatrix("_XForm", fLE);
        _material.SetBuffer("_Vertices", _pipeline.RawLeftEyeVertexBuffer);
        _material.SetPass(3);
        Graphics.DrawProceduralNow(MeshTopology.Lines, 64, 1);

        // Right eye
        var fRE = math.mul(fF, _pipeline.RightEyeCropMatrix);
        _material.SetMatrix("_XForm", fRE);
        _material.SetBuffer("_Vertices", _pipeline.RawRightEyeVertexBuffer);
        _material.SetPass(3);
        Graphics.DrawProceduralNow(MeshTopology.Lines, 64, 1);

        // Debug views
        // Face mesh
        var dF = MathUtil.ScaleOffset(0.5f, math.float2(0.125f, 0));
        _material.SetBuffer("_Vertices", _pipeline.RawFaceVertexBuffer);
        _material.SetPass(1);
        Graphics.DrawMeshNow(_resources.faceLineTemplate, dF);

        // Left eye
        var dLE = MathUtil.ScaleOffset(0.25f, math.float2(0.625f, 0.25f));
        _material.SetMatrix("_XForm", dLE);
        _material.SetBuffer("_Vertices", _pipeline.RawLeftEyeVertexBuffer);
        _material.SetPass(3);
        Graphics.DrawProceduralNow(MeshTopology.Lines, 64, 1);

        // Right eye
        var dRE = MathUtil.ScaleOffset(0.25f, math.float2(0.625f, 0f));
        _material.SetMatrix("_XForm", dRE);
        _material.SetBuffer("_Vertices", _pipeline.RawRightEyeVertexBuffer);
        _material.SetPass(3);
        Graphics.DrawProceduralNow(MeshTopology.Lines, 64, 1);
    }

    #endregion
}
