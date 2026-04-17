using UnityEngine;
using UnityEngine.Tilemaps;

namespace Metroidvania.Session5
{
    [CreateAssetMenu(fileName = "LevelLayoutData", menuName = "Metroidvania/Session5/LevelLayoutData")]
    public class LevelLayoutData : ScriptableObject
    {
        [Header("Tile References")]
        public TileBase GroundTile;

        [Tooltip("Assign a RuleTile asset from 2D Tilemap Extras for auto-tiling walls.")]
        public TileBase WallRuleTile;

        [Header("Simple Layout")]
        public Vector2Int[] GroundCells;
        public Vector2Int[] WallCells;
    }
}
