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
			string type = jo["$type"].Value<string>();
			switch (type)
			{
				case "AgGateway.ADAPT.ApplicationDataModel.Ingredient.CropNutritionIngredient, AgGateway.ADAPT.ApplicationDataModel":
					return jo.ToObject<CropNutritionIngredient>(serializer);
				case "AgGateway.ADAPT.ApplicationDataModel.Ingredient.ActiveIngredient, AgGateway.ADAPT.ApplicationDataModel":
					return jo.ToObject<ActiveIngredient>(serializer);
			}
			return null;
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
