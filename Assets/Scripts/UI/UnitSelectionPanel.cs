﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionPanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        selectionController = GameObject.FindObjectOfType<SelectionController>();
	}

	public int moveQueueLength;

    public Text Title;
    public Text Movement;
    public Text Facing;
    public Text MoveQueueLength;
    // public Text TurnRatingRemaining;

    public GameObject TurnCounterClockwiseButton;
    public GameObject MoveAheadButton;
    public GameObject TurnClockwiseButton;
    public GameObject CityBuildButton;

    SelectionController selectionController;
    	
	// Update is called once per frame
	void Update () {
		
        if(selectionController.SelectedUnit != null)
        {

            Title.text    = selectionController.SelectedUnit.Name;

            Movement.text = string.Format(
                "MP Remaining: {0}/{1}", 
                selectionController.SelectedUnit.MovementPointsRemainingThisPulse, 
                selectionController.SelectedUnit.MovementRating
            );

            moveQueueLength = selectionController.SelectedUnit.GetMoveQueueLength();
            MoveQueueLength.text  =  string.Format("Move queue length: {0}",moveQueueLength);
            
            Direction facing = selectionController.SelectedUnit.Facing;
            Facing.text = string.Format("Current Facing: {0} {1} {2}",
	            facing.Id, facing.Name, facing.Rotation);

            TurnCounterClockwiseButton.SetActive( true );
            MoveAheadButton.SetActive( true );
            TurnClockwiseButton.SetActive( true );

            if( selectionController.SelectedUnit.CanBuildCities && selectionController.SelectedUnit.Hex.City == null)
            {
                CityBuildButton.SetActive( true );
            }
            else
            {
                CityBuildButton.SetActive( false );
            }

        }

	}
}
