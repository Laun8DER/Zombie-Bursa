using UnityEngine;

public class GameSettings : MonoBehaviour
{
    void Awake()
    {
#if UNITY_ANDROID
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
#endif
    }
}