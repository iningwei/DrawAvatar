using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerIndexConfig
{
    public static int DefaultLayer = LayerMask.NameToLayer(LayerNameConfig.DefaultLayer);
    public static int WorldUILayer = LayerMask.NameToLayer(LayerNameConfig.WorldUILayer); 
    public static int UILayer = LayerMask.NameToLayer(LayerNameConfig.UILayer);
    
    
    public static int GroundLayer = LayerMask.NameToLayer(LayerNameConfig.GroundLayer);
    public static int ObstacleLayer = LayerMask.NameToLayer(LayerNameConfig.ObstacleLayer);
    public static int ShadowCameraLayer = LayerMask.NameToLayer(LayerNameConfig.ShadowCameraLayer);

    public static int BuildingLayer = LayerMask.NameToLayer(LayerNameConfig.BuildingLayer);
}

public class LayerNameConfig
{
    public static string DefaultLayer = "Default";
    public static string WorldUILayer = "WorldUI";
    public static string UILayer = "UI";
    
   
    public static string GroundLayer = "Ground";
    public static string ObstacleLayer = "Obstacle"; 
    public static string ShadowCameraLayer = "ShadowCamera";

    public static string BuildingLayer = "Building";
   
}
