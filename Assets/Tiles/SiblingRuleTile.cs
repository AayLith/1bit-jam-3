using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(menuName = "1bit/siblingRuleTile")]
public class SiblingRuleTile : RuleTile<SiblingRuleTile.Neighbor> {

    public enum TileType
    {
        Hill,
        Cliff,
    }
    public TileType[] matchableTileTypes;
    public TileType myTileType;
    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Null = 3;
        public const int NotNull = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase other) {
        if (other is RuleOverrideTile)
            other = (other as RuleOverrideTile).m_InstanceTile;

        switch (neighbor) {
            case Neighbor.This:
                return OtherTileTypeMatchable(other);
            case  Neighbor.NotThis:
                return !OtherTileTypeMatchable(other);
            case Neighbor.NotNull: return other != null;
        }
        return base.RuleMatch(neighbor, other);
    }

    private bool OtherTileTypeMatchable(TileBase other)
    {
        return other is SiblingRuleTile &&
        (other as SiblingRuleTile).matchableTileTypes.Contains(myTileType);
    }
}