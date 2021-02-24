using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour {

    void Start()
    {
        newPosition = this.transform.position;
        newAngles = this.transform.eulerAngles;
    }

    Vector3 newPosition;
    Vector3 newAngles;

    Vector3 currentVelocity;
    Vector3 currentRotationalVelocity;
    float smoothTime = 0.5f;
    private float smoothRotation = 60; //Degrees per second.

    public void OnUnitSteered(Direction newDirection)
    {
        Vector3 oldAngles = this.transform.eulerAngles;

        if (newAngles.y == 0)
        {
            Debug.LogError("It's a Zero!");
            oldAngles.y = -60;
        }

        newAngles = new Vector3(
                0, newDirection.Rotation, 0
            );
        
        // TODO Need to be able to steer from a negative angle to 0 as well;
        currentRotationalVelocity = Vector3.zero;
    }

    public void OnUnitMoved( Hex oldHex, Hex newHex )
    {
        // This GameObject is supposed to be a child of the hex we are
        // standing in. This ensures that we are in the correct place
        // in the hierarchy
        // Our correct position when we aren't moving, is to be at
        // 0,0 local position relative to our parent.

        // TODO: Get rid of VerticalOffset and instead use a raycast to determine correct height
        // on each frame.

        Vector3 oldPosition = oldHex.PositionFromCamera();
        newPosition = newHex.PositionFromCamera();
        currentVelocity = Vector3.zero;


        // TODO:  newPosition's Y component needs to be set from HexComponent's VerticalOffset
        oldPosition.y += oldHex.HexMap.GetHexGO(oldHex).GetComponent<HexComponent>().VerticalOffset;
        newPosition.y += newHex.HexMap.GetHexGO(newHex).GetComponent<HexComponent>().VerticalOffset;
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

        // TODO: Figure out the best way to determine the end of our animation
        if( Vector3.Distance( this.transform.position, newPosition ) < 0.1f )
        {
            GameObject.FindObjectOfType<HexMap>().AnimationIsPlaying = false;
        }
        
        // The step size is equal to speed times frame time.
        var step = smoothRotation * Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(newAngles);
        
        // Rotate our transform a step closer to the target's.
        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRotation, step);
        
    }


}
