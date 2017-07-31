//using Newtonsoft.Json;
//using Sheepshead.Models;
//using System;

//namespace Sheeshead.WebApi
//{
//    public class GameStartConverter : JsonConverter
//    {
//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            //var username = (GameStartModel)value;

//            writer.WriteStartObject();
//            serializer.Serialize(writer, "I'm in a place.");
//            writer.WriteEndObject();
//        }

//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            ////Variables to be set along with sensing variables
//            //string username = null;
//            //var gotName = false;

//            //Read the properties
//            while (reader.Read())
//            {
//                //    if (reader.TokenType != JsonToken.PropertyName)
//                //    {
//                //        break;
//                //    }

//                //    var propertyName = (string)reader.Value;
//                //    if (!reader.Read())
//                //    {
//                //        continue;
//                //    }

//                //    //Set the group
//                //    if (propertyName.Equals("UserName", StringComparison.OrdinalIgnoreCase))
//                //    {
//                //        username = serializer.Deserialize<string>(reader);
//                //        gotName = true;
//                //    }
//            }

//            //if (!gotName)
//            //{
//            //    throw new InvalidDataException("A username must be present.");
//            //}

//            return new GameStartModel() { HumanCount = -1 };
//        }

//        public override bool CanConvert(Type objectType)
//        {
//            return objectType == typeof(GameStartModel);
//        }
//    }
//}