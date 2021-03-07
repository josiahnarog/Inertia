using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CitySelectionPanel : MonoBehaviour
{

    private InertiaTurnController _turnController;
    
    public Text CurrentPhase;
    
    void Start ()
    {
        _turnController = GameObject.FindObjectOfType<InertiaTurnController>();
    }

    void Update()
    {
        CurrentPhase.text = string.Format("Current Phase: {0}", _turnController.turnPulse);
    }
}
