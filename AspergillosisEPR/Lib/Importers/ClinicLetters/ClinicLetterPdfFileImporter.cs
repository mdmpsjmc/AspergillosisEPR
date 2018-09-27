using AspergillosisEPR.Data;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.ClinicLetters
{
    public class ClinicLetterPdfFileImporter : Importer
    {
        public List<string> Lines;
        private string _text;
        private string _fullPath;

        public ClinicLetterPdfFileImporter(FileStream stream,
                                          string fullPath,
                                          AspergillosisContext context)
        {
            _stream = stream;
            _fullPath = fullPath;
            _context = context;
        }

        public string ReadFile()
        {
            _stream.Position = 0;
            _text = ReadPDFText();
            Lines = _text.Split("\n").ToList();
            return _text;
        }

        private string ReadPDFText()
        {
            using (PdfReader reader = new PdfReader(_stream))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }

                return text.ToString();
            }
        }
    }
}
