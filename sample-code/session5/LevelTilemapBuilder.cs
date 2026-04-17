using UnityEngine;
using UnityEngine.Tilemaps;

namespace Metroidvania.Session5
{
    public class LevelTilemapBuilder : MonoBehaviour
    {
        [SerializeField] private Tilemap _groundTilemap;
        [SerializeField] private Tilemap _wallTilemap;
        [SerializeField] private LevelLayoutData _layoutData;

        private void Start()
        {
            Build();
        }

        [ContextMenu("Build Tilemap")]
        public void Build()
        {
            if (_layoutData == null || _groundTilemap == null || _wallTilemap == null)
            {
                return;
            }

            _groundTilemap.ClearAllTiles();
            _wallTilemap.ClearAllTiles();

            for (int i = 0; i < _layoutData.GroundCells.Length; i++)
            {
                Vector2Int cell = _layoutData.GroundCells[i];
                _groundTilemap.SetTile(new Vector3Int(cell.x, cell.y, 0), _layoutData.GroundTile);
            }

            for (int i = 0; i < _layoutData.WallCells.Length; i++)
            {
                Vector2Int cell = _layoutData.WallCells[i];
                _wallTilemap.SetTile(new Vector3Int(cell.x, cell.y, 0), _layoutData.WallRuleTile);
            }
        }
    }
}
