using AgGateway.ADAPT.ApplicationDataModel.Products;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AgGateway.ADAPT.ISOv4Plugin.ImportMappers
{
	class IngredientJsonMapper : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Ingredient);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jo = JObject.Load(reader);
			if (jo["IngredientCode"] != null)
			{
				return jo.ToObject<CropNutritionIngredient>(serializer);
			}
			else
			{
				return jo.ToObject<ActiveIngredient>(serializer);
			}
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}
	}
}
