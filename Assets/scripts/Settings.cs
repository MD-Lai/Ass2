using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Settings : MonoBehaviour {
    public static int xAspect = 1;
    public static int zAspect = 1;
    public static bool useTablet = false;
    public ScrollBars sliderVals;
	
    public void getAspects() {
        xAspect = (int)sliderVals.scrollX.value;
        zAspect = (int)sliderVals.scrollY.value;
    }
    
    public void setTablet() {
        useTablet = true;
    }

    public void setKeyboard() {
        useTablet = false;
    }
}
