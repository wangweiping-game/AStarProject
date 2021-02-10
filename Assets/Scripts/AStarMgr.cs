using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarMgr 
{
    private static AStarMgr instance;
    public static AStarMgr Instance
    {
        get
        {
            if (instance == null)
                instance = new AStarMgr();
            return instance;
        }
    }
    //地图宽高
    private int mapW;
    private int mapH;
    //格子容器
    public AStarNode[,] nodes;
    //开启列表
    private List<AStarNode> openList = new List<AStarNode>();
    //关闭列表
    private List<AStarNode> closeList = new List<AStarNode>();

    /// <summary>
    /// 初始化地图
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    public void InitMapInfo(int w,int h)
    {
        this.mapW = w;
        this.mapH = h;
        nodes = new AStarNode[w, h];

        //根据宽高 创建格子 ，随机阻挡
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                nodes[i,j] = new AStarNode(i, j, (Random.Range(1, 100) < 20) ? E_Node_Type.Stop : E_Node_Type.Walk);
            }
        }
    }

    /// <summary>
    /// 寻路方法 提供给外部使用
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    public List<AStarNode> FindPath(Vector2 startPos,Vector2 endPos)
    {
        //首先判断传入的两个点 是否合法
        //不合法 直接返回Null
        //1，首先要在地图范围内
        if (startPos.x < 0 || startPos.x >= mapW
           || startPos.y < 0 || startPos.y >= mapH
           || endPos.x < 0 || endPos.x >= mapW
           || endPos.y < 0 || endPos.y >= mapH)
        {
            Debug.Log("起点或者终点在地图范围外！");
            return null;
        }
        
        //2，不是阻挡格子
        AStarNode startNode = nodes[(int)startPos.x, (int)startPos.y];
        AStarNode endNode = nodes[(int)endPos.x, (int)endPos.y];
        if (startNode.type == E_Node_Type.Stop ||
           endNode.type == E_Node_Type.Stop)
        {
            Debug.Log("起点或者终点是障碍物");
            return null;
        }

        //清空开启列表和关闭列表
        closeList.Clear();
        openList.Clear();

        //把起点放入关闭列表中
        startNode.father = null;
        startNode.f = 0;
        startNode.g = 0;
        startNode.h = 0;
        closeList.Add(startNode);

        startNode.x = (int)startPos.x;
        startNode.y = (int)startPos.y;

        while(true)
        {
            //从起点开始 找周围的点 并放入开启列表（去掉障碍物的格子）
            //左上 x-1 y-1
            FindNearlyNodeToOpenList(startNode.x - 1, startNode.y - 1, 1.4f, startNode, endNode);
            //上 x y-1
            FindNearlyNodeToOpenList(startNode.x, startNode.y - 1, 1, startNode, endNode);
            //右上 x+1 y-1
            FindNearlyNodeToOpenList(startNode.x + 1, startNode.y - 1, 1.4f, startNode, endNode);
            //左 x-1 y
            FindNearlyNodeToOpenList(startNode.x - 1, startNode.y, 1f, startNode, endNode);
            //右 x+1 y
            FindNearlyNodeToOpenList(startNode.x + 1, startNode.y, 1f, startNode, endNode);
            //左下 x-1 y+1
            FindNearlyNodeToOpenList(startNode.x - 1, startNode.y + 1, 1.4f, startNode, endNode);
            //下 x y+1
            FindNearlyNodeToOpenList(startNode.x, startNode.y + 1, 1f, startNode, endNode);
            //右下 x+1 y+1
            FindNearlyNodeToOpenList(startNode.x + 1, startNode.y + 1, 1.4f, startNode, endNode);

            //开启列表是空 都还没找到终点的话 认为是死路
            if(openList.Count == 0)
            {
                Debug.Log("死路");
                return null;
            }

            //选出开启列表中 寻路消耗最小的点 放入关闭列表中
            openList.Sort((x, y) =>
            {
                if (x.f > y.f)
                    return 1;
                return -1;
            });

            //将开启列表中消耗最小的点放入关闭列表中
            startNode = openList[0];
            closeList.Add(openList[0]);
            openList.RemoveAt(0);

            //找到路径了
            if(startNode == endNode)
            {
                List<AStarNode> path = new List<AStarNode>();
                path.Add(endNode);
                while(endNode.father != null)
                {
                    path.Add(endNode.father);
                    endNode = endNode.father;
                }
                //路径翻转
                path.Reverse();
                return path;
            }
        }
    }
    /// <summary>
    /// 判断周围的点是否可以放入开启列表
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void FindNearlyNodeToOpenList(int x,int y,float g, AStarNode father,AStarNode end)
    {
        //边界判断
        if (x < 0 || x >= mapW ||
            y < 0 || y >= mapH)
            return;
        //在范围内再去取点
        AStarNode node = nodes[x, y];

        //判断该点 是否是边界 是否是阻挡 是否在开启列表或者关闭列表 如果都不是则放入开启列表
        if (node == null || node.type == E_Node_Type.Stop || closeList.Contains(node) || openList.Contains(node))
            return;

        //计算f值 f= g + h
        node.father = father;
        node.g = father.g + g;
        node.h = Mathf.Abs(end.x - node.x) + Mathf.Abs(end.y - node.y);
        node.f = node.g + node.h;

        openList.Add(node);
    }
}
