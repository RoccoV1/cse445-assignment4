using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;

namespace ConsoleApp1
{
    public class Submission
    {
        public static string xmlURL = "https://raw.githubusercontent.com/RoccoV1/cse445-assignment4/main/NationalParks.xml";
        public static string xmlErrorURL = "https://raw.githubusercontent.com/RoccoV1/cse445-assignment4/main/NationalParksErrors.xml";
        public static string xsdURL = "https://raw.githubusercontent.com/RoccoV1/cse445-assignment4/main/NationalParks.xsd";

        public static void Main(string[] args)
        {
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        public static string Verification(string xmlUrl, string xsdUrl)
        {
            string xmlText = DownloadContent(xmlUrl);
            string xsdText = DownloadContent(xsdUrl);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            StringWriter errors = new StringWriter();
            bool hasErrors = false;

            settings.ValidationEventHandler += delegate(object sender, ValidationEventArgs e)
            {
                hasErrors = true;
                errors.WriteLine(e.Severity.ToString() + ": " + e.Message);
            };

            try
            {
                using (StringReader schemaString = new StringReader(xsdText))
                using (XmlReader schemaReader = XmlReader.Create(schemaString))
                {
                    settings.Schemas.Add(null, schemaReader);
                }

                using (StringReader xmlString = new StringReader(xmlText))
                using (XmlReader reader = XmlReader.Create(xmlString, settings))
                {
                    while (reader.Read()) { }
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errors.WriteLine("Error: " + ex.Message);
            }

            if (!hasErrors)
            {
                return "No errors are found";
            }

            return errors.ToString().Trim();
        }

        public static string Xml2Json(string xmlUrl)
        {
            string xmlText = DownloadContent(xmlUrl);
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlText);
            return JsonConvert.SerializeXmlNode(document, Formatting.Indented);
        }

        private static string DownloadContent(string url)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }
}
