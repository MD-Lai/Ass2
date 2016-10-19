using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour {
    public static int xAspect = 1;
    public static int zAspect = 1;
    public ScrollBars sliderVals;
	
    public void getAspects() {
        xAspect = (int)sliderVals.scrollX.value;
        zAspect = (int)sliderVals.scrollY.value;
    }
}
