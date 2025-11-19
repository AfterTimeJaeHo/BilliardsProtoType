using UnityEngine;

namespace Waving.MyTinyStreamer.Common
{
    public static class Define
    {
        public static Vector3 DefaultCameraPos = new Vector3(0, 0, -9.42f);
        public static float TextSpeedFactor = 1;
        public static float TargetResolutionX = 1920;
        public static float TargetResolutionY = 1080;
        public static float ShrinkLayoutX = 1280;
        public static float ShrinkLayoutY = 720;
        public static float ExpandLayoutX = 1920;
        public static float ExpandLayoutY = 1040;
        public static float DefaultCharacterHeight = -3.5f;
        public static float DefaultAutoInterval = 2.5f;
        public static string FanGageConditionKey = "FanGage";
        public static string FanGageTextPrefix = "팬심: ";
        public static string GuestUserName = "Guest21431";
        public static string AggroKey = "AggroSelect";
        public static string HeroineKoName = "히로인";
        public static string DefaultUserName = "나";
    }
    
    public enum StockName
    {
        Tesla,
        Outel,
        Meflix
    }
}