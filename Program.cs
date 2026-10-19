namespace OOP_I
{
    public enum PenzFelvetelEredmeny
    {
        Sikeres,
        ErvenytelenOsszeg,
        LimitTullepve,
        NincsElegFedezet
    }

    public class Bankszamla
    {
        private decimal egyenleg;
        private decimal felveteliLimit;
        public string Tulajdonos { get; }
        public decimal Egyenleg => egyenleg;

        public decimal FelveteliLimit
        {
            get => felveteliLimit;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("A felvételi limitnek pozitívnak kell lenni");
                }
                felveteliLimit = value;
            }
        }

        public Bankszamla(string tulajdonos, decimal kezdoegyenleg, decimal limit)
        {
            if (string.IsNullOrWhiteSpace(tulajdonos))
            {
                throw new ArgumentException("A tulajdonos neve nem lehet üres", nameof(tulajdonos));
            }

            if (kezdoegyenleg < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(kezdoegyenleg), "A kezdőegyenleg nem lehet negatív");
            }
            Tulajdonos = tulajdonos.Trim();
            egyenleg = kezdoegyenleg;
            FelveteliLimit = limit;
        }

        protected void Jovair(decimal osszeg)
        {
            if (osszeg <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(osszeg), "A jóváírandó összegnek pozitívnak kell lennie");
            }
            egyenleg += osszeg;
        }


        public virtual void Befizet(decimal osszeg)
        {
            Jovair(osszeg);
        }

        protected PenzFelvetelEredmeny MegprobalMegterhelni(decimal osszeg)
        {
            if (osszeg <= 0)
                return PenzFelvetelEredmeny.ErvenytelenOsszeg;
            if (osszeg > egyenleg)
                return PenzFelvetelEredmeny.NincsElegFedezet;
            egyenleg -= osszeg;
            return PenzFelvetelEredmeny.Sikeres;
        }

        public virtual PenzFelvetelEredmeny Felvesz(decimal osszeg)
        {
            if (osszeg <= 0)
                return PenzFelvetelEredmeny.ErvenytelenOsszeg;
            if (osszeg > FelveteliLimit)
                return PenzFelvetelEredmeny.LimitTullepve;
            return MegprobalMegterhelni(osszeg);
        }

        public override string ToString()
        {
            return $"{Tulajdonos} | Egyenleg:{Egyenleg:N2} Ft | Limit: {FelveteliLimit:N2} Ft";
        }
        public class DiakSzamla : Bankszamla
        {
            public string Iskola { get; }

            public DiakSzamla(string tulajdonos, decimal kezdoegyenleg, string iskola)
                : base(tulajdonos, kezdoegyenleg, 20000)
            {
                if (string.IsNullOrWhiteSpace(iskola))
                    throw new ArgumentException("Az iskola neve nem lehet üres.", nameof(iskola));
                Iskola = iskola.Trim();
            }

            public override string ToString()
            {
                return $"{base.ToString()} | Iskola: {Iskola}";
            }
        }
        internal class Program
        {
            static void Main(string[] args)
            {
                AlapOsztalyKiprobalasa();
            }
        }
        private static void FelvetelBemutatasa(Bankszamla szamla, decimal osszeg)
        {
            decimal eredetiEgyenleg = szamla.Egyenleg;
            PenzFelvetelEredmeny eredmeny = szamla.Felvesz(osszeg);
            Console.WriteLine($"{szamla.Tulajdonos}, felvétel: {osszeg:N2} Ft {EredmenyUzenet(eredmeny)} | Egyenleg{eredetiEgyenleg:N2} -> {szamla.Egyenleg:N2} Ft");
        }



        private static string EredmenyUzenet(PenzFelvetelEredmeny eredmeny)
        {
            return eredmeny switch
            {
                PenzFelvetelEredmeny.Sikeres => "Sikeres pénzfelvétel.",
                PenzFelvetelEredmeny.ErvenytelenOsszeg => "Az összegnek pozitívnak kell lennie.",
                PenzFelvetelEredmeny.LimitTullepve => "Az összeg meghaladja a felvételi limitet.",
                PenzFelvetelEredmeny.NincsElegFedezet => "Nincs elég fedezet a teljes terhelésre.",
                _ => throw new ArgumentOutOfRangeException(nameof(eredmeny))
            };

        }

        private static void AlapOsztalyKiprobalasa()
        {
            Console.WriteLine("Alaposztály kipróbálása");
            Bankszamla szamla = new Bankszamla("Kis Anna", 50000m, 30000m);
            Console.WriteLine(szamla);
            szamla.Befizet(10000m);
            Console.WriteLine($"Befizetés után: , {szamla.Egyenleg:N2} Ft");
            FelvetelBemutatasa(szamla, 20000m);
            FelvetelBemutatasa(szamla, 35000m);
            FelvetelBemutatasa(szamla, 0m);
            szamla.FelveteliLimit = 100000m;
            FelvetelBemutatasa(szamla, 50000m);

            try
            {
                szamla.Befizet(-1000m);
            }
            catch (ArgumentOutOfRangeException hiba)
            {
                Console.WriteLine($"Hibás befizetés: {hiba.Message} ");
            }

            try
            {
                szamla.FelveteliLimit = 0m;
            }
            catch (ArgumentOutOfRangeException hiba)
            {
                Console.WriteLine($"Hibás limit: {hiba.Message}");
            }

            Console.WriteLine($" hibás műveletek után: {szamla}");





        }


    }

}
