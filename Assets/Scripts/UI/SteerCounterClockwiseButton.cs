using UnityEngine;

namespace UI
{
    public class SteerCounterClockwiseButton : MonoBehaviour
    {
        public void SteerCounterClockwise()
        {
            SelectionController sc = GameObject.FindObjectOfType<SelectionController>();
            Unit u = sc.SelectedUnit;
            SteerCounterClockwise steerCounterClockwise = new SteerCounterClockwise();
            u.AddToMoveQueue(steerCounterClockwise);
        }
    }
}
