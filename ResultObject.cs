using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonParserGTIN
{
    /// <summary>
    /// Класс, описывающий GTIN
    /// </summary>
    public class ResultObject
    {
        public string gtin { get; set; }
        public string innProducer { get; set; }
        /// <summary>
        /// Эмитирован
        /// </summary>
        public bool isEMITTED { get; set; }
        /// <summary>
        /// В обороте
        /// </summary>
        public bool isINTRODUCED { get; set; }
        /// <summary>
        /// Выбыл из оборота
        /// </summary>
        public bool isRETIRED { get; set; }
        /// <summary>
        /// Списан
        /// </summary>
        public bool isWRITTEN_OFF { get; set; }

        public List<int> emissionType = new List<int>();
        /// <summary>
        /// Коллекция названий статусов
        /// </summary>
        string[] statusNameArr =
        {
            "EMITTED – Эмитирован. Выпущен",
            "APPLIED – Эмитирован. Получен",
            "INTRODUCED – В обороте",
            "WRITTEN_OFF – Списан",
            "RETIRED – Выбыл",
            "DISAGGREGATION – Расформирован"
        };
        /// <summary>
        /// Коллекция названий статусов эмиссии
        /// </summary>
        string[] emissionNameArr =
        {
            "Упаковка LEVEL2",
            "LOCAL - Произведен в РФ",
            "FOREIGN - Ввезен в РФ",
            "REMAINS - Остатки",
            "CROSSBORDER - Ввезен в РФ из стран ЕАЭС",
            "REMARK - Перемаркировка",
            "COMMISSION - Принят на комиссию от физ.лица"
        };
        /// <summary>
        /// Конструктор GTIN без КМ
        /// </summary>
        /// <param name="gtin">Номер GTIN</param>
        public ResultObject(string gtin)
        {
            this.gtin = gtin;
            this.innProducer = "Нет КМ";
        }
        /// <summary>
        /// Конструктор GTIN по КМ
        /// </summary>
        /// <param name="kiz">КМ</param>
        public ResultObject(Kiz kiz)
        {
            this.gtin = kiz.gtin;
            this.innProducer = kiz.producerId;
            this.emissionType.Add(kiz.emissionType);
            this.isEMITTED = false;
            this.isINTRODUCED = false;
            this.isRETIRED = false;
            this.isWRITTEN_OFF = false;
            if ((kiz.status == 0) || ((kiz.status == 1)))
            {
                this.isEMITTED = true;
            }else if(kiz.status == 2)
            {
                this.isINTRODUCED = true;
            }else if(kiz.status == 3)
            {
                this.isWRITTEN_OFF = true;
            }
            else if (kiz.status == 4)
            {
                this.isRETIRED = true;
            }
        }
        /// <summary>
        /// Переопределение записи GTIN в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "";
            result += $"{this.gtin} | ";
            result += $"{this.innProducer} | ";
            result += $"{this.isEMITTED} | ";
            result += $"{this.isINTRODUCED} | ";
            result += $"{this.isWRITTEN_OFF} | ";
            result += $"{this.isRETIRED} | ";
            foreach (var item in this.emissionType)
            {
                result += $" {this.emissionNameArr[item]},";
            }
            result += "\n";
            return result;
        }
    }
}
