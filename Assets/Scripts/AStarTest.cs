using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarTest : MonoBehaviour
{
    //地图格子的宽高
    public int mapW = 10;
    public int mapH = 10;
    public float offset = 2;
    public Material red;
    public Material green;
    public Material normal;
    public Material yellow;

    Dictionary<string, GameObject> cubes = new Dictionary<string, GameObject>();
    List<AStarNode> aimList;
    //起始点
    private Vector2 beginPos = Vector2.right * -1;
    // Start is called before the first frame update
    void Start()
    {
        AStarMgr.Instance.InitMapInfo(mapW, mapH);

        for (int i = 0; i < mapW; i++)
        {
            for (int j = 0; j < mapH; j++)
            {
                //创建立方体
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = new Vector3( i * offset,  j * offset, 0);
                obj.name = i + "_" + j;
                cubes.Add(obj.name, obj);

                AStarNode node = AStarMgr.Instance.nodes[i, j];
                if (node != null && node.type == E_Node_Type.Stop)
                    obj.GetComponent<MeshRenderer>().material = red;
                else
                    obj.GetComponent<MeshRenderer>().material = normal;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //按下鼠标左键
        if(Input.GetMouseButtonDown(0))
        {
            //进行射线检测
            RaycastHit info;
            //得到屏幕鼠标位置发出去的射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //射线检测
            if(Physics.Raycast(ray,out info,1000))
            {
                string[] indexs = info.collider.gameObject.name.Split('_');
                //起点
                if (beginPos == Vector2.right * -1)
                {
                    if (aimList != null)
                    {
                        for (int i = 0; i < aimList.Count; i++)
                        {
                            cubes[aimList[i].x + "_" + aimList[i].y].GetComponent<MeshRenderer>().material = normal;
                        }
                    }
                    
                    beginPos = new Vector2(int.Parse(indexs[0]), int.Parse(indexs[1]));
                    //点击对象改为黄色
                    info.collider.gameObject.GetComponent<MeshRenderer>().material = yellow;

                }
                //终点
                else
                {
                    Vector2 endPos = new Vector2(int.Parse(indexs[0]), int.Parse(indexs[1]));

                    //开始寻路
                    aimList = AStarMgr.Instance.FindPath(beginPos, endPos);
                    if(aimList != null)
                    {
                        for (int i = 0; i < aimList.Count; i++)
                        {
                            cubes[aimList[i].x + "_" + aimList[i].y].GetComponent<MeshRenderer>().material = green;
                        }
                    }
                    //重置
                    beginPos = Vector2.right * -1;
                }
            }
        }
    }
}
