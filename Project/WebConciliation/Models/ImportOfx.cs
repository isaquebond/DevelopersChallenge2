using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace WebConciliation.Models
{
    public class ImportOfx
    {
        public static XElement toXElement(string pathToOfxFile)
        {
            if (!File.Exists(pathToOfxFile))
            {
                throw new FileNotFoundException();
            }

            //USE LINQ TO GET ONLY THE LINES
            var tags = from line in File.ReadAllLines(pathToOfxFile)
                       where line.Contains("<STMTTRN>") ||
                       line.Contains("<TRNTYPE>") ||
                       line.Contains("<DTPOSTED>") ||
                       line.Contains("<TRNAMT>") ||
                       line.Contains("<FITID>") ||
                       line.Contains("<CHECKNUM>") ||
                       line.Contains("<MEMO>")
                       select line;

            XElement el = new XElement("root");
            XElement son = null;
            
            foreach (var l in tags)
            {
                if (l.IndexOf("<STMTTRN>") != -1)
                {
                    son = new XElement("STMTTRN");
                    el.Add(son);
                    continue;
                }

                var tagName = getTagName(l);
                var elSon = new XElement(tagName);
                elSon.Value = getTagValue(l, tagName);
                son.Add(elSon);
            }

            return el;
        }
        /// <summary>
        /// Get the Tag name to create an Xelement
        /// </summary>
        /// <param name="line">One line from the file</param>
        /// <returns></returns>
        private static string getTagName(string line)
        {
            int pos_init = line.IndexOf("<") + 1;
            int pos_end = line.IndexOf(">");
            pos_end = pos_end - pos_init;
            return line.Substring(pos_init, pos_end);
        }
        /// <summary>
        /// Get the value of the tag to put on the Xelement
        /// </summary>
        /// <param name="line">The line</param>
        /// <returns></returns>
        private static string getTagValue(string line, string tagName)
        {
            int pos_init = line.IndexOf(">") + 1;
            string retValue = line.Substring(pos_init).Trim();

            if (tagName == "TRNAMT")
            {
                retValue = retValue.Replace("-", "").Replace(".", ",");
            }

            if (tagName == "DTPOSTED")
            {
                retValue = DateFormat(retValue.Substring(0, 8));
            }

            return retValue;
        }

        private static string DateFormat(string Value)
        {
            string year = Value.Substring(0, 4);
            string month = Value.Substring(4, 2);
            string day = Value.Substring(6, 2);

            string _restult = day + "/" + month + "/" + year;

            return _restult;
        }

    }
}