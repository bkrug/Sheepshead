using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Linq;

namespace Sheeshead.WebApi.Json
{
    //Got general idea of code from here:
    //https://stackoverflow.com/questions/37282014/webapi-custom-model-binding-of-complex-abstract-object
    public class JsonBodyModelBinder<T> : IModelBinder
    {
        private const StringComparison noCase = StringComparison.OrdinalIgnoreCase;

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(T))
            {
                return false;
            }

            try
            {
                //First, try to get the object from a query parameter in the Uri.
                var queryParameter = actionContext.ActionArguments.FirstOrDefault(kvp => kvp.Key.Equals(bindingContext.ModelName, noCase));
                if (queryParameter.Key != null)
                {
                    bindingContext.Model = DeserializeObjectFromJson(queryParameter.Value.ToString());
                    return true;
                }
                //Second, try to get it from the body content.
                else
                {
                    var content = actionContext.Request.Content;
                    var json = content.ReadAsStringAsync().Result;
                    bindingContext.Model = DeserializeObjectFromJson(json);
                    return true;
                }
            }
            catch (JsonException exception)
            {
                bindingContext.ModelState.AddModelError("JsonDeserializationException", exception);
                return false;
            }
        }

        private static T DeserializeObjectFromJson(string json)
        {
            var binder = new TypeNameSerializationBinder("");

            var obj = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Binder = binder
            });
            return obj;
        }
    }

    public class TypeNameSerializationBinder : SerializationBinder
    {
        public string TypeFormat { get; private set; }

        public TypeNameSerializationBinder(string typeFormat)
        {
            TypeFormat = typeFormat;
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            string resolvedTypeName = string.Format(TypeFormat, typeName);

            return Type.GetType(resolvedTypeName, true);
        }
    }
}