using UnityEngine;
using System.Collections;

public class OrientationController : MonoBehaviour
{
    public enum Orientation
    {
        Portrait,
        Landscape,
    }

    public Orientation _screenOrientation;

    private void Start()
    {
        switch (_screenOrientation)
        {
            case Orientation.Portrait:
                Screen.orientation = Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown ?
                    ScreenOrientation.PortraitUpsideDown :
                    ScreenOrientation.Portrait;

                Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = true;
                Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = false;
                break;

            case Orientation.Landscape:
                Screen.orientation = Input.deviceOrientation == DeviceOrientation.LandscapeRight ?
                    ScreenOrientation.LandscapeRight :
                    ScreenOrientation.LandscapeLeft;

                Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = false;
                Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = true;
                break;
        }
    }
}