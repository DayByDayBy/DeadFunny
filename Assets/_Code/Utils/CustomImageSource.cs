using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class CustomImageSource : MonoBehaviour
{
    #region Public property

    public Vector2Int OutputResolution => _outputResolution;

    #endregion

    #region Editable attributes
   
    // Webcam options
    [SerializeField] string _webcamName = "";
    [SerializeField] Vector2Int _webcamResolution = new Vector2Int(1920, 1080);
    [SerializeField] int _webcamFrameRate = 30;

    // Output options
    [SerializeField] Vector2Int _outputResolution = new Vector2Int(1920, 1080);

    #endregion

    #region Package asset reference

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Private members

    WebCamTexture _webcam;
    RenderTexture _buffer;

    public RenderTexture OutputBuffer
      => _buffer;

    IEnumerator Start()
    {
        FindWebCams();

        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.Log("webcam found");
        }
        else
        {
            Debug.Log("webcam not found");
        }

        FindMicrophones();

        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Debug.Log("Microphone found");
        }
        else
        {
            Debug.Log("Microphone not found");
        }
    }

    void FindWebCams()
    {
        foreach (var device in WebCamTexture.devices)
        {
            Debug.Log("Name: " + device.name);
        }
    }

    void FindMicrophones()
    {
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
    }

    // Blit a texture into the output buffer with aspect ratio compensation.
    void Blit(Texture source, bool vflip = false)
    {
        if (source == null) return;

        var aspect1 = (float)source.width / source.height;
        var aspect2 = (float)OutputBuffer.width / OutputBuffer.height;

        var scale = new Vector2(aspect2 / aspect1, aspect1 / aspect2);
        scale = Vector2.Min(Vector2.one, scale);
        if (vflip) scale.y *= -1;

        var offset = (Vector2.one - scale) / 2;

        Graphics.Blit(source, OutputBuffer, scale, offset);
    }

    #endregion

    #region MonoBehaviour implementation

    public void Init()
    {
        if (_webcam == null) ResetCamera();
        _webcam.Play();

        if (_buffer != null) Destroy(_buffer);
        
        _buffer = new RenderTexture
            (_outputResolution.x, _outputResolution.y, 0);
    }

    public void ResetCamera()
    {
        print("Resetting Camera");
        // select the first webcam
        _webcamName = WebCamTexture.devices[0].name;

        _webcam = new WebCamTexture
            (_webcamName,
            _webcamResolution.x, _webcamResolution.y, _webcamFrameRate);
    }

    void OnDestroy()
    {
        if (_webcam != null) Destroy(_webcam);
        if (_buffer != null) Destroy(_buffer);
    }

    void Update()
    {
        if (!_webcam) return;
        Blit(_webcam, _webcam.videoVerticallyMirrored);
    }

    #endregion
}