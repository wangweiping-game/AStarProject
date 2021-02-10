using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Node_Type
{
    Walk,
    Stop,
}
/// <summary>
/// A*格子类
/// </summary>
public class AStarNode 
{
    //格子对象的坐标
    public int x;
    public int y;

    //寻路消耗
    public float f;
    //离起点的距离
    public float g;
    //离终点的距离
    public float h;

    public AStarNode father;

    //格子类型
    public E_Node_Type type;


    public AStarNode(int x,int y,E_Node_Type type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }
}
