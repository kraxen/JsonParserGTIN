using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonParserGTIN
{
    /// <summary>
    /// Класс для обработки JSON структуры
    /// </summary>
    public class JsonSerializetion
    {
        /// <summary>
        /// Обработка строки JSON
        /// </summary>
        /// <param name="json">Строка JSON</param>
        /// <returns></returns>
        public static List<Kiz> Parse(string json)
        {
            List<Kiz> kizList = new List<Kiz>();
            try
            {
                var kizes = JObject.Parse(json)["kizes"].ToArray();

                foreach (var item in kizes)
                {
                    if(item["packageType"].ToString() == "LEVEL2")
                    {
                        //Добавление упаковки LEVEL2
                        kizList.Add(new Kiz(
                        item["producerId"]["inn"].ToString(),
                        Convert.ToInt32(item["status"].ToString()),
                        item["gtin"].ToString(),
                        0
                        ));
                    }
                    else
                    {
                        //Добавление остальных КМ
                        kizList.Add(new Kiz(
                        item["producerId"]["inn"].ToString(),
                        Convert.ToInt32(item["status"].ToString()),
                        item["gtin"].ToString(),
                        Convert.ToInt32(item["emissionType"].ToString())
                        ));
                    }
                }
            }
            catch
            {

            }
            return kizList;
        }        
    }
}
