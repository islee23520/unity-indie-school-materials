using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Metroidvania.Session13
{
    public class OptimizationProfilingPlaybook : MonoBehaviour
    {
        [Header("Atlas")]
        [SerializeField] private string _atlasResourcePath = "UI_Atlas";
        [SerializeField] private string _spriteName = "button_start";
        [SerializeField] private Image _targetImage;

        [Header("Render Budget")]
        [SerializeField] private int _targetDrawCallBudget = 20;

        private static readonly ProfilerMarker ApplyAtlasMarker = new ProfilerMarker("Session13.ApplySpriteAtlas");

        private void Start()
        {
            ApplySpriteFromAtlas();
            LogProfilingChecklist();
        }

        [ContextMenu("Apply Sprite Atlas Sprite")]
        public void ApplySpriteFromAtlas()
        {
            using (ApplyAtlasMarker.Auto())
            {
                SpriteAtlas atlas = Resources.Load<SpriteAtlas>(_atlasResourcePath);
                if (atlas == null || _targetImage == null)
                {
                    Debug.LogWarning("Sprite Atlas sample skipped. Atlas or target image is missing.");
                    return;
                }

                Sprite sprite = atlas.GetSprite(_spriteName);
                _targetImage.sprite = sprite;
            }
        }

        [ContextMenu("Log Optimization Checklist")]
        public void LogProfilingChecklist()
        {
            Debug.Log($"Target Draw Call budget: {_targetDrawCallBudget}");
            Debug.Log("Open Profiler (Window > Analysis > Profiler) and capture baseline.");
            Debug.Log("Open Frame Debugger (Window > Analysis > Frame Debugger) and inspect batching breaks.");
            Debug.Log("If Draw Call count exceeds budget, merge textures into Sprite Atlas and reuse materials.");
        }
    }
}
