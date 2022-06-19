internal class Gracz
{
    string imie;
    public int Wynik { get; private set; } 

    Plansza plansza;

    public Gracz(string imie, Plansza plansza)
    {
        this.imie = imie;
        this.plansza = plansza;
        Restart();
    }

    internal void DodajStatek(string pozycja)
    {
        plansza.DodajStatek(pozycja);
    }

    internal void WypiszPozycje()
    {
        Console.WriteLine($"Statki {imie}a znajduja sie na pozycjach: {String.Join(", ", plansza.StatkiGracza)}");
    }

    internal string WylosujPole(Random random, bool usun = true)
    {
        return plansza.WylosujPole(random, usun);
    }

    internal bool CzyTrafiony(string strzalKomputera)
    {
        return plansza.CzyTrafiony(strzalKomputera);
    }

    internal void ZwiekszWynik()
    {
        Wynik++;
    }

    internal void Restart()
    {
        plansza.Restart();
        Wynik = 0;
    }
}

