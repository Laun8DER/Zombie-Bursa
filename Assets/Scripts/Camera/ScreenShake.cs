using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private static ScreenShake instance;

    public static ScreenShake Instance
    {
        get
        {
            if (instance == null)
            {
                CreateInstance();
            }

            return instance;
        }
    }

    private ScreenShakeExtension currentExtension;
    private CinemachineVirtualCameraBase currentCamera;
    private Coroutine shakeRoutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        CreateInstance();
    }

    private static void CreateInstance()
    {
        if (instance != null)
        {
            return;
        }

        GameObject go = new GameObject("ScreenShake");
        instance = go.AddComponent<ScreenShake>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Shake(float duration, float amplitude, float frequency)
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }

        ClearCurrentShake();
        shakeRoutine = StartCoroutine(ShakeRoutine(duration, amplitude, frequency));
    }

    private IEnumerator ShakeRoutine(float duration, float amplitude, float frequency)
    {
        float elapsed = 0f;
        float seed = Random.Range(0f, 1000f);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            if (EnsureExtension())
            {
                float strength = amplitude * (1f - Mathf.Clamp01(elapsed / duration));
                float sampleTime = Time.unscaledTime * frequency;
                Vector3 offset = new Vector3(
                    (Mathf.PerlinNoise(seed, sampleTime) - 0.5f) * 2f,
                    (Mathf.PerlinNoise(seed + 1f, sampleTime) - 0.5f) * 2f,
                    0f) * strength;

                currentExtension.ShakeOffset = offset;
            }

            yield return null;
        }

        ClearCurrentShake();
        shakeRoutine = null;
    }

    private bool EnsureExtension()
    {
        CinemachineVirtualCameraBase liveCamera = GetLiveCamera();
        if (liveCamera == null)
        {
            return false;
        }

        if (liveCamera == currentCamera && currentExtension != null)
        {
            return true;
        }

        ClearCurrentShake();
        currentCamera = liveCamera;
        currentExtension = liveCamera.GetComponent<ScreenShakeExtension>();

        if (currentExtension == null)
        {
            currentExtension = liveCamera.gameObject.AddComponent<ScreenShakeExtension>();
        }

        return currentExtension != null;
    }

    private CinemachineVirtualCameraBase GetLiveCamera()
    {
        for (int i = 0; i < CinemachineBrain.ActiveBrainCount; i++)
        {
            CinemachineBrain brain = CinemachineBrain.GetActiveBrain(i);
            if (brain == null)
            {
                continue;
            }

            CinemachineVirtualCameraBase liveCamera = brain.ActiveVirtualCamera as CinemachineVirtualCameraBase;
            if (liveCamera != null)
            {
                return liveCamera;
            }
        }

        return FindAnyObjectByType<CinemachineCamera>();
    }

    private void ClearCurrentShake()
    {
        if (currentExtension != null)
        {
            currentExtension.ShakeOffset = Vector3.zero;
        }
    }
}

public class ScreenShakeExtension : CinemachineExtension
{
    public Vector3 ShakeOffset { get; set; }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Finalize || ShakeOffset == Vector3.zero)
        {
            return;
        }

        state.PositionCorrection += state.GetCorrectedOrientation() * ShakeOffset;
    }
}
