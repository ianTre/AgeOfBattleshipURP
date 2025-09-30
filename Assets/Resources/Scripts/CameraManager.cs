using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    Camera deployCamera;
    [SerializeField]
    Camera radarCamera;
    [SerializeField]
    Camera radarSmallRotatorCamera;
    [SerializeField]
    Camera CinemPlayerMapCamera;
    [SerializeField]
    Camera camera5;
    [SerializeField]
    Camera camera6;
    List<Camera> allCameras;
    public static CameraManager instance;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        allCameras = new List<Camera>();
        if (deployCamera != null)
            allCameras.Add(deployCamera);
        if (radarCamera != null)
            allCameras.Add(radarCamera);
        if (radarSmallRotatorCamera != null)
            allCameras.Add(radarSmallRotatorCamera);
        if (CinemPlayerMapCamera != null)
            allCameras.Add(CinemPlayerMapCamera);
        if (camera5 != null)
            allCameras.Add(camera5);
        if (camera6 != null)
            allCameras.Add(camera6);
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleZoom(float zoomLevel)
    {
        var activeCameras = GetActiveCamera();
        if(activeCameras == null)
            return;

        if(activeCameras.Contains(deployCamera))
        {
            deployCamera.transform.GetComponentInParent<CameraController>().Zoom(zoomLevel);
        }

        if(activeCameras.Contains(radarCamera))
        {
            radarCamera.transform.GetComponentInParent<RadarCameraController>().Zoom(zoomLevel);
        }
    }

    public void ChangeToDeployStage()
    {
        List<Camera> cameraToActivate = new List<Camera>
        {
            deployCamera,
        };
        ActivateCamera(cameraToActivate);
    }

    public void ChangeToPlayerAtacckIARadar()
    {
        List<Camera> cameraToActivate = new List<Camera>
        {
            radarCamera,
            radarSmallRotatorCamera
        };
        ActivateCamera(cameraToActivate);
    }

    public void ChangeToPlayerAtacckIACinematick()
    {
        List<Camera> cameraToActivate = new List<Camera>
        {
            CinemPlayerMapCamera,
        };
        ActivateCamera(cameraToActivate);
    }

    public void TurnOffAllCameras()
    {
        allCameras.ForEach(cam => cam.gameObject.SetActive(false));
    }

    public void ActivateCamera(List<Camera> camerasToActivate) 
    {
        allCameras.ForEach(cam => cam.gameObject.SetActive(false));
        camerasToActivate.ForEach(cam => cam.gameObject.SetActive(true));
        Camera oldMainCamera = allCameras.Find(cam => cam.tag == "MainCamera");
        if (oldMainCamera != null)
            oldMainCamera.tag = "Untagged";
        camerasToActivate.First().tag = "MainCamera";
        InputController.instance.UpdateCameraReference();
    }

    private List<Camera> GetActiveCamera()
    {
        return allCameras.Where(cam => cam.gameObject.activeSelf).ToList();
    }


}
