using System;
using UnityEngine;
public static class Util {    
    #region Swizzle
    public static Vector2 xx (this Vector3 v) { return new Vector2(v.x, v.x); }
    public static Vector2 xy (this Vector3 v) { return new Vector2(v.x, v.y); }
    public static Vector2 yx (this Vector3 v) { return new Vector2(v.y, v.x); }
    public static Vector2 yy (this Vector3 v) { return new Vector2(v.y, v.y); }
    public static Vector2 zx (this Vector3 v) { return new Vector2(v.z, v.x); }
    public static Vector2 xz (this Vector3 v) { return new Vector2(v.x, v.z); }
    public static Vector2 zy (this Vector3 v) { return new Vector2(v.z, v.y); }
    public static Vector2 yz (this Vector3 v) { return new Vector2(v.y, v.z); }
    public static Vector2 zz (this Vector3 v) { return new Vector2(v.z, v.z); }
    public static Vector2 xx (this Vector2 v) { return new Vector2(v.x, v.x); }
    public static Vector2 xy (this Vector2 v) { return new Vector2(v.x, v.y); }
    public static Vector2 yx (this Vector2 v) { return new Vector2(v.y, v.x); }
    public static Vector2 yy (this Vector2 v) { return new Vector2(v.y, v.y); }


    public static Vector3 xxx (this Vector2 v) { return new Vector3(v.x, v.x, v.x); }
    public static Vector3 xxy (this Vector2 v) { return new Vector3(v.x, v.x, v.y); }
    public static Vector3 xyx (this Vector2 v) { return new Vector3(v.x, v.y, v.x); }
    public static Vector3 xyy (this Vector2 v) { return new Vector3(v.x, v.y, v.y); }
    public static Vector3 yxx (this Vector2 v) { return new Vector3(v.y, v.x, v.x); }
    public static Vector3 yxy (this Vector2 v) { return new Vector3(v.y, v.x, v.y); }
    public static Vector3 yyx (this Vector2 v) { return new Vector3(v.y, v.y, v.x); }
    public static Vector3 yyy (this Vector2 v) { return new Vector3(v.y, v.y, v.y); }


    public static Vector3 xxx (this Vector3 v) { return new Vector3(v.x, v.x, v.x); }
    public static Vector3 xxy (this Vector3 v) { return new Vector3(v.x, v.x, v.y); }
    public static Vector3 xxz (this Vector3 v) { return new Vector3(v.x, v.x, v.z); }
    public static Vector3 xyx (this Vector3 v) { return new Vector3(v.x, v.y, v.x); }
    public static Vector3 xyy (this Vector3 v) { return new Vector3(v.x, v.y, v.y); }
    public static Vector3 xyz (this Vector3 v) { return new Vector3(v.x, v.y, v.z); }
    public static Vector3 xzx (this Vector3 v) { return new Vector3(v.x, v.z, v.x); }
    public static Vector3 xzy (this Vector3 v) { return new Vector3(v.x, v.z, v.y); }
    public static Vector3 xzz (this Vector3 v) { return new Vector3(v.x, v.z, v.z); }
    public static Vector3 yxx (this Vector3 v) { return new Vector3(v.y, v.x, v.x); }
    public static Vector3 yxy (this Vector3 v) { return new Vector3(v.y, v.x, v.y); }
    public static Vector3 yxz (this Vector3 v) { return new Vector3(v.y, v.x, v.z); }
    public static Vector3 yyx (this Vector3 v) { return new Vector3(v.y, v.y, v.x); }
    public static Vector3 yyy (this Vector3 v) { return new Vector3(v.y, v.y, v.y); }
    public static Vector3 yyz (this Vector3 v) { return new Vector3(v.y, v.y, v.z); }
    public static Vector3 yzx (this Vector3 v) { return new Vector3(v.y, v.z, v.x); }
    public static Vector3 yzy (this Vector3 v) { return new Vector3(v.y, v.z, v.y); }
    public static Vector3 yzz (this Vector3 v) { return new Vector3(v.y, v.z, v.z); }
    public static Vector3 zxx (this Vector3 v) { return new Vector3(v.z, v.x, v.x); }
    public static Vector3 zxy (this Vector3 v) { return new Vector3(v.z, v.x, v.y); }
    public static Vector3 zxz (this Vector3 v) { return new Vector3(v.z, v.x, v.z); }
    public static Vector3 zyx (this Vector3 v) { return new Vector3(v.z, v.y, v.x); }
    public static Vector3 zyy (this Vector3 v) { return new Vector3(v.z, v.y, v.y); }
    public static Vector3 zyz (this Vector3 v) { return new Vector3(v.z, v.y, v.z); }
    public static Vector3 zzx (this Vector3 v) { return new Vector3(v.z, v.z, v.x); }
    public static Vector3 zzy (this Vector3 v) { return new Vector3(v.z, v.z, v.y); }
    public static Vector3 zzz (this Vector3 v) { return new Vector3(v.z, v.z, v.z); }
    #endregion Swizzle

    public static Vector3 WithZ(this Vector2 v, float z) { return new Vector3(v.x, v.y, z); }
    public static Vector3Int WithZ(this Vector2Int v, int z) { return new Vector3Int(v.x, v.y, z); }
    
    public static Vector3 WithZ(this Vector3 v, float z) { return new Vector3(v.x, v.y, z); }
    public static Vector3Int WithZ(this Vector3Int v, int z) { return new Vector3Int(v.x, v.y, z); }
    
    public static Vector3Int ToInt(this Vector3 v) { return new Vector3Int((int)v.x, (int)v.y, (int)v.z); }
    public static Vector2Int ToInt(this Vector2 v) { return new Vector2Int((int)v.x, (int)v.y); }

    public static Vector2Int RoundToInt(this Vector2 v) { return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y)); }
    public static Vector3Int RoundToInt(this Vector3 v) { return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z)); }

    public static Vector3 ToFloat(this Vector3Int v) { return new Vector3(v.x, v.y, v.z); }
    public static Vector2 ToFloat(this Vector2Int v) { return new Vector2(v.x, v.y); }

    public static Vector3 MouseWorldPosition(Camera cam) => cam.ScreenToWorldPoint(Input.mousePosition).WithZ(0);
    public static Vector3Int MouseTilePosition(Camera cam) => MouseWorldPosition(cam).RoundToInt();

    
}