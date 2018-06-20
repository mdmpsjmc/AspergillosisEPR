using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.ClinicLetters
{
    public class MsWordFormatConverter
    {
        private string _filePath;
        private bool _deleteOriginalFile;

        public MsWordFormatConverter(string filePath, bool deleteOriginalFile)
        {
            _filePath = filePath;
            _deleteOriginalFile = deleteOriginalFile;
        }

        public string ConvertDocToDocx()
        {
            Application word = new Application();
            var sourceFile = new FileInfo(_filePath);
            string newFileName = sourceFile.FullName.Replace(".doc", ".docx");
            if (_filePath.ToLower().EndsWith(".doc"))
            {

                var document = word.Documents.Open(sourceFile.FullName);
                document.SaveAs2(newFileName, WdSaveFormat.wdFormatXMLDocument,
                                 CompatibilityMode: WdCompatibilityMode.wdWord2010);

                word.ActiveDocument.Close();
                word.Quit();
            }
            if (_deleteOriginalFile) File.Delete(_filePath);
            return newFileName;
        }
    }
}
