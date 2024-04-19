using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Gaia")]
public class Gaia : ScriptableObject
{
    private static Gaia _this;
    
    [SerializeField] protected List<GameObject> _plainTrees = new List<GameObject>();
    [SerializeField] protected List<GameObject> _desertTrees = new List<GameObject>();
    [SerializeField] protected List<GameObject> _plainBushes = new List<GameObject>();
    [SerializeField] protected List<GameObject> _desertBushes = new List<GameObject>();
    
    #region Init
    public static void Init()
    {
        if (_this != null)
        {
            return;
        }

        _this = Resources.Load<Gaia>("Gaia");
    }
    
    #endregion

    private static GameObject GetRandom(List<GameObject> fromList)
    {
        return fromList[Random.Range(0, fromList.Count)];
    }

    public static GameObject GetRandomTree(bool plain)
    {
        if (plain)
        {
            return GetRandom(_this._plainTrees);
        }
        else
        {
            return GetRandom(_this._desertTrees);
        }
    }
    
    public static GameObject GetRandomBush(bool plain)
    {
        if (plain)
        {
            return GetRandom(_this._plainBushes);
        }
        else
        {
            return GetRandom(_this._desertBushes);
        }
    }
}
