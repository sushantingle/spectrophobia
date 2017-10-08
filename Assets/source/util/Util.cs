using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour{

    public static List<Transform> getTransformListWithLayer(int layerId) {
        List<Transform> layerTrans = new List<Transform>();

        var ObjList = FindObjectsOfType(typeof(GameObject));
        foreach (GameObject obj in ObjList)
        {
            if (obj.layer == layerId)
                layerTrans.Add(obj.transform);
        }

        return layerTrans;
    }
}
