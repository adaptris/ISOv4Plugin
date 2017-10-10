using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AgGateway.ADAPT.ISOv4Plugin.ImportMappers
{
	class DeviceElementConfigurationJsonMapper : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DeviceElementConfiguration);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jo = JObject.Load(reader);
			if (jo["ImplementLength"] != null)
			{
				return jo.ToObject<ImplementConfiguration>(serializer);
			}
			else if (jo["OriginAxleLocation"] != null)
			{
				return jo.ToObject<MachineConfiguration>(serializer);
			}
			else if (jo["SectionWidth"] != null)
			{
				return jo.ToObject<SectionConfiguration>(serializer);
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
