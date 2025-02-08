using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DataNRO.CLI
{
    public class IgnoreMapTemplateResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.DeclaringType == typeof(GameData) && property.PropertyName == nameof(GameData.Map.mapTemplate))
                property.ShouldSerialize = instance => false;
            return property;
        }
    }
}
