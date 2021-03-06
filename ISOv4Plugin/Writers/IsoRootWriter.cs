﻿using System.Globalization;
using AgGateway.ADAPT.ISOv4Plugin.Models;

namespace AgGateway.ADAPT.ISOv4Plugin.Writers
{
    public class IsoRootWriter
    {
        public static void Write(TaskDocumentWriter writer)
        {
            writer.RootWriter.WriteStartElement("ISO11783_TaskData");
            writer.RootWriter.WriteAttributeString("VersionMajor", "4");
            writer.RootWriter.WriteAttributeString("VersionMinor", "0");
            writer.RootWriter.WriteAttributeString("ManagementSoftwareManufacturer", "AgGateway");
            writer.RootWriter.WriteAttributeString("ManagementSoftwareVersion", "1.0");
            writer.RootWriter.WriteAttributeString("DataTransferOrigin", ((int)ISO11783_TaskDataDataTransferOrigin.Item1).ToString(CultureInfo.InvariantCulture));
            writer.RootWriter.WriteAttributeString("TaskControllerManufacturer", "");
            writer.RootWriter.WriteAttributeString("TaskControllerVersion", "");
            if (writer.DataModel != null)
            {
                if (writer.DataModel.Catalog != null)
                    WriteMetaItems(writer);
            }
        }

        private static void WriteMetaItems(TaskDocumentWriter writer)
        {
            AttachedFileWriter.Write(writer);
            CropWriter.Write(writer);
            CustomerWriter.Write(writer);
            FarmWriter.Write(writer);
            FieldWriter.Write(writer);
            ProductWriter.Write(writer);
            WorkerWriter.Write(writer);
            CommentWriter.Write(writer);
            PrescriptionWriter.Write(writer);
        }
    }
}
