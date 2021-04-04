using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace JsonParserGTIN 
{
    /// <summary>
    /// Класс для экспорта данных в файл
    /// </summary>
    public class Export
    {
        /// <summary>
        /// Метод, который экспортирует данные в фалй
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="results">Коллекция данных</param>
        public static void export(string path, List<ResultObject> results) 
        {
            try
            {
                string result = "GTIN | innProducer | isEMITTED | isINTRODUCED (В ОБОРОТЕ) | isWRITTEN_OFF (СПИСАН) | isRETIRED (ВЫБЫЛ ИЗ ОБОРОТА) | emissionType\n";
                foreach (var item in results)
                {
                    result += item.ToString();
                }
                File.WriteAllText(path, result);
            }
            catch
            {
                MessageBox.Show("Не удалось экспортировать");
            }
            
        }
    }
}
