using SqlSugar;

namespace fr34kyn01535.Uconomy.Model
{
    public class Uconomys
    {
        [SugarColumn(IsPrimaryKey = true)]
        public uint player { get; set; }
        public decimal balance { get; set; }
        public Uconomys() { }
        public Uconomys(uint player)
        {
            this.player = player;
            balance = Uconomy.Instance.Configuration.Instance.InitialBalance;
        }
    }
}
