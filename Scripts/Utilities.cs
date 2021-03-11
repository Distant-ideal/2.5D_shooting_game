using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour {
    //将传递进来的参数：_dataArray进行重新洗牌，打乱顺序
    public static T[] ShuffleCoords<T>(T[] _dataArray)
    {
        for (int i = 0; i < _dataArray.Length; i++)
        {
            int randomNum = Random.Range(i, _dataArray.Length);
            T temp = _dataArray[randomNum];
            _dataArray[randomNum] = _dataArray[i];
            _dataArray[i] = temp;
        }
        return _dataArray;
    }
}
