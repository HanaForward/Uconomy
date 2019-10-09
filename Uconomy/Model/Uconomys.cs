using SqlSugar;

namespace fr34kyn01535.Uconomy.Model
{
    class Uconomys
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public uint Id { get; set; }

        public uint player { get; set; }
        
        public decimal balance { get; set; }

        public Uconomys() { }


        public Uconomys(uint player) {
            this.player = player;

        }
    }
}
