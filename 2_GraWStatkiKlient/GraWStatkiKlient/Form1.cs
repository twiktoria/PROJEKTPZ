using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GraWStatkiKlient
{
    public partial class Form1 : Form
    {
        List<Button> buttonyGracza;
        List<Button> buttonyKomputera;
        List<string> statkiGracza = new List<string> { };

        int wynikGracza;
        int wynikKomputera;
        int liczbaStatkow;

        SimpleTcpClient klient;

        public Form1()
        {
            InitializeComponent();
            PrzedGra();
        }

        private void PrzedGra()
        {
            liczbaStatkow = 3;
            wynikGracza = 0;
            wynikKomputera = 0;
            buttonyGracza = new List<Button> { W1, W2, W3, W4, X1, X2, X3, X4, Y1, Y2, Y3, Y4, Z1, Z2, Z3, Z4 };
            buttonyKomputera = new List<Button> { A1, A2, A3, A4, B1, B2, B3, B4, C1, C2, C3, C4, D1, D2, D3, D4 };
            buttonAtaku.Enabled = false;

            for (int i = 0; i < buttonyKomputera.Count; i++)
            {
                buttonyKomputera[i].Enabled = false;
                ListBoxKomputera.Items.Add(buttonyKomputera[i].Text);
            }

            for (int i = 0; i < buttonyGracza.Count; i++)
            {
                buttonyGracza[i].Enabled = false;
            }
        }

        private void UstawPrzyciski(bool state)
        {
            for (int i = 0; i < buttonyGracza.Count; i++)
            {
                buttonyGracza[i].Enabled = state;
            }
        }

        private void NacisnieciePrzyciskuPolaczenia(object sender, EventArgs e)
        {
            klient = new SimpleTcpClient(ipPort.Text);
            try
            {
                klient.Connect();
                UstawPrzyciski(true);
                klient.Events.Connected += Polaczenie;
                klient.Events.Disconnected += Rozlaczenie;
                klient.Events.DataReceived += OtrzymanieDanych;
                polacz.Enabled = false;
                ipPort.Enabled = false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                MessageBox.Show("Upewnij się że serwer jest włączony!", "BRAK POŁĄCZENIA");
            }

        }

        private void WyborPozycjiGracza(object sender, EventArgs e)
        {
            if (statkiGracza.Count < liczbaStatkow)
            {
                var przycisk = (Button)sender;
                przycisk.Enabled = false;
                przycisk.BackgroundImage = Properties.Resources.ship;
                przycisk.BackColor = Color.Black;
                statkiGracza.Add(przycisk.Name);
            }

            if (statkiGracza.Count == liczbaStatkow)
            {
                UstawPrzyciski(false);
                buttonAtaku.Enabled = true;
                buttonAtaku.BackColor = Color.LightSkyBlue;
                txtPomoc.Text = "Teraz wybierz pozycję z rozwijanej listy i kliknij przycisk Ataku.";
                klient.Send("pozycja;" + String.Join(";", statkiGracza));
            }
        }

        private void NacisnieciePrzyciskuAtaku(object sender, EventArgs e)
        {
            string pozycjaAtaku = ListBoxKomputera.Text;

            if (pozycjaAtaku == "")
            {
                MessageBox.Show("Najpierw wybierz pozycję do ataku!", "PUSTY ATAK");
            }
            else
            {
                ruchGracza.Text = pozycjaAtaku;
                klient.Send("strzal;" + pozycjaAtaku);
            }
        }

        private void OtrzymanieDanych(object sender, SuperSimpleTcp.DataReceivedEventArgs e)
        {
            string wiadomosc = Encoding.UTF8.GetString(e.Data);
            string typ = wiadomosc.Split(';')[0];
            RozpocznijGre(typ);
            SprawdzTrafienie(wiadomosc, typ);
            SprawdzPudlo(wiadomosc, typ);
            SprawdzStrzalKomputera(wiadomosc, typ);
            SprawdzCzyKoniec(wiadomosc);
        }

        private static void RozpocznijGre(string typ)
        {
            if (typ.Equals("start"))
            {
                MessageBox.Show("Zaczynamy!", "START");
            }
        }

        private void SprawdzTrafienie(string wiadomosc, string typ)
        {
            if (typ.Equals("trafiony"))
            {
                wynikGracza += 1;
                string attackPosition = wiadomosc.Split(';')[1];
                int index = buttonyKomputera.FindIndex(a => a.Name == attackPosition);

                buttonyKomputera[index].Invoke(new Action(delegate ()
                {
                    buttonyKomputera[index].BackgroundImage = Properties.Resources.sunk;
                    buttonyKomputera[index].BackColor = Color.Black;
                }));

                txtWynikGracza.Invoke(new Action(delegate ()
                {
                    txtWynikGracza.Text = wynikGracza.ToString();
                }));

                ListBoxKomputera.Invoke(new Action(delegate ()
                {
                    ListBoxKomputera.Items.Remove(buttonyKomputera[index].Text);

                }));
            }
        }

        private void SprawdzPudlo(string wiadomosc, string typ)
        {
            if (typ.Equals("pudlo"))
            {
                string attackPosition = wiadomosc.Split(';')[1];
                int index = buttonyKomputera.FindIndex(a => a.Name == attackPosition);
                buttonyKomputera[index].Invoke(new Action(delegate ()
                {
                    buttonyKomputera[index].BackgroundImage = Properties.Resources.sea;
                    buttonyKomputera[index].BackColor = Color.Black;
                }));

                ListBoxKomputera.Invoke(new Action(delegate ()
                {
                    ListBoxKomputera.Items.Remove(buttonyKomputera[index].Text);
                }));
            }
        }

        private void SprawdzStrzalKomputera(string wiadomosc, string typ)
        {
            if (typ.Equals("trafionyPC"))
            {
                wynikKomputera += 1;
                string enemyAttackPosition = wiadomosc.Split(';')[1];
                int index = buttonyGracza.FindIndex(a => a.Name == enemyAttackPosition);

                buttonyGracza[index].Invoke(new Action(delegate ()
                {
                    buttonyGracza[index].BackgroundImage = Properties.Resources.sunk;
                    buttonyGracza[index].BackColor = Color.Black;
                }));

                ruchPrzeciwnika.Invoke(new Action(delegate ()
                {
                    ruchPrzeciwnika.Text = buttonyGracza[index].Text;
                }));

                txtWynikKomputera.Invoke(new Action(delegate ()
                {
                    txtWynikKomputera.Text = wynikKomputera.ToString();
                }));
            }

            if (typ.Equals("pudloPC"))
            {
                string enemyAttackPosition = wiadomosc.Split(';')[1];
                int index = buttonyGracza.FindIndex(a => a.Name == enemyAttackPosition);

                buttonyGracza[index].Invoke(new Action(delegate ()
                {
                    buttonyGracza[index].BackgroundImage = Properties.Resources.sea;
                    buttonyGracza[index].BackColor = Color.Black;
                }));

                ruchPrzeciwnika.Invoke(new Action(delegate ()
                {
                    ruchPrzeciwnika.Text = buttonyGracza[index].Text;
                }));
            }
        }

        private void SprawdzCzyKoniec(string wiadomosc)
        {
            if (wiadomosc.Contains("koniec"))
            {

                buttonAtaku.Invoke(new Action(delegate ()
                {
                    if (wiadomosc.Contains("wygrana"))
                    {
                        MessageBox.Show($"Wygrana {txtWynikGracza.Text}:{txtWynikKomputera.Text}!", "KONIEC");
                        buttonAtaku.Enabled = false;
                        Application.Restart();
                    }
                    else if (wiadomosc.Contains("przegrana"))
                    {
                        MessageBox.Show($"Przegrana {txtWynikGracza.Text}:{txtWynikKomputera.Text}!", "KONIEC");
                        buttonAtaku.Enabled = false;
                        Application.Restart();
                    }
                    else
                    {
                        MessageBox.Show($"Remis {txtWynikGracza.Text}:{txtWynikKomputera.Text}!", "KONIEC");
                        buttonAtaku.Enabled = false;
                        Application.Restart();
                    }
                }));
            }
        }

        private void Polaczenie(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Połączono.");
        }

        private void Rozlaczenie(object sender, ConnectionEventArgs e)
        {
            MessageBox.Show($"Rozłączono serwer!", "STATUS");
            Application.Restart();
        }
    }
}
