using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum PathType {
    MonsterMovePath,
    UnitSetPath,
    None,
}

public class Path
{
    public Vector2Int point;
    public Vector2 position;
    public PathType pathType;
    public Path(Vector2Int maxXY, Vector2 position, PathType pathType)
    {
        this.point = maxXY;
        this.position = position; 
        this.pathType = pathType;
    }
}

public class MapManager : MonoBehaviour
{
    public const int mapX = 8;
    public const int mapY = 9;
    public const int middleY = 5;

    public const float left = -1.95f;
    public const float right = 1.9f;
    public const float up = 2.5f;
    public const float middle = 0.5f;
    public const float down = -1.5f;


    public Path[] map = new Path[mapX * mapY];

    public void Init()
    {
        SetMapPath(0, mapX, 0, mapY, PathType.MonsterMovePath);
        SetMapPath(1, mapX - 1, 1, middleY - 1, PathType.UnitSetPath);
        SetMapPath(1, mapX - 1, middleY, mapY - 1, PathType.UnitSetPath);

        FindMapPathDFS();
        Sort();
    }

    private void SetMapPath(int xMin, int xMax, int yMin, int yMax, PathType pathType)
    {
        var horizontalTerm = (float)(right - left) / (mapX - 1);
        var verticalTerm = (float)(up - down) / (mapY - 1);

        var xPos = 0f;
        var yPos = 0f;

        for (int y = yMin; y < yMax; y++)
        {
            yPos = down + verticalTerm * y;
            for (int x = xMin; x < xMax; x++)
            {
                xPos = left + horizontalTerm * x;
                map[y * mapX + x] = new Path(new Vector2Int(x, y), new Vector2(xPos, yPos), pathType);
            }
        }
    }

    public Path GetPath(Vector2Int point)
    {
        for(int i = 0; i < map.Length; i++)
            if(map[i].point == point)
                return map[i];
        return null;
    }


    #region _DFSMap_

    private Stack<Path> stack = new Stack<Path>();
    private int[,] road;
    private Path curPath;
    private List<Path> guideMapA = new List<Path>();
    private List<Path> guideMapB = new List<Path>();
    private List<Path> unitMapA = new List<Path>();
    private List<Path> unitMapB = new List<Path>();
    public List<Path> GuideMapA() => guideMapA;
    public List<Path> GuideMapB() => guideMapB;

    public List<Path> UnitMapA() => unitMapA;
    public List<Path> UnitMapB() => unitMapB;

    private List<Path> nowMap;

    private void FindMapPathDFS()
    {
        road = new int[mapY, mapX];

        var type = PathType.MonsterMovePath;

        nowMap = guideMapA;
        curPath = map[0];
        while (true)
        {
            if (!Detect(curPath, type))
            {
                if (stack.Count > 0)
                    curPath = stack.Pop();
                else
                    break; // 종료
            }
        }

        road = new int[mapY, mapX];
        stack.Clear();

        nowMap = guideMapB;
        curPath = map[mapX * (mapY - 1)];
        while (true)
        {
            if (!Detect(curPath, type))
            {
                if (stack.Count > 0)
                    curPath = stack.Pop();
                else
                    break; // 종료
            }
        }

        type = PathType.UnitSetPath;

        road = new int[mapY, mapX];
        stack.Clear();

        nowMap = unitMapA;
        curPath = map[mapX * 1 + 1];
        while (true)
        {
            if (!Detect(curPath, type))
            {
                if (stack.Count > 0)
                    curPath = stack.Pop();
                else
                    break; // 종료
            }
        }



        road = new int[mapY, mapX];
        stack.Clear();

        nowMap = unitMapB;
        curPath = map[mapX * (mapY - 2) + 1];
        while (true)
        {
            if (!Detect(curPath, type))
            {
                if (stack.Count > 0)
                    curPath = stack.Pop();
                else
                    break; // 종료
            }
        }
    }

    private bool Detect(Path curPos, PathType type)
    {
        if (curPos.pathType != type) return false;
        if (road[curPos.point.y, curPos.point.x] == 1) return false;

        Visit(curPos);
        nowMap.Add(curPos);

        return true;
    }

    private void Visit(Path curPos)
    {
        road[curPos.point.y, curPos.point.x] = 1;

        Push(curPos.point.y, curPos.point.x + 1);
        Push(curPos.point.y + 1, curPos.point.x);
        Push(curPos.point.y, curPos.point.x - 1);
        Push(curPos.point.y - 1, curPos.point.x);
    }

    private void Push(int y, int x)
    {
        // 범위 초과시
        if (x < 0 || x >= mapX) return;

        if ((nowMap == guideMapA || nowMap == unitMapA) && !(y >= 0 && y < middleY)) return;
        if ((nowMap == guideMapB || nowMap == unitMapB) && !(y >= (mapY - middleY) && y < mapY)) return;

        stack.Push(map[y * mapX + x]);
    }

    #endregion

    private void Sort()
    {
        var aMap = UnitMapA();
        aMap.Sort(delegate (Path a, Path b)
        {
            if (a.point.x == b.point.x)
                return b.point.y.CompareTo(a.point.y);
            else
                return a.point.x.CompareTo(b.point.x);
        });

        var bMap = UnitMapB();
        bMap.Sort(delegate (Path a, Path b)
        {
            if (a.point.x == b.point.x)
                return b.point.y.CompareTo(a.point.y);
            else
                return a.point.x.CompareTo(b.point.x);
        });

    }

    public List<Path> GetUnitPoint()
    {
        return UnitMapA();
    }

}
