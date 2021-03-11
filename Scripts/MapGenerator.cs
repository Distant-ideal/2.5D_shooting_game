using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public GameObject tilePrefab;
    public Vector2 mapsize; //public float mapSizeX, mapSizeY; 
    public Transform mapHolder;
    [Range(0, 1)] public float outlinePercent;

    public GameObject obsPrefab;
    //public float obsCount;
    public List<Coord> allTilesCoord = new List<Coord>();

    private Queue<Coord> shuffledQueue;
    public Color foregroundColor, backgroundColor;
    public float minObsHeight, maxObsHeight;

    [Header("Map Fully Accessible")]
    [Range(0, 1)] public float obsPercent;
    private Coord mapCenter; //任何随机地图，中心点都不能有任何阻碍物，这个点用来人物生成和填充算法判定使用的
    bool[,] mapObstacles; //判断任何坐标位置是否有障碍物

    [Header("NaveMesh Agent")]
    public Vector2 mapMaxSize;
    public GameObject navMeshObs;
    public GameObject player;

    private void Start() 
    {
        GenerateMap();
        Init();
    }

    private void Init()
    {
        Instantiate(player, new Vector3(-mapsize.x / 2 + 0.5f + mapCenter.x, 0, -mapsize.y / 2 + 0.5f + mapCenter.y), Quaternion.identity);
    }

    private void GenerateMap()
    {
        //生成瓦片
        for (int i = 0; i < mapsize.x; i++)
        {
            for (int j = 0; j < mapsize.y; j++)
            {
                Vector3 newPos = new Vector3(-mapsize.x / 2 + 0.5f + i, 0, -mapsize.y / 2 + 0.5f + j);
                GameObject spawTile = Instantiate(tilePrefab, newPos, Quaternion.Euler(90, 0, 0));
                spawTile.transform.SetParent(mapHolder);
                spawTile.transform.localScale *= (1 - outlinePercent);

                allTilesCoord.Add(new Coord(i, j));
            }
        }
        //障碍物随机生成
        /*for (int i = 0; i < obsCount; i++)
        {
            Coord randomCoord = allTilesCoord[UnityEngine.Random.Range(0, allTilesCoord.Count)];

            Vector3 newPos = new Vector3(-mapsize.x / 2 + 0.5f + randomCoord.x, 0, -mapsize.y / 2 + 0.5f + randomCoord.y);
            GameObject spawObs = Instantiate(obsPrefab, newPos, Quaternion.identity);
            spawObs.transform.SetParent(mapHolder);
            spawObs.transform.localScale *= (1 - outlinePercent);
        }*/

        shuffledQueue = new Queue<Coord>(Utilities.ShuffleCoords(allTilesCoord.ToArray()));

        int obsCount = (int)(mapsize.x * mapsize.y * obsPercent);
        mapCenter = new Coord((int)mapsize.x / 2, (int)mapsize.y / 2); //设置地图中心坐标点
        mapObstacles = new bool[(int)mapsize.x, (int)mapsize.y]; //初始化

        int currentObsCount = 0;
    
        for (int i = 0; i < obsCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            mapObstacles[randomCoord.x, randomCoord.y] = true; //假设随机坐标位置有障碍物
            currentObsCount++;

            //并不是所有生成的障碍物都能够随机位置生成
            if (randomCoord != mapCenter && mapIsFullyAccessible(mapObstacles, currentObsCount))
            {
                //float obsHeight = UnityEngine.Random.Range(minObsHeight, maxObsHeight);
                float obsHeight = Mathf.Lerp(minObsHeight, maxObsHeight, UnityEngine.Random.Range(0f, 1f));

                Vector3 newPos = new Vector3(-mapsize.x / 2 + 0.5f + randomCoord.x, obsHeight / 2, -mapsize.y / 2 + 0.5f + randomCoord.y);
                GameObject spawnObs = Instantiate(obsPrefab, newPos, Quaternion.identity);
                spawnObs.transform.SetParent(mapHolder);
                //spawnObs.transform.localScale *= (1 - outlinePercent);
                spawnObs.transform.localScale = new Vector3(1 - outlinePercent, obsHeight, 1 - outlinePercent);

                #region
                MeshRenderer meshRender = spawnObs.GetComponent<MeshRenderer>();
                Material material = meshRender.material;

                float colorPercent = randomCoord.y / mapsize.y;
                material.color = Color.Lerp(foregroundColor, backgroundColor, colorPercent);
                meshRender.material = material;
                #endregion
            } 
            else
            {
                mapObstacles[randomCoord.x, randomCoord.y] = false; //假设随机坐标位置没有障碍物
                currentObsCount--;
            }

        }

        //动态创建空气墙
        //上下左右的顺序
        GameObject navMeshObsForward = Instantiate(navMeshObs, Vector3.forward * (mapMaxSize.y + mapsize.y) / 4, Quaternion.identity);
        navMeshObsForward.transform.localScale = new Vector3(mapsize.x, 5, (mapMaxSize.y / 2 - mapsize.y / 2));

        GameObject navMeshObsBack = Instantiate(navMeshObs, Vector3.back * (mapMaxSize.y + mapsize.y) / 4, Quaternion.identity);
        navMeshObsBack.transform.localScale = new Vector3(mapsize.x, 5, (mapMaxSize.y / 2 - mapsize.y / 2));

        GameObject navMeshObsLeft = Instantiate(navMeshObs, Vector3.left * (mapMaxSize.x + mapsize.x) / 4, Quaternion.identity);
        navMeshObsLeft.transform.localScale = new Vector3((mapMaxSize.x / 2 - mapsize.x / 2), 5, mapsize.y);

        GameObject navMeshObsRight = Instantiate(navMeshObs, Vector3.right * (mapMaxSize.x + mapsize.x) / 4, Quaternion.identity);
        navMeshObsRight.transform.localScale = new Vector3((mapMaxSize.x / 2 - mapsize.x / 2), 5, mapsize.y);
    }

    private bool mapIsFullyAccessible(bool[,] _mapObstacles, int _currentObsCount)
    {
        bool[,] mapFlags = new bool[_mapObstacles.GetLength(0), _mapObstacles.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>(); //所有坐标都会“筛选后”存储在这个队列中
        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.x, mapCenter.y] = true; //中心点已标记为[已检测]

        int accessibleCount = 1; //等于1是因为我们的MapCenter无论何时都是可以行走可以通过的
        while (queue.Count > 0) //队列只要大于0就一直检测下去去
        {
            Coord currentTile = queue.Dequeue(); //检测的就要被移除
            for (int x = -1; x <= 1; x++) //检测相邻四周的坐标点X轴
            {
                for (int y = -1; y <= 1; y++) //检测相邻四周的坐标点Y轴
                {
                    int neighborX = currentTile.x + x;
                    int neighborY = currentTile.y + y;
                    if (x == 0 || y == 0) //保证上下左右四哥位置，排除45度角方向的坐标位置
                    {
                        if (neighborX >= 0 && neighborX < _mapObstacles.GetLength(0)
                        && neighborY >= 0 && neighborY < _mapObstacles.GetLength(1)) //防止相邻点，超出地图临界位置
                        {
                            //保证相邻点：1.没有被检测，mapFlags为False， 2.mapObstacle也为false没有障碍物
                            if (!mapFlags[neighborX, neighborY] && !_mapObstacles[neighborX, neighborY])
                            {
                                mapFlags[neighborX, neighborY] = true;
                                accessibleCount++;
                                queue.Enqueue(new Coord(neighborX, neighborY));
                            }
                        }
                    }
                }
            }
        }
        int obsTargetCount = (int)(mapsize.x * mapsize.y - _currentObsCount); //我们应该能行走的瓦片/格子数量
        return accessibleCount == obsTargetCount;
    }

    private Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledQueue.Dequeue();
        shuffledQueue.Enqueue(randomCoord);
        return randomCoord;
    }
} 

[System.Serializable]
public struct Coord
{
    public int x;
    public int y;
    public Coord(int _x, int _y)
    {
        this.x = _x;
        this.y = _y;
    }
    public static bool operator != (Coord _c1, Coord _c2)
    {
        return !(_c1 == _c2);
    }
    public static bool operator ==(Coord _c1, Coord _c2)
    {
        return (_c1.x == _c2.x) && (_c1.y == _c2.y);
    }
}
