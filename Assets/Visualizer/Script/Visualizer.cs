using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using Klak.TestTools;
using MediaPipe.FaceMesh;
using System.Collections.Generic;
using Fungus;

public sealed class Visualizer : Singleton<Visualizer>
{
    #region member variables

    public class MouthDimensions {
        public float width, height;
    }

    [SerializeField] ImageSource _source = null;
    [Space]
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] Shader _shader = null;
    [Space]
    // [SerializeField] RawImage _mainUI = null;
    [SerializeField] RawImage _faceUI = null;
    // [SerializeField] RawImage _leftEyeUI = null;
    // [SerializeField] RawImage _rightEyeUI = null;

    public List<Texture2D> _screenshots = new List<Texture2D>();

    public bool _settingFace, _faceSet = false;
    public float _mouthWidth, _mouthHeight;
    public float _currentHeight, _currentWidth;
    [Range(0, 1)]
    public float _similarity = 0f;

    #endregion

    #region Private members

    FacePipeline _pipeline;
    Material _material;

    #endregion

    #region standard unity methods

    void Start()
    {
        _pipeline = new FacePipeline(_resources);
        _material = new Material(_shader);
    }

    void Update()
    {
       MouthDimensions mouthDimensions = GetMouthDimensions();
       _currentHeight = mouthDimensions.height;
         _currentWidth = mouthDimensions.width;

        // calculate the similarity between the mouth height and width averages against the user's mouth
        if (_faceSet)
        {
            _similarity = (mouthDimensions.height / _mouthHeight + mouthDimensions.width / _mouthWidth) / 2;
        }
        else
        {
            if (_settingFace && mouthDimensions.height > 0.07)
            {
                _mouthHeight = mouthDimensions.height;
                _mouthWidth = mouthDimensions.width;
                Fungus.Flowchart.BroadcastFungusMessage ("smile_ok");
                _faceSet = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetScreenshot();
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
        // _mainUI.texture = _source.Texture;
        _faceUI.texture = _pipeline.CroppedFaceTexture;
        // _leftEyeUI.texture = _pipeline.CroppedLeftEyeTexture;
        // _rightEyeUI.texture = _pipeline.CroppedRightEyeTexture;
    }

    // void OnRenderObject()
    // {
    //     // Main view overlay
    //     var mv = float4x4.Translate(math.float3(-0.875f, -0.5f, 0));
    //     _material.SetBuffer("_Vertices", _pipeline.RefinedFaceVertexBuffer);
    //     _material.SetPass(1);
    //     Graphics.DrawMeshNow(_resources.faceLineTemplate, mv);

    //     // Face view
    //     // Face mesh
    //     var fF = MathUtil.ScaleOffset(0.5f, math.float2(0.125f, -0.5f));
    //     _material.SetBuffer("_Vertices", _pipeline.RefinedFaceVertexBuffer);
    //     _material.SetPass(0);
    //     Graphics.DrawMeshNow(_resources.faceMeshTemplate, fF);

    //     // Left eye
    //     var fLE = math.mul(fF, _pipeline.LeftEyeCropMatrix);
    //     _material.SetMatrix("_XForm", fLE);
    //     _material.SetBuffer("_Vertices", _pipeline.RawLeftEyeVertexBuffer);
    //     _material.SetPass(3);
    //     Graphics.DrawProceduralNow(MeshTopology.Lines, 64, 1);

    //     // Right eye
    //     var fRE = math.mul(fF, _pipeline.RightEyeCropMatrix);
    //     _material.SetMatrix("_XForm", fRE);
    //     _material.SetBuffer("_Vertices", _pipeline.RawRightEyeVertexBuffer);
    //     _material.SetPass(3);
    //     Graphics.DrawProceduralNow(MeshTopology.Lines, 64, 1);

    //     // Debug views
    //     // Face mesh
    //     var dF = MathUtil.ScaleOffset(0.5f, math.float2(0.125f, 0));
    //     _material.SetBuffer("_Vertices", _pipeline.RawFaceVertexBuffer);
    //     _material.SetPass(1);
    //     Graphics.DrawMeshNow(_resources.faceLineTemplate, dF);

    //     // Left eye
    //     var dLE = MathUtil.ScaleOffset(0.25f, math.float2(0.625f, 0.25f));
    //     _material.SetMatrix("_XForm", dLE);
    //     _material.SetBuffer("_Vertices", _pipeline.RawLeftEyeVertexBuffer);
    //     _material.SetPass(3);
    //     Graphics.DrawProceduralNow(MeshTopology.Lines, 64, 1);

    //     // Right eye
    //     var dRE = MathUtil.ScaleOffset(0.25f, math.float2(0.625f, 0f));
    //     _material.SetMatrix("_XForm", dRE);
    //     _material.SetBuffer("_Vertices", _pipeline.RawRightEyeVertexBuffer);
    //     _material.SetPass(3);
    //     Graphics.DrawProceduralNow(MeshTopology.Lines, 64, 1);
    // }

    #endregion

    #region public methods

    public MouthDimensions GetMouthDimensions()
    {
        Vector3 mouthTop = _pipeline.GetVertexPosition(13);
        Vector3 mouthBottom = _pipeline.GetVertexPosition(14);
        // get the distance between them to get if the user is smiling
        float mouthVerticalDistance = Vector3.Distance(mouthTop, mouthBottom);
        // get the mouth left and right points
        Vector3 mouthLeft = _pipeline.GetVertexPosition(61);
        Vector3 mouthRight = _pipeline.GetVertexPosition(291);
        // get the distance between them to get if the user is smiling
        float mouthHorizontalDistance = Vector3.Distance(mouthLeft, mouthRight);

        return new MouthDimensions { height = mouthVerticalDistance, width = mouthHorizontalDistance };
    }

    public void SetSmile()
    {
        _settingFace = true;
    }

    public void GetScreenshot()
    {
        // get all pixels and map them to a new texture
        Texture2D texture = new Texture2D(_pipeline.CroppedFaceTexture.width, _pipeline.CroppedFaceTexture.height);
        Texture faceTexture  = _pipeline.CroppedFaceTexture;
        // convert texture to texture2d
        RenderTexture.active = (RenderTexture)faceTexture;
        texture.ReadPixels(new Rect(0, 0, faceTexture.width, faceTexture.height), 0, 0);
        texture.Apply();
        _screenshots.Add(texture);
    }

    #endregion
}
