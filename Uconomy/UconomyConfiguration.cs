using Rocket.API;

namespace fr34kyn01535.Uconomy
{
    public class UconomyConfiguration : IRocketPluginConfiguration
    {

        public decimal InitialBalance;
        public string MoneySymbol;
        public string MoneyName;

        public string MessageColor;

        public void LoadDefaults()
        {
            InitialBalance = 30;
            MoneySymbol = "‡";
            MoneyName = "金币";
            MessageColor = "blue";
        }
    }
}