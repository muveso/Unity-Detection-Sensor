# Detection Sensor For Unity

[![Unity](https://img.shields.io/badge/unity-all-blue.svg)](https://unity3d.com/get-unity/download)
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://github.com/muveso/Attribute-Injector/blob/main/LICENSE)
<br/><br/><a href="https://www.buymeacoffee.com/muveso" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/yellow_img.png" alt="Buy Me A Coffee"></a>
<br/>
<br/>
![4](https://user-images.githubusercontent.com/94571116/156586011-39a2feea-451a-46ba-b1e1-284ec4fb45c1.gif)
<br/>
<br/>
The **Detection Sensor** provides the basic functionality required for visual object detection. It wraps a sensor and exposes a ray list, to which you can use for raycast. You can instantiate and assign the sensor yourself.



## Simple Usage

#### Create a gameobject and attach **DetectionSensor** component :

- Script automatically add visual components for editor.
- Adjust vertical and horizontal ray count.
- Adjust ray positions on scene.

![1](https://user-images.githubusercontent.com/94571116/156586800-b0577a8e-bee8-49b3-9174-d69a4b7cb8e4.png)

#### **DetectionSensor** will also simulate scene :
<br/>

![7](https://user-images.githubusercontent.com/94571116/156588088-88799de1-a21a-4e32-857f-2c6b29f99eae.png)

#### You can use your custom scripts with **DetectionSensor** :

```C#
[SerializeField] public DetectionSensor detectionSensor;
private DetectionRay[] _sensorRays;

private void Start()
{
    _sensorRays = detectionSensor.Rays;
}

private void FixedUpdate()
{
    for (var i = 0; i < _sensorRays.Length; i++)
    {
        var ray = _sensorRays[i];
        if (Physics.Raycast(ray.Ray))
        {
            Debug.Log("Hit!");
        }
    }
}
```

### Unity Package

You can add the code directly to the project:

1. Clone the repo or download the latest release.
2. Add the **UnityDetectionSensor** folder to your Unity project or import the .unitypackage


Contact : sefa@muveso.com



