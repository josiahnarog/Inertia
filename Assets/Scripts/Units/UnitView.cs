using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class UnitView : MonoBehaviour {

    // TODO: 
    void Start()
    {
        newPosition = this.transform.position;
        newAngles = this.transform.eulerAngles;
    }

    public bool unitAnimationPlaying = false;

    private bool unitNotMoving = true;
    private bool unitNotRotating = true;

    Vector3 newPosition;
    Vector3 newAngles;

    Vector3 currentVelocity;
    Vector3 currentRotationalVelocity;
    
    // TODO: We should actually set this speed to correspond to the pulse, so fast ships move... faster.
    float smoothTime = 0.5f;
    private float smoothRotation = 60; //Degrees per second.

    public void OnUnitSteered(Direction newDirection)
    {
        this.unitAnimationPlaying = true;
        
        Vector3 oldAngles = this.transform.eulerAngles;
        newAngles = new Vector3(
                0, newDirection.Rotation, 0
            );
        currentRotationalVelocity = Vector3.zero;
    }
    
    public void OnUnitMoved( Hex oldHex, Hex newHex )
    {
        // This GameObject is supposed to be a child of the hex we are
        // standing in. This ensures that we are in the correct place
        // in the hierarchy
        // Our correct position when we aren't moving, is to be at
        // 0,0 local position relative to our parent.
        
        this.unitAnimationPlaying = true;

        Vector3 oldPosition = oldHex.PositionFromCamera();
        newPosition = newHex.PositionFromCamera();
        currentVelocity = Vector3.zero;
        
        this.transform.position = oldPosition;

        if( Vector3.Distance(this.transform.position, newPosition) > 2 )
        {
            // This OnUnitMoved is considerably more than the expected move
            // between two adjacent tiles -- it's probably a map seam thing,
            // so just teleport
            this.transform.position = newPosition;
        }
        else {
            // TODO: WE need a better signalling system and/or animation queueing
            GameObject.FindObjectOfType<HexMap>().AnimationIsPlaying = true;
        }
    }

    void Update()
    {

        this.transform.position = Vector3.SmoothDamp( this.transform.position, newPosition, ref currentVelocity, smoothTime );

        // The step size is equal to speed times frame time.
        var step = smoothRotation * Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(newAngles);
        
        // Rotate our transform a step closer to the target's.
        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRotation, step);

        this.unitNotMoving = Quaternion.Angle(this.transform.rotation, targetRotation) < 0.1f;
        this.unitNotRotating = Vector3.Distance(this.transform.position, newPosition) < 0.1f;

        if(unitNotMoving  &&  unitNotRotating)
        {
            GameObject.FindObjectOfType<HexMap>().AnimationIsPlaying = false;
            this.unitAnimationPlaying = false;
        }
        
        
    }


}
