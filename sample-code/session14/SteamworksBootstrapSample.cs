using UnityEngine;

#if STEAMWORKS_NET
using Steamworks;
#endif

namespace Metroidvania.Session14
{
    public class SteamworksBootstrapSample : MonoBehaviour
    {
        [SerializeField] private bool _logPersonaOnStart = true;

        private void Start()
        {
            LogSteamReleaseContext();
            InitializeSteamworks();
        }

        private void InitializeSteamworks()
        {
#if STEAMWORKS_NET
            if (SteamManager.Initialized && _logPersonaOnStart)
            {
                string playerName = SteamFriends.GetPersonaName();
                Debug.Log($"Steamworks.NET initialized. Steam user: {playerName}");
            }
            else
            {
                Debug.LogWarning("Steamworks.NET is present but Steam client is not initialized.");
            }
#else
            Debug.LogWarning("Steamworks.NET package symbol not enabled. This is a teaching-only fallback path.");
#endif
        }

        private void LogSteamReleaseContext()
        {
            Debug.Log("Session 14 focus: Steam build pipeline, CLI automation, and CI/CD.");
            Debug.Log("Primary release target is Steam (Windows/macOS/Linux depots), not mobile-only packaging.");
        }
    }
}
