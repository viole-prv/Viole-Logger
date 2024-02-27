using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Viole_Logger_Interface
{
    public class JsonPropertyResolver<T> : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo MemberInfo, MemberSerialization MemberSerialization)
        {
            var Property = base.CreateProperty(MemberInfo, MemberSerialization);

            Property.Ignored = false;

            return Property;
        }
    }
}
