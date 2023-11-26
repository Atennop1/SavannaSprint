using UnityEngine;

public class OrientationSetter : MonoBehaviour
{
    public enum Orientation
    {
        Portrait,
        Landscape,
    }

    [SerializeField] private Orientation _screenOrientation;

    private void FixedUpdate()
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