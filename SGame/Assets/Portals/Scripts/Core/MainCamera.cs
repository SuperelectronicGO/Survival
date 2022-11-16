using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class MainCamera : MonoBehaviour
{

    Portal[] portals;

    void Awake()
    {
        portals = FindObjectsOfType<Portal>();
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;

    }
    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }


    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {

        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PrePortalRender();
        }
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].Render(context);
        }

        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PostPortalRender();
        }

    }

}
/*
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class MainCamera : MonoBehaviour
{

    Portal[] portals;

    void Awake()
    {
        portals = FindObjectsOfType<Portal>();
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;

    }
    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }


    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {

        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PrePortalRender();
        }
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].Render(context);
        }

        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].PostPortalRender();
        }

    }

}
*/