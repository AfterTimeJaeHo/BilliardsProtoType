using System;
using UnityEngine;

namespace Aftertime.SecretSome.Common
{
    [Serializable]
    public class JsonVector2
    {
        public float x;
        public float y;

        public JsonVector2()
        {
            
        }

        public JsonVector2(Vector2 vector)
        {
            x = vector.x;
            y = vector.y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }
    }
    
    [Serializable]
    public class JsonVector3 : JsonVector2
    {
        public float z;

        public JsonVector3()
        {
            
        }

        public JsonVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
        
        public static implicit operator Vector3(JsonVector3 v) => new Vector3(v.x, v.y, v.z);

        public static implicit operator JsonVector3(Vector3 v) => new JsonVector3(v);

        public override int GetHashCode() => (x, y, z).GetHashCode();
    }   
}
