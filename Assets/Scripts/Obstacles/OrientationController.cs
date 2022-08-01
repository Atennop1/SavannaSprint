using UnityEngine;
using System;
using System.Collections;

public class OrientationController : MonoBehaviour
{
    public enum Orientation
    {
        Portrait,
        Landscape,
    }

    public Orientation ScreenOrientation;

    private void Start()
    {
        switch (ScreenOrientation)
        {
            case Orientation.Portrait:
                if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
                    Screen.orientation = UnityEngine.ScreenOrientation.PortraitUpsideDown;
                else
                    Screen.orientation = UnityEngine.ScreenOrientation.Portrait;

                Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = true;
                Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = false;
                break;

            case Orientation.Landscape:
                if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
                    Screen.orientation = UnityEngine.ScreenOrientation.LandscapeRight;
                else
                    Screen.orientation = UnityEngine.ScreenOrientation.LandscapeLeft;

                Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = false;
                Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = true;

                break;
        }
    }
}