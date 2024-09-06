using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapSceneUtils
{
    public static readonly Vector3 ViewCenter = new Vector3(0.5f, 0.5f, 0);
    public const float SIZE_PER_CELL = 3.0f;
    public static readonly Vector3[] ViewCorner = new Vector3[4] {
        new Vector3(0.0f, 0.0f, 0),
        new Vector3(0.0f, 1.0f, 0),
        new Vector3(1.0f, 1.0f, 0),
        new Vector3(1.0f, 0.0f, 0)
    };

    public static Vector3 GetVPPointOnPlane(Camera mainCamera, Vector3 viewPortPt, in Plane floorPlane)
    {
        var dist = 0.0f;
        var ltRay = mainCamera.ViewportPointToRay(viewPortPt);
        floorPlane.Raycast(ltRay, out dist);
        return ltRay.GetPoint(dist);
    }

    public static void GetVPPointOnPlane(Camera mainCamera, Vector3 viewPortPt, in Plane floorPlane,ref Vector3 output)
    {
        var dist = 0.0f;
        var ltRay = mainCamera.ViewportPointToRay(viewPortPt);
        floorPlane.Raycast(ltRay, out dist);
        output = ltRay.GetPoint(dist);
    }

    public static void RecordOriginBound(BoxCollider collider_bg,ref Vector3[] boundPoints,ref Plane floorPlane)
    {
        var _tfm = collider_bg.transform;
        boundPoints[0] = _tfm.TransformPoint(collider_bg.center + new Vector3(-collider_bg.size.x * 0.5f, -collider_bg.size.y * 0.5f, 0));
        boundPoints[1] = _tfm.TransformPoint(collider_bg.center + new Vector3(-collider_bg.size.x * 0.5f, collider_bg.size.y * 0.5f, 0));
        boundPoints[2] = _tfm.TransformPoint(collider_bg.center + new Vector3(+collider_bg.size.x * 0.5f, collider_bg.size.y * 0.5f, 0));
        boundPoints[3] = _tfm.TransformPoint(collider_bg.center + new Vector3(+collider_bg.size.x * 0.5f, -collider_bg.size.y * 0.5f, 0));
        floorPlane = new Plane(boundPoints[0], boundPoints[2], boundPoints[1]);
    }

    public static void _RecordOriginBoundXZ(BoxCollider collider_bg, ref Vector3[] boundPoints, ref Plane floorPlane)
    {
        var _tfm = collider_bg.transform;
        boundPoints[0] = _tfm.TransformPoint(collider_bg.center + new Vector3(-collider_bg.size.x * 0.5f, 0, -collider_bg.size.z * 0.5f));
        boundPoints[1] = _tfm.TransformPoint(collider_bg.center + new Vector3(-collider_bg.size.x * 0.5f, 0, collider_bg.size.z * 0.5f));
        boundPoints[2] = _tfm.TransformPoint(collider_bg.center + new Vector3(+collider_bg.size.x * 0.5f, 0, collider_bg.size.z * 0.5f));
        boundPoints[3] = _tfm.TransformPoint(collider_bg.center + new Vector3(+collider_bg.size.x * 0.5f, 0, -collider_bg.size.z * 0.5f));
        floorPlane = new Plane(boundPoints[0], boundPoints[2], boundPoints[1]);
    }

    public static void RecordOriginBoundXZ(BoxCollider collider_bg, ref Vector3[] boundPoints, ref Plane floorPlane)
    {
        var _tfm = collider_bg.transform;
        boundPoints[0] = _tfm.TransformPoint(collider_bg.center + new Vector3(-collider_bg.size.x * 0.5f, -collider_bg.size.y * 0.5f, 0f));
        boundPoints[1] = _tfm.TransformPoint(collider_bg.center + new Vector3(-collider_bg.size.x * 0.5f, collider_bg.size.y * 0.5f, 0f));
        boundPoints[2] = _tfm.TransformPoint(collider_bg.center + new Vector3(+collider_bg.size.x * 0.5f, collider_bg.size.y * 0.5f, 0f));
        boundPoints[3] = _tfm.TransformPoint(collider_bg.center + new Vector3(+collider_bg.size.x * 0.5f, -collider_bg.size.y * 0.5f, 0f));
        floorPlane = new Plane(boundPoints[0], boundPoints[2], boundPoints[1]);
    }


    public static Rect RecordBoundXZ(BoxCollider collider_bg)
    {
        var _tfm = collider_bg.transform;
        var center = _tfm.TransformPoint(collider_bg.center);
        var left_bottom = _tfm.TransformPoint(collider_bg.center + new Vector3(-collider_bg.size.x * 0.5f, 0, -collider_bg.size.z * 0.5f));
        var size = new Vector2(Math.Abs(left_bottom.x - center.x) * 2, Math.Abs(left_bottom.z - center.z) * 2);
        Rect rect = new Rect(new Vector2(center.x, center.z) - new Vector2(size.x/2f,size.y/2f), size);
        return rect;
    }

    /// <summary>
    /// 菜单的显示区域,
    /// 在世界会遇到相对位置;
    /// </summary>
    /// <param name="building"></param> 
    /// <returns></returns>
    public static bool InBoundAndFixPos(Vector3 viewPosition, ref Rect bound,ref Vector3 fixCoord)
    {
        fixCoord = viewPosition;
        fixCoord.z = 0;
        if (viewPosition.x > bound.xMax)
        {
            if (!IsInVertical(ref bound, ref viewPosition, ref fixCoord.y))
            {

            }
            fixCoord.x = bound.xMax;
            return false;
        }
        else if (viewPosition.x < (bound.x))
        {
            if (!IsInVertical(ref bound, ref viewPosition, ref fixCoord.y))
            {

            }
            fixCoord.x = bound.x;
            return false;
        }
        else
        {
            if (!IsInVertical(ref bound, ref viewPosition, ref fixCoord.y))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 垂直方向是否满足
    /// </summary>
    /// <param name="viewPosition"></param>
    /// <param name="fixY"></param>
    /// <returns></returns>
    private static bool IsInVertical(ref Rect bound, ref Vector3 viewPosition, ref float fixY)
    {
        if (viewPosition.y < bound.y)
        {
            fixY = bound.y;
            return false;
        }
        else if (viewPosition.y > bound.yMax)
        {
            fixY = bound.yMax;
            return false;
        }
        return true;
    }

    #region Grid Hash

    /// <summary>
    /// 服务端保存位置的方式;
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int ServerHash(int x, int y)
    {
        return GetGridId(x, y);
    }

    public static Vector2Int ServerHash_Rev(int p)
    {
        var vec3 = Grid2DBy(p);
        return new Vector2Int(vec3.x, vec3.y);
    }

    public static void KeyHV(int hashKey, ref int grid_x, ref int grid_y)
    {
        var v = ServerHash_Rev(hashKey);
        grid_x = v.x;
        grid_y = v.y;
    }

    public static int PositionHash(int x, int y)
    {
        return (x << 8) + y;
    }

    public static Vector2Int PositionHash_Rev(int p)
    {
        return new Vector2Int(p >> 8, p & 0xFF);
    }
    #endregion

    #region 服务器端zone_id到客户端的zone_id的对应关系;

    private static List<Vector3Int> keyGridDict = new List<Vector3Int>(81);

    public static int GetGridId(int grid_x,int grid_y)
    {
        for (int i = 0; i < keyGridDict.Count; i++)
        {
            var vec = keyGridDict[i];
            if (grid_x == vec.x  && grid_y == vec.y)
            {
                return vec.z;
            }
        }
        var maxGrid = Math.Max(Mathf.Abs(grid_x), Mathf.Abs(grid_y));
        var maxCount = (maxGrid * 2 + 1) * (maxGrid * 2 + 1);
        int targetID = 0;
        for (int i = maxCount - 1; i >= 0; i--)
        {
            if(keyGridDict[i].z != i)
            {
                var vex = Grid2DBy(i);
                if(vex.x == grid_x && vex.y == grid_y)
                {
                    targetID = i;
                }
            }
        }
        return targetID;
    }
    /// <summary>
    /// 服务端zone_id编号
    /// 21   22   23   24   9
    /// 20   7     8   1    10
    /// 19   6     0   2    11
    /// 18   5     4   3    12
    /// 17   16   15  14    13
    /// n即为offset
    ///                     (n,n)
    /// 
    ///         (0,0)
    ///     (-1,-1)
    /// (-n,-n)
    /// </summary>
    public static Vector3Int Grid2DBy(int id)
    {
        if (keyGridDict[id].z==id)
        {
            return keyGridDict[id];
        }
        int gridX;
        int gridY;
        if (id == 0)
        {
            gridX = gridY = 0;
        }
        else
        {
            int n = (int)Math.Sqrt(id);
            n = (n - 1) >> 1;
            gridX = gridY = n + 1;
            int idx = id - (2 * n + 1) * (2 * n + 1);
            int len = 2 * n + 2;
            if (idx < len)
            {
                gridY -= idx;
            }
            else if (idx < 2 * len)
            {
                gridX -= (idx % len);
                gridY = -gridY;
            }
            else if (idx < 3 * len)
            {
                gridX = -gridX;
                gridY = idx % len - gridY;
            }
            else
            {
                gridX = (idx % len) - gridX;
            }
        }
        var tmpVec = new Vector3Int(gridX, gridY, id);
        keyGridDict[id] = tmpVec;
        return keyGridDict[id];
    }

    /// <summary>
    /// 服务端zone_id编号
    /// 21   22   23   24   9
    /// 20   7     8   1    10
    /// 19   6     0   2    11
    /// 18   5     4   3    12
    /// 17   16   15  14    13
    /// n即为offset
    ///                     (2n,2n)
    /// 
    ///         (n,n)
    ///     (1,1)
    /// (0,0)
    /// </summary>
    private static Vector3Int _Grid2DBy(int id, int offset)
    {
        int gridX;
        int gridY;
        if (id == 0)
        {
            gridX = gridY = offset;
        }
        else
        {
            int n = (int)Math.Sqrt(id);
            n = (n - 1) >> 1;
            gridX = gridY = n + 1;
            int idx = id - (2 * n + 1) * (2 * n + 1);
            int len = 2 * n + 2;
            if (idx < len)
            {
                gridY -= idx;
            }
            else if (idx < 2 * len)
            {
                gridX -= (idx % len);
                gridY = -gridY;
            }
            else if (idx < 3 * len)
            {
                gridX = -gridX;
                gridY = idx % len - gridY;
            }
            else
            {
                gridX = (idx % len) - gridX;
            }
            gridX += offset;
            gridY += offset;
        }

        return new Vector3Int(gridX, gridY, MapSceneUtils.ServerHash(gridX,gridY));
    }

    public static int GridIdBy(int gridX, int gridY, int offset,int[,] grid)
    {
        return grid[gridX, gridY];
    }
    #endregion

    #region 中心和偏移量
    /// <summary>
    /// 相对于grid的中心块;
    /// </summary>
    /// <param name="vCount"></param>
    /// <param name="hCount"></param>
    /// <param name="CELLSIZE"></param>
    /// <param name="offsetX"></param>
    /// <param name="offsetZ"></param>
    public static void MapOffset(int vCount,int hCount,int CELLSIZE,ref float offsetX,ref float offsetZ)
    {
        offsetX = -(vCount - 1) * CELLSIZE * 0.5f;
        offsetZ = -(hCount - 1) * CELLSIZE * 0.5f;
    }
    #endregion

    #region AreaZoneApi
    public static Vector3 GetWorldPtByZoneLocalCell(int zone_index, int locCellX, int locCellZ, int CELLSIZE = 16, float y = 0f)
    {
        var  vec = Grid2DBy(zone_index);
        var grid_x = vec.x;
        var grid_z = vec.y;
        var worldCell = MapLocalCellToWorld(grid_x, grid_z, locCellX, locCellZ, CELLSIZE);
        return GetWorldPtByWorldCell(worldCell.x, worldCell.y, y);
    }

    public static Vector3 GetWorldPtByLocCell(int gridX, int gridZ, int locCellX, int locCellZ, int CELLSIZE = 16, float y = 0.01f)
    {
        var worldCell = MapSceneUtils.MapLocalCellToWorld(gridX, gridZ, locCellX, locCellZ, CELLSIZE);
        return GetWorldPtByWorldCell(worldCell.x, worldCell.y, y);
    }


    /// <summary>
    /// Grid本地的坐标Cell映射到世界坐标系
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridZ"></param>
    /// <param name="locCellX"></param>
    /// <param name="locCellZ"></param>
    /// <param name="worldCellX"></param>
    /// <param name="worldCellZ"></param>
    public static Vector2Int MapLocalCellToWorld(int gridX, int gridZ, int locCellX, int locCellZ, int CELLSIZE = 16)
    {
        return new Vector2Int((CELLSIZE * gridX - CELLSIZE/2) + locCellX, (CELLSIZE * gridZ - CELLSIZE / 2) + locCellZ);
    }

    ///// <summary>
    ///// server_id 为zone_id,locX,locY移位封装的int;
    ///// CountryAreaLocationData.ServerHash_ZONE_LOC_XY_Rev(server_id)
    ///// </summary>
    ///// <param name="city_id"></param>
    ///// <param name="server_id"></param>
    ///// <param name="CELLSIZE"></param>
    ///// <returns></returns>
    //public static Vector2Int MapLocalServerHashToWorld(int city_id, int server_id, int CELLSIZE = 16)
    //{
    //    var pos = CountryAreaLocationData.ServerHash_ZONE_LOC_XY_Rev(server_id);
    //    return MapLocalHashToWorld(city_id, pos.x, pos.y, pos.z, CELLSIZE);
    //}

    /// <summary>
    /// 将服务器端的server_zone_id,x,y,映射到世界坐标;
    /// WorldXYtoZoneXY的逆运算
    /// </summary>
    /// <param name="city_id"></param>
    /// <param name="server_zone_id"></param>
    /// <param name="locCellX"></param>
    /// <param name="locCellZ"></param>
    /// <param name="CELLSIZE"></param>
    /// <returns></returns>
    public static Vector2Int MapLocalHashToWorld(int city_id, int server_zone_id, int locCellX, int locCellZ, int CELLSIZE = 16)
    {
        var zone_grid = MapSceneUtils.Grid2DBy(server_zone_id);
        return MapSceneUtils.MapLocalCellToWorld(zone_grid.x, zone_grid.y, locCellX, locCellZ);
    }

    /// <summary>
    /// 世界到本地
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridZ"></param>
    /// <param name="locCellX"></param>
    /// <param name="locCellZ"></param>
    /// <param name="CELLSIZE"></param>
    /// <returns></returns>
    public static Vector2Int MapWorldCellToLoc(int worldX, int worldZ, ref Vector2Int locZone, int CELLSIZE = 16)
    {
        var gridX = Mathf.FloorToInt( (worldX + CELLSIZE / 2) / (float)CELLSIZE);
        var gridZ = Mathf.FloorToInt((worldZ + CELLSIZE / 2) / (float)CELLSIZE);
        locZone.Set(gridX, gridZ);
        return new Vector2Int(worldX - (CELLSIZE * gridX - CELLSIZE / 2), worldZ - (CELLSIZE * gridZ - CELLSIZE / 2));
    }

    /// <summary>
    /// 根据城市id,服务器块区Id,和块区坐标,获取世界坐标..
    /// 
    /// </summary>
    /// <param name="city_id"></param>
    /// <param name="zone_server_id"></param>
    /// <param name="locPt"></param>
    /// <returns></returns>
    public static Vector2Int CityZoneLocalToWorld(int city_id, int zone_server_id, Vector2Int locPt)
    {
        var zone = MapSceneUtils.Grid2DBy(zone_server_id);
        return MapSceneUtils.MapLocalCellToWorld(zone.x, zone.y, locPt.x, locPt.y);
    }

    public static Vector3Int WorldCellToLocal(int world_x,int world_z,int CELLSIZE)
    {
        var gridX = world_x / CELLSIZE;
        var gridZ = world_z / CELLSIZE;
        return new Vector3Int(world_x - (CELLSIZE * gridX), world_z - (CELLSIZE * gridZ), MapSceneUtils.ServerHash(gridX, gridZ));
    }

    /// <summary>
    /// 服务端保存位置的方式;
    /// </summary>
    /// <returns></returns>
    public static Vector2Int LocalCellToWorld(int zone,int local_x, int local_z, int CELLSIZE)
    {
        var zone_ = MapSceneUtils.ServerHash_Rev(zone);
        return MapSceneUtils.MapLocalCellToWorld(zone_.x, zone_.y, local_x, local_z);
    }


    #region 获取地图实际位置与逻辑位置的转换
    /// <summary> @GetWorldPtByWorldCell为可逆变化</summary>
    public static void GetWorldCellByWorldPt(float worldX, float worldZ, ref int xIndex, ref int zIndex, int CELLSIZE)
    {
        xIndex = Mathf.FloorToInt(((worldX)) / SIZE_PER_CELL);
        zIndex = Mathf.FloorToInt(((worldZ)) / SIZE_PER_CELL);
    }

    /// <summary>为点的中心,这个点大小为2*2 </summary>
    public static Vector3 GetWorldPtByWorldCell(int world_x, int world_z, float y = 0f)
    {
        return new Vector3((world_x) * SIZE_PER_CELL + SIZE_PER_CELL / 2f,
            y,
            (world_z) * SIZE_PER_CELL + SIZE_PER_CELL / 2f);
    }

    #endregion

    public static void GetGridByWorldPt(float worldX, float worldZ, ref int gridX, ref int gridZ,int CELLSIZE = 16)
    {
        int xIndex = 0; int zIndex = 0;
        Vector2Int zoneVec = default(Vector2Int);
        MapSceneUtils.GetWorldCellByWorldPt(worldX, worldZ, ref xIndex, ref zIndex, CELLSIZE);
        MapSceneUtils.MapWorldCellToLoc(xIndex, zIndex, ref zoneVec, CELLSIZE);
        gridX = zoneVec.x;
        gridZ = zoneVec.y;
    }

    public static Vector3 GetWorldPtOfZoneLB(int xIndex, int zIndex,int CELLSIZE = 16)
    {
        return new Vector3((CELLSIZE * xIndex - CELLSIZE * 0.5f) * SIZE_PER_CELL , 0, (CELLSIZE * zIndex - CELLSIZE * 0.5f) * SIZE_PER_CELL);
    }

    public static Vector3 GetWorldPtOfZone(float xIndex, float zIndex, int CELLSIZE = 16)
    {
        return new Vector3((CELLSIZE * xIndex - CELLSIZE * 0.5f) * SIZE_PER_CELL, 0, (CELLSIZE * zIndex - CELLSIZE * 0.5f) * SIZE_PER_CELL);
    }
    #endregion


    /// <summary>
    ///  xIndex = Mathf.FloorToInt(((worldX + SIZE_PER_CELL / 2f)) / SIZE_PER_CELL);
    //   zIndex = Mathf.FloorToInt(((worldZ + SIZE_PER_CELL / 2f)) / SIZE_PER_CELL);
    /// </summary>
    /// <param name="worldX"></param>
    /// <param name="worldZ"></param>
    /// <param name="gridX"></param>
    /// <param name="gridZ"></param>
    /// <param name="CELLSIZE"></param>
    public static void WorldPosInGrid(float worldX, float worldZ, ref float gridX, ref float gridZ,int CELLSIZE)
    {
        //TODO---
        float xIndex = (((worldX)) / (float)SIZE_PER_CELL);
        float zIndex = (((worldZ)) / (float)SIZE_PER_CELL);
        gridX = ((xIndex + CELLSIZE / 2f) / (float)CELLSIZE);
        gridZ = ((zIndex + CELLSIZE / 2f) / (float)CELLSIZE);
    }

    #region 线段与Grid的相交

    /// <summary>
    /// 求出线段..
    /// xy.步进个1,然后做线段交点;,直到两个方向都超出了end节点;
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="intersects"></param>
    public static void CaclLineGrid(ref Vector3 start, ref Vector3 end, int CELLSIZE,System.Action<bool, float, float> intersectFunc)
    {
        var startXIndex = 0.0f;
        var startZIndex = 0.0f;
        var endXIndex = 0.0f;
        var endZIndex = 0.0f;
        WorldPosInGrid(start.x, start.z, ref startXIndex, ref startZIndex, CELLSIZE);
        WorldPosInGrid(end.x, end.z, ref endXIndex, ref endZIndex, CELLSIZE);
        DDLine(startXIndex, startZIndex, endXIndex, endZIndex, intersectFunc);
    }

    public static void DDLine(float startXIndex, float startZIndex, float endXIndex, float endZIndex, System.Action<bool, float, float> intersectFunc)
    {
        var dx = endXIndex - startXIndex;
        var dz = endZIndex - startZIndex;
        int ddX = (int)Mathf.Sign(dx);
        int ddZ = (int)Mathf.Sign(dz);

        var maxCount = endXIndex > startXIndex ? Mathf.CeilToInt(endXIndex) - Mathf.FloorToInt(startXIndex) : Mathf.CeilToInt(startXIndex) - Mathf.FloorToInt(endXIndex);
        for (int i = 0; i < maxCount; i++)
        {
            int fixX = Mathf.FloorToInt(startXIndex + i * ddX);
            // && fixX > 0
            if ((startXIndex - fixX) * (fixX - endXIndex) > 0)
            {
                var destPtZ = startZIndex - ((startXIndex - fixX) * dz / dx);
                intersectFunc(true, fixX, destPtZ);
            }
        }

        maxCount = endZIndex > startZIndex ? Mathf.CeilToInt(endZIndex) - Mathf.FloorToInt(startZIndex) : Mathf.CeilToInt(startZIndex) - Mathf.FloorToInt(endZIndex);
        for (int j = 0; j < maxCount; j++)
        {
            int fixZ = Mathf.FloorToInt(startZIndex + j * ddZ);
            // && fixZ > 0
            if ((startZIndex - fixZ) * (fixZ - endZIndex) > 0)
            {
                var destPtX = (startXIndex - ((startZIndex - fixZ) * dx / dz));
                intersectFunc(false, destPtX, fixZ);
            }
        }
    }

    #endregion




    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputData"></param>
    /// <param name="border"></param>
    /// <returns></returns>
    public static bool GetBorderData(Vector3[] screenPts, Vector3 centerCamera, Vector3 inputData, ref Vector3 border)
    {
        float destX = 0, destZ = 0;
        bool crossed = false;
        if (get_line_intersection(screenPts[0].x, screenPts[0].z, screenPts[1].x, screenPts[1].z
            , centerCamera.x, centerCamera.z, inputData.x, inputData.z, ref destX, ref destZ))
        {
            crossed = true;
        }
        else if (get_line_intersection(screenPts[1].x, screenPts[1].z, screenPts[2].x, screenPts[2].z
            , centerCamera.x, centerCamera.z, inputData.x, inputData.z, ref destX, ref destZ))
        {
            crossed = true;
        }
        else if (get_line_intersection(screenPts[2].x, screenPts[2].z, screenPts[3].x, screenPts[3].z
           , centerCamera.x, centerCamera.z, inputData.x, inputData.z, ref destX, ref destZ))
        {
            crossed = true;
        }
        else if (get_line_intersection(screenPts[3].x, screenPts[3].z, screenPts[0].x, screenPts[0].z
           , centerCamera.x, centerCamera.z, inputData.x, inputData.z, ref destX, ref destZ))
        {
            crossed = true;
        }
        border.x = destX;
        border.z = destZ;
        return crossed;
    }

    private static bool get_line_intersection(float p0_x, float p0_y, float p1_x, float p1_y,
    float p2_x, float p2_y, float p3_x, float p3_y, ref float i_x, ref float i_y)
    {
        float s1_x, s1_y, s2_x, s2_y;
        s1_x = p1_x - p0_x; s1_y = p1_y - p0_y;
        s2_x = p3_x - p2_x; s2_y = p3_y - p2_y;

        float s, t;
        s = (-s1_y * (p0_x - p2_x) + s1_x * (p0_y - p2_y)) / (-s2_x * s1_y + s1_x * s2_y);
        t = (s2_x * (p0_y - p2_y) - s2_y * (p0_x - p2_x)) / (-s2_x * s1_y + s1_x * s2_y);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        {
            // Collision detected
            i_x = p0_x + (t * s1_x);
            i_y = p0_y + (t * s1_y);
            return true;
        }
        return false; // No collision
    }

}
