using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MotionVectorEffect : MonoBehaviour
{
    public Shader shader;
    private Material _material;

    [SerializeField]
    private RenderTexture _ping;
    [SerializeField]
    private RenderTexture _pong;

    private Camera _camera;

    [Header("Effect Settings")]
    [SerializeField]
    private float _motionMultiplier = 1.0f;
    [SerializeField]
    private float _mix = 0.1f;
    [SerializeField]
    private Texture _texture;

    private RenderTexture _currentRT;

    private bool CheckResources()
    {
        if (shader == null || !shader.isSupported)
        {
            enabled = false;
            return false;
        }

        if (_material == null)
        {
            _material = new Material(shader);
        }

        _camera = GetComponent<Camera>();

        _camera.depthTextureMode = DepthTextureMode.MotionVectors | DepthTextureMode.DepthNormals | DepthTextureMode.Depth;

        return true;
    }

    private void Start()
    {
        _material = new Material(shader);

//        _ping = new RenderTexture(Screen.height, Screen.width, 24, RenderTextureFormat.ARGB32);
//        _pong = new RenderTexture(Screen.height, Screen.width, 24, RenderTextureFormat.ARGB32);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (CheckResources() == false) return;

        _material.SetFloat("_MotionMultiplier", _motionMultiplier);
        _material.SetFloat("_Mix", _mix);

        _material.SetTexture("_FrameBuffer", _currentRT);
        _material.SetTexture("_ColorInputTexture", _texture);

        _currentRT = _currentRT == _pong ? _ping : _pong;

        Graphics.Blit(src, _currentRT, _material, 1);

        Graphics.Blit(_currentRT, dest, _material, 0);
    }
}
