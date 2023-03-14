using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(menuName = "dataEditor/EBlockModle")]
[JsonObject(MemberSerialization.OptIn)]
public class SC_Entity_BlockModel : ScriptableObject
{
    [JsonProperty]
    public Entity_BlockModel model = new Entity_BlockModel();
}
