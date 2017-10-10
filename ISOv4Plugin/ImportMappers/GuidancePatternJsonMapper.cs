using AgGateway.ADAPT.ApplicationDataModel.Guidance;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AgGateway.ADAPT.ISOv4Plugin.ImportMappers
{
	class GuidancePatternJsonMapper : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(GuidancePattern);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jo = JObject.Load(reader);
			if (isAbCurve(jo))
			{
				return jo.ToObject<AbCurve>(serializer);
			}
			if (isAbLine(jo))
			{
				return jo.ToObject<AbLine>(serializer);
			}
			if (isAPlus(jo))
			{
				return jo.ToObject<APlus>(serializer);
			}
			if (isMultiAbLine(jo))
			{
				return jo.ToObject<MultiAbLine>(serializer);
			}
			if (isPivotGuidancePattern(jo))
			{
				return jo.ToObject<PivotGuidancePattern>(serializer);
			}
			return jo.ToObject<Spiral>(serializer);
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}

		private bool isAbCurve(JObject jo)
		{
			return jo["NumberOfSegments"] != null;
		}

		private bool isAbLine(JObject jo)
		{
			return jo["EastShiftComponent"] != null && jo["NorthShiftComponent"] != null;
		}

		private bool isAPlus(JObject jo)
		{
			return jo["Point"] != null;
		}

		private bool isMultiAbLine(JObject jo)
		{
			return jo["AbLines"] != null;
		}

		private bool isPivotGuidancePattern(JObject jo)
		{
			return jo["StartPoint"] != null && jo["EndPoint"] != null && jo["Center"] != null;
		}
	}
}
