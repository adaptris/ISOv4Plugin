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
			if (isCropNutritionProduct(jo))
			{
				return jo.ToObject<CropNutritionProduct>(serializer);
			}
			if (isCropProtectionProduct(jo))
			{
				return jo.ToObject<CropProtectionProduct>(serializer);
			}
			if (isCropVarietyProduct(jo))
			{
				return jo.ToObject<CropVarietyProduct>(serializer);
			}
			if (isMixProduct(jo))
			{
				return jo.ToObject<MixProduct>(serializer);
			}
			if (isHarvestedCommodityProduct(jo))
			{
				return jo.ToObject<HarvestedCommodityProduct>(serializer);
			}
			return jo.ToObject<GenericProduct>(serializer);
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}

		private bool isCropNutritionProduct(JObject jo)
		{
			return jo["IsManure"] != null;
		}

		private bool isCropProtectionProduct(JObject jo)
		{
			return jo["Biological"] != null && jo["Organophosphate"] != null && jo["Carbamate"] != null;
		}

		private bool isCropVarietyProduct(JObject jo)
		{
			return jo["CropId"] != null && jo["TraitIds"] != null && jo["GeneticallyEnhanced"] != null;
		}

		private bool isMixProduct(JObject jo)
		{
			return jo["TotalQuantity"] != null && jo["IsTemporary"] != null && jo["IsHotMix"] != null;
		}

		private bool isHarvestedCommodityProduct(JObject jo)
		{
			return jo["CropId"] != null;
		}
	}
}
