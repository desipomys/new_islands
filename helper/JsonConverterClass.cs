using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sirenix.OdinInspector;


public class DatetimeConverter:IsoDateTimeConverter
{
    public DatetimeConverter():base()
    {
        base.DateTimeFormat="yyyy/MM/dd hh:mm:ss";
    }
}


/// <summary>
/// Converts a <see cref="T_Block"/>  
/// </summary>
public class TBlockConverter : Newtonsoft.Json.JsonConverter
{
    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        string ticks = Convert.ToBase64String(((T_Block)value).ToByte()) ;

        

        writer.WriteValue(ticks);

    }

    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing property value of the JSON that is being converted.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {

        var it = (string)reader.Value;
        //Debug.Log(it);
        Byte[] bt=Convert.FromBase64String(it);
        T_Block ans=T_Block.FromByte(bt);
        return ans;
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(T_Block)==objectType;
    }
}

public class BBlockConverter : Newtonsoft.Json.JsonConverter
{
    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        string ticks = Convert.ToBase64String(((B_Block)value).ToByte()) ;

        

        writer.WriteValue(ticks);

    }

    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing property value of the JSON that is being converted.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {

        var it = (string)reader.Value;
        //Debug.Log(it);
        Byte[] bt=Convert.FromBase64String(it);
        B_Block ans=B_Block.FromByte(bt);
        return ans;
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(B_Block)==objectType;
    }
}
