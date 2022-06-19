
internal class Plansza
{

    List<string> polaGracza;
    List<string> DomyslnePola;
    public List<string> StatkiGracza { get; private set; }

    public Plansza(List<string> listaPol)
    {
        DomyslnePola = listaPol;
    }

    internal void DodajStatek(string pozycja)
    {
        StatkiGracza.Add(pozycja);
    }

    internal string WylosujPole(Random random, bool usun)
    {
        int indeksStrzalu = random.Next(polaGracza.Count);
        string strzalKomputera = polaGracza[indeksStrzalu];
        if (usun)
        {
            polaGracza.RemoveAt(indeksStrzalu);
        }
        return strzalKomputera;
    }

    internal bool CzyTrafiony(string strzalKomputera)
    {
        return StatkiGracza.Contains(strzalKomputera);
    }

    internal void Restart()
    {
        polaGracza = new List<string>(DomyslnePola);
        StatkiGracza = new List<string>();
    }
}
