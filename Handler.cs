using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace JsonParserGTIN
{
    /// <summary>
    /// Класс для обработки коллекции
    /// </summary>
    public class Handler
    {
        /// <summary>
        /// Метод обработки коллекции КМ в коллекцию GTIN с наличием КМ
        /// </summary>
        /// <param name="kizs">Коллекция КМ</param>
        /// <param name="gtins">Коллекция GTIN</param>
        /// <returns></returns>
        public static List<ResultObject> hendler(List<Kiz> kizs, List<string> gtins)
        {
            try
            {
                List<ResultObject> result = new List<ResultObject>();

                //Метод, получающий GTIN с наличием КМ, вместо КМ
                ResultObject toResultObject(Kiz kiz)
                {
                    ResultObject result1 = new ResultObject(kiz);
                    return result1;
                }

                try
                {
                    //Добавление первого объекта
                    result.Add(new ResultObject(kizs[0]));
                    foreach (var item in kizs)
                    {
                        ResultObject temp = toResultObject(item);
                        // Если коллекция содержит элемент
                        if (result.Any(e =>
                        (e.gtin == temp.gtin) &&
                        (e.innProducer == temp.innProducer)
                        ))
                        //То:
                        {
                            //Находим индекс элемента
                            int tempIndex = result.FindIndex(e =>
                            (e.gtin == temp.gtin) &&
                            (e.innProducer == temp.innProducer)
                            );
                            //Если тип эмиссии найденного элемента содержит тип эмиссии нового элемента
                            if (result[tempIndex].emissionType.Any(e => e == temp.emissionType[0]))
                            {
                                //То мы ничего не делаем
                            }
                            else
                            {
                                //Иначе мы добавляем тип эмиссии элементу
                                result[tempIndex].emissionType.Add(temp.emissionType[0]);
                            }

                            //Проверка на наличие КМ в определенном статусе
                            if (temp.isEMITTED == true)
                            {
                                result[tempIndex].isEMITTED = true;
                            }
                            if (temp.isINTRODUCED == true)
                            {
                                result[tempIndex].isINTRODUCED = true;
                            }
                            if (temp.isRETIRED == true)
                            {
                                result[tempIndex].isRETIRED = true;
                            }
                            if (temp.isWRITTEN_OFF == true)
                            {
                                result[tempIndex].isWRITTEN_OFF = true;
                            }
                        }
                        else
                        {
                            //Иначе мы добавляем элемент в коллекцию
                            result.Add(new ResultObject(item));
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Не удалось обработать ответ метода foiv или нет КМ");
                }
                //Сортировка GTIN по идентификатору
                result = result.OrderBy(x => x.gtin).ToList();

                //Добавление GTIN без КМ
                foreach (var item in gtins)
                {
                    if (!result.Any(e => e.gtin == item))
                    {
                        result.Add(new ResultObject(item));
                    }
                }

                return result;
            }
            catch
            {
                MessageBox.Show("Не удалось обработать ответ метода foiv");
                return null;
            }
        }
            
    }
}
