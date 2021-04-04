

namespace JsonParserGTIN
{
    /// <summary>
    /// Класс, в котором содержится один код маркировки
    /// </summary>
    public class Kiz
    {
        #region Поля
        /// <summary>
        /// ИНН владельца кода маркировки
        /// </summary>
        public string producerId { get; set; }
        /// <summary>
        /// Статус кода маркировки
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// Идентификатор кода маркировки
        /// </summary>
        public string gtin { get; set; }
        /// <summary>
        /// Статус эмиссии
        /// </summary>
        public int emissionType { get; set; }
        #endregion
        #region Конструктор
        public Kiz(string producerId, int status, string gtin, int emissionType)
        {
            this.producerId = producerId;
            this.status = status;
            this.gtin = gtin;
            this.emissionType = emissionType;
        }
        #endregion
    }
}
