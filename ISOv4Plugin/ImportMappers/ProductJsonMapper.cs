using AgGateway.ADAPT.ApplicationDataModel.Products;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AgGateway.ADAPT.ISOv4Plugin.ImportMappers
{
	class ProductJsonMapper : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Product);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jo = JObject.Load(reader);
			string type = jo["$type"].Value<string>();
			switch (type)
			{
				case "AgGateway.ADAPT.ApplicationDataModel.Products.CropNutritionProduct, AgGateway.ADAPT.ApplicationDataModel":
					return jo.ToObject<CropNutritionProduct>(serializer);
				case "AgGateway.ADAPT.ApplicationDataModel.Products.CropProtectionProduct, AgGateway.ADAPT.ApplicationDataModel":
					return jo.ToObject<CropProtectionProduct>(serializer);
				case "AgGateway.ADAPT.ApplicationDataModel.Products.CropVarietyProduct, AgGateway.ADAPT.ApplicationDataModel":
					return jo.ToObject<CropVarietyProduct>(serializer);
				case "AgGateway.ADAPT.ApplicationDataModel.Products.HarvestedCommodityProduct, AgGateway.ADAPT.ApplicationDataModel":
					return jo.ToObject<HarvestedCommodityProduct>(serializer);
				case "AgGateway.ADAPT.ApplicationDataModel.Products.GenericProduct, AgGateway.ADAPT.ApplicationDataModel":
					return jo.ToObject<GenericProduct>(serializer);
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
