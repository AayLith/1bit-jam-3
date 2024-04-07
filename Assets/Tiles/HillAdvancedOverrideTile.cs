using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "1bit/Better Rule Tile")]
public class HillAdvancedOverrideTile : RuleTile {
    public bool alwaysConnect;
    public TileBase[] tilesToConnectTo;
    public bool checkSelf;

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Any = 3;
        public const int Specified = 4;
        public const int Nothing = 5;

    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        
        if (tile is RuleOverrideTile ruleOverrideTile)
        {
            tile = ruleOverrideTile.m_InstanceTile;
        }
        
        switch (neighbor) {
            case Neighbor.This: return CheckThis(tile);
            case Neighbor.NotThis: return CheckNotThis(tile);
            case Neighbor.Any: return CheckAny(tile);
            case Neighbor.Specified: return CheckSpecified(tile);
            case Neighbor.Nothing: return CheckNothing(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }

    private bool CheckThis(TileBase tile)
    {
        if(!alwaysConnect) return tile == this;
        return tilesToConnectTo.Contains(tile) || tile == this;
    }

    private bool CheckNotThis(TileBase tile)
    {
        return tile != this;
    }

    private bool CheckAny(TileBase tile)
    {
        if(checkSelf) return tile != null;
        return tile != null && tile != this;
    }

    private bool CheckSpecified(TileBase tile)
    {
        return tilesToConnectTo.Contains(tile);
    }

    private bool CheckNothing(TileBase tile)
    {
        return tile == null;
    }

}