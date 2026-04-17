using NavMeshPlus.Components;
using UnityEngine;

namespace Metroidvania.Session6
{
    public class NavMeshSurfaceBootstrap : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface _surface;

        private void Start()
        {
            if (_surface != null)
            {
                _surface.BuildNavMesh();
            }
        }

        [ContextMenu("Rebuild NavMesh")]
        public void RebuildNavMesh()
        {
            if (_surface == null)
            {
                return;
            }

            _surface.UpdateNavMesh(_surface.navMeshData);
        }
    }
}
