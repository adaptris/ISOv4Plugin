﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.ISOv4Plugin.Extensions;
using AgGateway.ADAPT.ISOv4Plugin.ImportMappers.LogMappers.XmlReaders;
using AgGateway.ADAPT.ISOv4Plugin.Models;
using AgGateway.ADAPT.ISOv4Plugin.Writers;

namespace AgGateway.ADAPT.ISOv4Plugin.ExportMappers
{
    public interface ITlgMapper
    {
        IEnumerable<TLG> Map(IEnumerable<OperationData> operationDatas, string taskDataPath, TaskDocumentWriter taskDocumentWriter);
    }

    public class TlgMapper : ITlgMapper
    {
        private readonly IXmlReader _xmlReader;
        private readonly ITimHeaderMapper _timHeaderMapper;
        private readonly IBinaryWriter _binaryWriter;

        public TlgMapper() : this(new XmlReader(), new TimHeaderMapper(), new BinaryWriter())
        {
            
        }

        public TlgMapper(IXmlReader xmlReader, ITimHeaderMapper timHeaderMapper, IBinaryWriter binaryWriter)
        {
            _xmlReader = xmlReader;
            _timHeaderMapper = timHeaderMapper;
            _binaryWriter = binaryWriter;
        }

        public IEnumerable<TLG> Map(IEnumerable<OperationData> operationDatas, string taskDataPath, TaskDocumentWriter taskDocumentWriter)
        {
            if (operationDatas == null)
                return Enumerable.Empty<TLG>();
            return operationDatas.Select(x => Map(x, taskDataPath, taskDocumentWriter));
        }

        private TLG Map(OperationData operationData, string taskDataPath, TaskDocumentWriter taskDocumentWriter)
        {
            var tlgId = operationData.Id.FindIsoId() ?? "TLG" + operationData.Id.ReferenceId;
            taskDocumentWriter.Ids.Add(tlgId, operationData.Id);

            var tlg = new TLG { A = tlgId};
            var sections = operationData.GetAllSections();
            var meters = sections.SelectMany(x => x.GetWorkingDatas()).ToList();
            var spatialRecords = operationData.GetSpatialRecords != null ? operationData.GetSpatialRecords() : null;

            var timHeader = _timHeaderMapper.Map(meters);
            _xmlReader.WriteTlgXmlData(taskDataPath, tlg.A + ".xml", timHeader);

            var binFilePath = Path.Combine(taskDataPath, tlg.A + ".bin");
            _binaryWriter.Write(binFilePath, meters, spatialRecords);

            return tlg;
        }
    }
}
