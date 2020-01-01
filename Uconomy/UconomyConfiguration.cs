using Rocket.API;
using System.Xml.Serialization;

namespace fr34kyn01535.Uconomy
{
    public class UconomyConfiguration : IRocketPluginConfiguration
    {
        public string TableName;

        [XmlIgnore]
        public string DatabaseAddress;
        [XmlIgnore]
        public string DatabaseUsername;
        [XmlIgnore]
        public string DatabasePassword;
        [XmlIgnore]
        public string DatabaseName;
        [XmlIgnore]
        public string DatabaseTableName;
        [XmlIgnore]
        public int DatabasePort;

        public decimal InitialBalance;
        public string MoneySymbol;
        public string MoneyName;
        public string MessageColor;

        public void LoadDefaults()
        {
            TableName = "Uconomys";
            InitialBalance = 30;
            MoneySymbol = "‡";
            MoneyName = "金币";
            MessageColor = "blue";
        }
    }
}