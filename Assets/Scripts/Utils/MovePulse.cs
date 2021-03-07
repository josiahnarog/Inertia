using System;
using static System.Math;


public class MovePulse
{
    public int First(int movementRating)
    {
        int movePulseNum = 1;
        
        int movePoints = movementRating / 3; //Will round down.

        if ((movePoints % 3) >= movePulseNum)
        {
            movePoints += 1;
        }
        
        return(movePoints);
    }
    
    public int Second(int movementRating)
    {
        int movePulseNum = 2;
        
        int movePoints = movementRating / 3; //Will round down.

        if ((movePoints % 3) >= movePulseNum)
        {
            movePoints += 1;
        }
        
        return(movePoints);
    }
    
    public int Third(int movementRating)
    {
        int movePoints = movementRating / 3; //Will round down.
        return(movePoints);
    }
}