using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AgGateway.ADAPT.ISOv4Plugin.ExportMappers;
using AgGateway.ADAPT.ISOv4Plugin.Writers;
using Newtonsoft.Json;

namespace AgGateway.ADAPT.ISOv4Plugin
{
    public interface IExporter
    {
        XmlWriter Export(ApplicationDataModel.ADM.ApplicationDataModel applicationDataModel, string taskDataPath, TaskDocumentWriter writer);
		string Export(ApplicationDataModel.ADM.ApplicationDataModel applicationDataModel, TaskDocumentWriter writer);
		string Export(ApplicationDataModel.ADM.ApplicationDataModel applicationDataModel);
    }

    public class Exporter : IExporter
    {
        private readonly ITaskMapper _taskMapper;

        public Exporter()
            : this(new TaskMapper())
        {
        }

        public Exporter(ITaskMapper taskMapper)
        {
            _taskMapper = taskMapper;
        }

        public XmlWriter Export(ApplicationDataModel.ADM.ApplicationDataModel applicationDataModel, string taskDataPath, TaskDocumentWriter writer)
        {
            var isoTaskData = writer.Write(taskDataPath, applicationDataModel);

            if (applicationDataModel != null)
            {
            
                var numberOfExistingTasks = GetNumberOfExistingTasks(isoTaskData, writer);
                var tasks = applicationDataModel.Documents == null
                    ? null
                    : _taskMapper.Map(applicationDataModel.Documents.LoggedData, applicationDataModel.Catalog,
                        taskDataPath, numberOfExistingTasks, writer, false);
                if (tasks != null)
                {
                    var taskList = tasks.ToList();
                    taskList.ForEach(t => t.WriteXML(isoTaskData));
                }
            }

            //Close the root element with </ISO11783_TaskData>
            isoTaskData.WriteEndElement();
            isoTaskData.Close();
            return isoTaskData;
        }

		public string Export(ApplicationDataModel.ADM.ApplicationDataModel applicationDataModel, TaskDocumentWriter writer)
		{
			var isoTaskData = writer.Write(applicationDataModel);

			if (applicationDataModel != null)
			{
				var numberOfExistingTasks = GetNumberOfExistingTasks(isoTaskData, writer);
				var tasks = applicationDataModel.Documents == null
					? null
					: _taskMapper.Map(applicationDataModel.Documents.LoggedData, applicationDataModel.Catalog, "miscellaneous", numberOfExistingTasks, writer, false);
				if (tasks != null)
				{
					var taskList = tasks.ToList();
					taskList.ForEach(t => t.WriteXML(isoTaskData));
				}
			}

			//Close the root element with </ISO11783_TaskData>
			isoTaskData.WriteEndElement();
			isoTaskData.Close();
			return Encoding.UTF8.GetString(writer.XmlStream.ToArray());
		}

		public string Export(ApplicationDataModel.ADM.ApplicationDataModel applicationDataModel)
		{
			if (applicationDataModel == null)
			{
				return null;
			}
			return JsonConvert.SerializeObject(applicationDataModel, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Objects,
				TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
			});
		}

        private static int GetNumberOfExistingTasks(XmlWriter data, TaskDocumentWriter isoTaskData)
        {
            data.Flush();
            var xml = Encoding.UTF8.GetString(isoTaskData.XmlStream.ToArray());
            if(!xml.EndsWith(">"))
                xml += ">";
            xml += "</ISO11783_TaskData>";
            var xDocument = XDocument.Parse(xml);
            return xDocument.Root.Descendants("TSK").Count();
        }
    }
}