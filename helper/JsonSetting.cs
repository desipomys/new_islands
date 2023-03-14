using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class JsonSetting 
{
    public static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        
        //Formatting = Formatting.Indented,
    };
}
