using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSRO.Common
{
    public interface ICsvExporter
    {
        byte[] ExportEventsToCsv<T>(List<T> listToExport);
    }

    public class CsvExporter : ICsvExporter
    {
        public byte[] ExportEventsToCsv<T>(List<T> listToExport)
        {
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                using var csvWriter = new CsvWriter(streamWriter);
                csvWriter.WriteRecords(listToExport);
            }

            return memoryStream.ToArray();
        }
    }
}
