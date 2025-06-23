using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Votanic.vXR.vCast;

public class VXRHDRP : MonoBehaviour
{
    public bool addLightsOnStart = true;
    public List<Light> lights = new List<Light>();
    List<HDAdditionalCameraData> cameraData = new List<HDAdditionalCameraData>();

    private void Awake()
    {
        Votanic.vXR.vCast.Core.Quality.OnAnisotropicFilteringChanged += OnAnisotropicFilteringChanged;
        Votanic.vXR.vCast.Core.Quality.OnAntiAliasingChanged += OnAntiAliasingChanged;
        Votanic.vXR.vCast.Core.Quality.OnShadowQualityChanged += OnShadowQualityChanged;
        Votanic.vXR.vCast.Core.Quality.OnShadowResolutionChanged += OnShadowResolutionChanged;
        if (addLightsOnStart && lights.Count == 0)
        {
            lights.AddRange(FindObjectsOfType<Light>());
        }
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;
        for (int i = 0; i < vCast.GetCameras().Count; i++)
        {
            Camera[] uCams = vCast.GetCamera(i).gameObject.GetComponentsInChildren<Camera>();
            for (int c = 0; c < uCams.Length; c++)
            {
                HDAdditionalCameraData data = uCams[c].gameObject.GetComponent<HDAdditionalCameraData>();
                if (!data) data = uCams[c].gameObject.AddComponent<HDAdditionalCameraData>();
                data.xrRendering = vCast.environment == Votanic.vXR.vCast.Core.SystemType.HMD;
                cameraData.Add(data);
            }
        }
        OnAntiAliasingChanged((byte)QualitySettings.antiAliasing);
        var wands = FindObjectsOfType<vCast_Wand>();
        for (int i = 0; i < wands.Length; i++)
        {
            wands[i].OnRenderQueueChanged += OnRenderQueneChanged;
        }
        var faders = FindObjectsOfType<vCast_Fader>();
        for (int i = 0; i < faders.Length; i++)
        {
            faders[i].OnRenderQueueChanged += OnRenderQueneChanged;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnRenderQueneChanged(vCast_Wand wand, int renderQueue)
    {
        //Debug.Log(wand.name + " " + renderQueue);
    }

    void OnRenderQueneChanged(vCast_Fader fader, int renderQueue)
    {
        //Debug.Log(fader.name + " " + renderQueue);
    }
    void OnAnisotropicFilteringChanged(AnisotropicFiltering anisotropicFiltering)
    {
        //Debug.Log(anisotropicFiltering);
    }
    void OnAntiAliasingChanged(byte antiAliasing)
    {
        //Debug.Log(antiAliasing);
        for (int i = 0; i < cameraData.Count; i++)
        {
            switch (antiAliasing)
            {
                case 2: cameraData[i].antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing; break;
                case 4: cameraData[i].antialiasing = HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing; break;
                case 8: cameraData[i].antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing; break;
                default: cameraData[i].antialiasing = HDAdditionalCameraData.AntialiasingMode.None; break;
            }
        }
    }
    void OnShadowQualityChanged(ShadowQuality shadowQuality)
    {
        //Debug.Log(shadowQuality);
        for (int i = 0; i < lights.Count; i++)
        {
            lights[i].GetComponent<HDAdditionalLightData>().EnableShadows(shadowQuality != ShadowQuality.Disable);
        }
    }
    void OnShadowResolutionChanged(ShadowResolution shadowResolution)
    {
        //Debug.Log(shadowResolution);
        for (int i = 0; i < lights.Count; i++)
        {
            switch (shadowResolution)
            {
                case ShadowResolution.Low:
                    lights[i].GetComponent<HDAdditionalLightData>().SetShadowResolutionLevel(0);
                    break;
                case ShadowResolution.Medium:
                    lights[i].GetComponent<HDAdditionalLightData>().SetShadowResolutionLevel(1);
                    break;
                case ShadowResolution.High:
                    lights[i].GetComponent<HDAdditionalLightData>().SetShadowResolutionLevel(2);
                    break;
                case ShadowResolution.VeryHigh:
                    lights[i].GetComponent<HDAdditionalLightData>().SetShadowResolutionLevel(3);
                    break;
            }
        }
    }
}
