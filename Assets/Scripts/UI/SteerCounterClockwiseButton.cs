using UnityEngine;

namespace UI
{
    public class SteerCounterClockwiseButton : MonoBehaviour
    {
        public void SteerCounterClockwise()
        {
            SteerDirection steer = SteerDirection.Counterclockwise;
            SelectionController sc = GameObject.FindObjectOfType<SelectionController>();
            Unit selectedUnit = sc.SelectedUnit;
            selectedUnit.DoSteer(steer);
        }
    }
}
