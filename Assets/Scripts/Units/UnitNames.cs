using System.Collections.Generic;
using UnityEngine;

public class UnitNames : MonoBehaviour
{
    public int lastUnitName;

    void Start()
    {
        lastUnitName = 0;
    }

    void Update()
    {
    }

    Dictionary<int, string> namesDictionary = new Dictionary<int, string> {
        [1] = "ShipOne",
        [2] = "ShipTwo",
        [3] = "ShipThree",
        [4] = "ShipFour",
        [5] = "ShipFive",
    };

    public string GETNextName()
    {
        lastUnitName += 1;
        return namesDictionary[lastUnitName];
    }
}