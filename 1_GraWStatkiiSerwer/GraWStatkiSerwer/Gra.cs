using SuperSimpleTcp;
using System.Text;

internal class Gra
{
    internal void Rozpocznij()
    {
        int liczbaStatkow = 3;
        var random = new Random();

        Plansza planszaGracza = new Plansza(new List<string> { "W1", "W2", "W3", "W4", "X1", "X2", "X3", "X4", "Y1", "Y2", "Y3", "Y4", "Z1", "Z2", "Z3", "Z4" });
        Plansza planszaKomputera = new Plansza(new List<string>() { "A1", "A2", "A3", "A4", "B1", "B2", "B3", "B4", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4" });

        Gracz gracz = new Gracz("gracz", planszaGracza);
        Gracz komputer = new Gracz("komputer", planszaKomputera);

        SimpleTcpServer server = new SimpleTcpServer("127.0.0.1:9999");

        server.Start();
        server.Events.ClientConnected += Polaczenie;
        server.Events.ClientDisconnected += Rozlaczenie;
        server.Events.DataReceived += OtrzymanieDanych;
        Console.WriteLine("Zaczynamy!");

        void OtrzymanieDanych(object? sender, DataReceivedEventArgs e)
        {
            string wiadomosc = Encoding.UTF8.GetString(e.Data);
            string typ = wiadomosc.Split(';')[0];
            Console.WriteLine($"Otrzymałem nastepującą wiadomość: {wiadomosc}");

            if (typ.Equals("pozycja"))
            {
                string pozycja1StatkuGracza = wiadomosc.Split(';')[1];
                string pozycja2StatkuGracza = wiadomosc.Split(';')[2];
                string pozycja3StatkuGracza = wiadomosc.Split(';')[3];

                gracz.DodajStatek(pozycja1StatkuGracza);
                gracz.DodajStatek(pozycja2StatkuGracza);
                gracz.DodajStatek(pozycja3StatkuGracza);
                gracz.WypiszPozycje();


                for (int i = 0; i < liczbaStatkow; i++)
                {
                Back2:
                    string pole = komputer.WylosujPole(random, false);
                    if (komputer.CzyTrafiony(pole))
                    {
                        goto Back2;
                    }
                    else
                    {
                        komputer.DodajStatek(pole);
                    }
                }
                komputer.WypiszPozycje();
                Console.WriteLine("Zaczynam gre!");
                server.Send(e.IpPort.ToString(), "start");
            }

            if (typ.Equals("strzal"))
            {
                string strzalGracza = wiadomosc.Split(';')[1];

                if (komputer.CzyTrafiony(strzalGracza))
                {
                    Console.WriteLine($"Gracz strzela w: {strzalGracza} i trafia!");
                    server.Send(e.IpPort.ToString(), "trafiony;" + strzalGracza);
                    gracz.ZwiekszWynik();
                }

                else
                {
                    Console.WriteLine($"Gracz strzela w: {strzalGracza} i nie trafia!");
                    server.Send(e.IpPort.ToString(), "pudlo;" + strzalGracza);
                }
                string strzalKomputera = gracz.WylosujPole(random);

                if (gracz.CzyTrafiony(strzalKomputera))
                {
                    Console.WriteLine($"Komputer strzela w: {strzalKomputera} i trafia!");
                    server.Send(e.IpPort.ToString(), "trafionyPC;" + strzalKomputera);
                    komputer.ZwiekszWynik();
                }

                else
                {
                    Console.WriteLine($"Komputer strzela w: {strzalKomputera} i nie trafia!");
                    server.Send(e.IpPort.ToString(), "pudloPC;" + strzalKomputera);
                }

                if (gracz.Wynik == liczbaStatkow && komputer.Wynik < liczbaStatkow)
                {
                    Console.WriteLine($"Gracz wygrał {gracz.Wynik}:{komputer.Wynik}.");
                    server.Send(e.IpPort.ToString(), ";koniec;wygrana");
                    Restart();
                }

                if (komputer.Wynik == liczbaStatkow && gracz.Wynik < liczbaStatkow)
                {
                    Console.WriteLine($"Komputer wygrał {komputer.Wynik}:{gracz.Wynik}.");
                    server.Send(e.IpPort.ToString(), ";koniec;przegrana");
                    Restart();
                }

                if (komputer.Wynik == liczbaStatkow && gracz.Wynik == liczbaStatkow)
                {
                    Console.WriteLine($"Remis {gracz.Wynik}:{komputer.Wynik}.");
                    server.Send(e.IpPort.ToString(), ";koniec;remis");
                    Restart();
                }
            }
        }

        void Restart()
        {
            string client = server.GetClients().First();
            server.DisconnectClient(client);
            liczbaStatkow = 3;
            random = new Random();
            gracz.Restart();
            komputer.Restart();
        }

        void Polaczenie(object? sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Połączono.");
        }

        void Rozlaczenie(object? sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Rozłączono.");
        }
    }
}