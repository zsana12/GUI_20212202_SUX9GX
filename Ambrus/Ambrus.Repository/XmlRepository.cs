using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrus.Repository
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlRepository : IGameRepository
    {
        private readonly int maxScores = 1000;
        private string filePath;
        private List<Entry> scores = new List<Entry>();

        public XmlRepository()
        {
            var FilePath = "../../../Data";
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            this.filePath = FilePath + "/scores.xml";

            if (File.Exists(this.filePath))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(this.filePath);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(List<Entry>);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        this.scores = (List<Entry>)serializer.Deserialize(reader);
                    }
                }
            }
        }

        public void SaveScore(string playerName, int score)
        {
            this.scores.Add(new Entry()
            {
                Name = playerName,
                Score = score,
            });
            this.scores = this.scores.OrderBy(t => t.Score).Reverse().ToList();

            if (this.scores.Count > this.maxScores)
            {
                this.scores.RemoveAt(this.maxScores);
            }

            XmlDocument xmlDocument = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(this.scores.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, this.scores);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(this.filePath);
            }
        }

        public IEnumerable<Entry> LoadScores()
        {
            return this.scores;
        }
    }

}
