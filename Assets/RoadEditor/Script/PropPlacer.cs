﻿using UnityEngine;
using System.Collections.Generic;

public class PropPlacer : MonoBehaviour
{
	enum PlaysmentType{
		relativeToCenter,
		relativeToLeft,
		RelativeToRight
	}
	[SerializeField]
	private PlaysmentType propPlacementType;
	[SerializeField]
	private Vector3 offset;
    //public Node[] path;
    [HideInInspector]
    [SerializeField]
    private List<Transform> pathList = new List<Transform>();

    [HideInInspector]
    [SerializeField]
    private Transform holder;

    [SerializeField]
    private Track[] trackParts;

    [SerializeField]
    private float bezierStepSize = 1;

    
    [SerializeField]
    private GameObject prop;
    /*
    [SerializeField]
    private bool right = true;

    [SerializeField]
    private bool left = true;
    */
    //private int pathLength;

    void Start()
    {
       // pathLength = pathList.count;
    }

    void AddPathPoint(int number)
    {
        Transform newPathePoint = new GameObject("P" + number.ToString()).transform;
        newPathePoint.transform.position = transform.position;
        pathList.Add(newPathePoint);
    }

    //returns true if build
    public bool Rebuild()
    {
        int i;
        int j;

        //create new
        List<Vector3> pointList = new List<Vector3>();
        List<Quaternion> rotationList = new List<Quaternion>();
        List<float> widthList = new List<float>();

        Vector3[] tempPositions = new Vector3[1] { new Vector3(0, 0, 0) };
        Quaternion[] tempRotatioins = new Quaternion[1] { new Quaternion(0, 0, 0, 0) };
        float[] tempWidth = new float[1] { 0 };
        float[] tempHeight = new float[1] { 0 };

        //List<Node> newNodes = new List<Node>();

        Debug.Log("trackParts L:" + trackParts.Length);
        for (i = 0; i < trackParts.Length; i++)
        {
            float? bezierTimeStepSize = trackParts[i].BezierStepSize(bezierStepSize);
            bool createPointsSucses = trackParts[i].CreateBezieredPoints(ref tempPositions, ref tempRotatioins, ref tempWidth, ref tempHeight, false, bezierTimeStepSize.Value);
            if (createPointsSucses)
            {
                float pointCount = tempPositions.Length;
                for (j = 0; j < pointCount-1; j++) /// -1 SKIPS LAST POINT
                {
                    pointList.Add(tempPositions[j]);
                    rotationList.Add(tempRotatioins[j]);
                    widthList.Add(tempWidth[j]);
                }
            }
            else
            {
                return false;
            }
        }

        Debug.Log("pointList COUNT L:" + pointList.Count);
       
        //remove old
        for (i = pathList.Count - 1; i > -1; i--)
        {
            if (pathList[i] != null)
            {
                GameObject.DestroyImmediate(pathList[i].gameObject);
                //pathList.RemoveAt(i);
            }
            pathList.RemoveAt(i);
        }
        
        // if holder is null create holder
        if (holder == null)
        {
            holder = new GameObject("Prop Holder").transform;
            holder.parent = transform;
        }

        //add nodes if needed
        for (i = 0; i < pointList.Count; i++)
        {
            Debug.Log("pointList i:" + i);
            if (i >= pathList.Count)
            {
                Debug.Log("add");
                AddPathPoint(i);
            }
            if (pathList[i] == null)
            {
                AddPathPoint(i);
            }

            for (j = 0; j < pathList.Count; j++)
            {
                if (i != j)
                {
                    if (pathList[i] == pathList[j])
                    {
                        AddPathPoint(i);
                    }
                }
            }
            pathList[i].transform.parent = holder.transform;
#if UNITY_EDITOR
            //IconManager.SetIcon(pathList[i].gameObject, IconManager.LabelIcon.Purple);
#endif
        }

        //Debug.Log("pointList COUNT L:" + pointList.Count);
       
        //update
        for (i = 0; i < pointList.Count; i++)
        {
            pathList[i].position = pointList[i];
            pathList[i].rotation = rotationList[i];
			if(propPlacementType == PlaysmentType.relativeToLeft){
				pathList[i].Translate(new Vector3(-(widthList[i] / 2), 0, 0), Space.Self);
			}else if(propPlacementType == PlaysmentType.RelativeToRight){
				pathList[i].Translate(new Vector3((widthList[i] / 2), 0, 0), Space.Self);
			}
			pathList[i].Translate(offset,Space.Self);
            GameObject newProp = (GameObject)GameObject.Instantiate(prop, pathList[i].position, pathList[i].rotation);
            newProp.isStatic = true;
            newProp.name = "paal";
            newProp.transform.parent = pathList[i];
            //pathList[i].maxLeft.Translate(new Vector3((widthList[i]/2), 0, 0), Space.Self);
        }

        return true;
    }

    public int getNextID(int currentID)
    {
        currentID++;
        if (currentID > pathList.Count - 1)
        {
            currentID = 0;
        }
        return currentID;
    }
}
