using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AgGateway.ADAPT.ISOv4Plugin.ExportMappers;
using AgGateway.ADAPT.ISOv4Plugin.Writers;
using Newtonsoft.Json;
using System.IO;

namespace AgGateway.ADAPT.ISOv4Plugin
{
    public interface IExporter
    {
        XmlWriter Export(ApplicationDataModel.ADM.ApplicationDataModel applicationDataModel, string taskDataPath, TaskDocumentWriter writer);
		string Export2(ApplicationDataModel.ADM.ApplicationDataModel applicationDataModel, string taskDataPath, TaskDocumentWriter writer);
		string Export(ApplicationDataModel.ADM.ApplicationDataModel applicationDataModel, TaskDocumentWriter writer);
		string Export(ApplicationDataModel.ADM.ApplicationDataModel applicationDataModel);
    }

    public class Exporter : IExporter
    {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
			XmlWriter isoTaskData = taskDataPath != null ? writer.Write(taskDataPath, applicationDataModel) : writer.Write(applicationDataModel);
			log.Info("Exporting application data model [ " + applicationDataModel + " ] to task data" + (taskDataPath != null ? " path " + taskDataPath : "") + " ...");

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
			log.Info("Exported application data model");
            return isoTaskData;
        }

		public string Export2(ApplicationDataModel.ADM.ApplicationDataModel applicationDataModel, string taskDataPath, TaskDocumentWriter writer)
		{
			log.Info("Exporting application data model [ " + applicationDataModel + " ] to XML ...");
			Export(applicationDataModel, taskDataPath, writer);
			var xml = Encoding.UTF8.GetString(writer.XmlStream.ToArray());
			log.Info("Exported application data model to XML");
			return xml;
		}


		public string Export(ApplicationDataModel.ADM.ApplicationDataModel applicationDataModel, TaskDocumentWriter writer)
		{
			log.Info("Exporting application data model [ " + applicationDataModel + " ] to XML ...");
			Export(applicationDataModel, null, writer);
			var xml = Encoding.UTF8.GetString(writer.XmlStream.ToArray());
			log.Info("Exported application data model to XML");
			return xml;
		}

		public string Export(ApplicationDataModel.ADM.ApplicationDataModel applicationDataModel)
		{
			log.Info("Exporting application data model [ " + applicationDataModel + " ] to JSON ...");
			if (applicationDataModel == null)
			{
				log.Warn("Application data model is NULL");
				return null;
			}

			var settings = new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};

			// serialize JSON directly to a file
			using (StreamWriter file = File.CreateText(@"export.json"))
			{
				JsonSerializer serializer = JsonSerializer.Create(settings);
				serializer.Serialize(file, applicationDataModel);
			}

			var j = JsonConvert.SerializeObject(applicationDataModel, null/*Newtonsoft.Json.Formatting.Indented*/, settings);
			log.Info("Exported application data model to JSON");
			log.Debug("JSON: " + j);
			return j;
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