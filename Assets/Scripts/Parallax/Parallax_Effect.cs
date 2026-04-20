using Unity.Cinemachine;
using UnityEngine;

public class ParallaxCinemachine : MonoBehaviour
{
    [SerializeField] private float parallaxSpeed = 0.1f;
    private Transform cam;
    private Vector3 lastCamPos;

    void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }

    void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
    }


    private void OnCameraUpdated(CinemachineBrain brain)
    {
        Vector3 delta = cam.position - lastCamPos;


        transform.position += new Vector3(delta.x * parallaxSpeed, 0f, 0);

        lastCamPos = cam.position;
    }
}