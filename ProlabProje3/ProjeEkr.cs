using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ProlabProje3
{
    public partial class ProjeEkr : Form
    {
        // --- DATA YAPILARI ---
        struct burdaki_makkale
        {
            public int h_index;
            public double h_median;
           
            public string yazarin_buyukleri;
            public List<string> yazar_lar;
            public int yazilma_yili;
            public string isim;
            public string Id;
            public string KisaId;
            public List<string> referanc_lst;
          
            public int mtrs;
            public bool k_core_durumu;
            public double betwnes;
            public float x;
            public float y;
            public int atif_sayisi;
        }

       
        static burdaki_makkale[] yeni_bir_makale = new burdaki_makkale[800];
        static int json_mak_sayisi = 0;
        static int[,] grafin_yol_mtrs = new int[800, 800];
        Random rnd = new Random();
        bool h_core_secim = false;

        
        enum Mod { Genel, KCore, HCore }
        Mod aktif_mod = Mod.Genel;
        List<int> h_core_gostlecek = new List<int>();
        ToolTip bilgiKarti = new ToolTip();
        int son_gost_index = -1;

        
        public ProjeEkr()
        {
            
                InitializeComponent();
                
                this.WindowState = FormWindowState.Maximized;

          
                this.Load += new EventHandler(ekran_yukle);

                buton_ayarla(this);
            
        }
        private void buton_ayarla(Control ebeveyn)
        {
            foreach (Control item in ebeveyn.Controls)
            {
 
                if (item is Button tus)
                {
                    tus.Click -= tusK_analiz;
                    tus.Click -= tusH_analiz;
                    tus.Click -= tusGenel_analiz;


                    string yazi = tus.Text.ToLower(); 
                    string isim = tus.Name.ToLower();

                    if (yazi.Contains("k-core") || yazi.Contains("k core") || isim.Contains("kcore"))
                    {
                        tus.Click += new EventHandler(tusK_analiz);
                    }
                    else if (yazi.Contains("h-core") || yazi.Contains("h core") || isim.Contains("hcore"))
                    {
                        tus.Click += new EventHandler(tusH_analiz);
                    }
                    else
                    {
                        tus.Click += new EventHandler(tusGenel_analiz);
                    }
                }

                if (item.HasChildren)
                {
                    buton_ayarla(item); 
                }

                if (item is Panel && item.Name != "pnlMenu") 
                {

                    item.Paint += new PaintEventHandler(panelgraf_renk);
                    item.MouseClick += new MouseEventHandler(panelgraf_tıkla);
                    item.MouseMove += new MouseEventHandler(panelgraf_dolas);
                }
            }
        }

        private void ekran_yukle(object sender, EventArgs e)
        {
            string dosya_yol = "1txt.txt";


            if (!File.Exists(dosya_yol))
            {
                if (File.Exists("../1txt.txt")) dosya_yol = "../1txt.txt";
                else if (File.Exists("../../1txt.txt")) dosya_yol = "../../1txt.txt";
            }

            if (File.Exists(dosya_yol))
            {
                string dosyamiz = File.ReadAllText(dosya_yol);
                List<burdaki_makkale> veri = json_veri_ler(dosyamiz);

                json_mak_sayisi = veri.Count > 800 ? 800 : veri.Count;
                veri.Sort(sirala_id);


                int genislik = this.ClientSize.Width < 100 ? Screen.PrimaryScreen.WorkingArea.Width : this.ClientSize.Width;
                int yukseklik = this.ClientSize.Height < 100 ? Screen.PrimaryScreen.WorkingArea.Height : this.ClientSize.Height;

                for (int i = 0; i < json_mak_sayisi; i++)
                {
                    yeni_bir_makale[i] = veri[i];
                    yeni_bir_makale[i].mtrs = i;
                    yeni_bir_makale[i].k_core_durumu = true;
                    yeni_bir_makale[i].h_index = 0;


                    if (!string.IsNullOrEmpty(yeni_bir_makale[i].Id))
                    {
                        string[] parcalar = yeni_bir_makale[i].Id.Split('/');
                        yeni_bir_makale[i].KisaId = parcalar[parcalar.Length - 1];
                    }
                    else yeni_bir_makale[i].KisaId = "?";

                    if (yeni_bir_makale[i].yazar_lar.Count > 0)
                        yeni_bir_makale[i].yazarin_buyukleri = buyuk_leri_al(yeni_bir_makale[i].yazar_lar[0]);
                    else
                        yeni_bir_makale[i].yazarin_buyukleri = "";


                    int menuGenisligi = 200;


                    if (genislik > menuGenisligi + 100)
                    {
                        yeni_bir_makale[i].x = rnd.Next(menuGenisligi + 50, genislik - 50);
                    }
                    else 
                    {
                        yeni_bir_makale[i].x = rnd.Next(50, genislik - 50);
                    }

                    yeni_bir_makale[i].y = rnd.Next(50, yukseklik - 50);
                }


                graf_mtrs_olustur();
                atifsay_hesap(); 
                for (int i = 0; i < json_mak_sayisi; i++) 
                    h_med_ind(i);
                BetwCent_Hesap();

                istatstk();
                printyap_disarya();

                string rapor = istatstk();
               istatstk_txt.Text = rapor;
            }
            else
            {
                MessageBox.Show("1txt.txt dosyası bulunamadı! Lütfen bin/Debug klasörüne atın.");
            }
        }

  
       


        private void tusK_analiz(object sender, EventArgs e)
        {
            int k = 0;
            bool kutuBulundu = false;

            foreach (Control item in this.Controls)
            {
                if (item is TextBox) { int.TryParse(item.Text, out k); 
                    kutuBulundu = true; 
                    break; }
            }


            if (!kutuBulundu)
            {
                foreach (Control item in this.Controls)
                {
                    if (item is Panel)
                    {
                        foreach (Control ic in item.Controls)
                        {
                            if (ic is TextBox) { int.TryParse(ic.Text, out k); break; }
                        }
                    }
                }
            }

            if (k <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir K değeri (tamsayı) giriniz.");
                return;
            }

            aktif_mod = Mod.KCore;
            K_cor_calistir(k);
            ekran_f5();
            this.Text = $"Mod: K-Core Decomp. (k={k})";
        }


        private void tusH_analiz(object sender, EventArgs e)
        {

            aktif_mod = Mod.Genel;


            h_core_secim = true;

            h_core_gostlecek.Clear(); 
            
            ekran_f5(); 

            MessageBox.Show("H-Core Modu Devrede!\n\nAnaliz için düğüm(daire) seçin!");
            
            this.Text = "Mod: H-Core (Lütfen bir Daire Seçin...)";
        }

        private void tusGenel_analiz(object sender, EventArgs e)
        {
            aktif_mod = Mod.Genel;
            ekran_f5();
            this.Text = "Mod: Genel Görünüm";
        }

        private void panelgraf_renk(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            StringFormat fmt = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            Pen okKalemi = new Pen(Color.FromArgb(65, 0, 0, 0), 0.5f) { CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(3, 3) };//65 şeffaflık

            for (int i = 0; i < json_mak_sayisi; i++)
            {

                bool ciz = false;
                if (aktif_mod == Mod.Genel) ciz = true;
                else if (aktif_mod == Mod.KCore && yeni_bir_makale[i].k_core_durumu) ciz = true;
                else if (aktif_mod == Mod.HCore && h_core_gostlecek.Contains(i)) ciz = true;

                if (!ciz) continue;


                for (int j = 0; j < json_mak_sayisi; j++)
                {
                    bool karsiCiz = (aktif_mod == Mod.Genel) ||
                                    (aktif_mod == Mod.KCore && yeni_bir_makale[j].k_core_durumu) ||
                                    (aktif_mod == Mod.HCore && h_core_gostlecek.Contains(j));

                    if (karsiCiz && grafin_yol_mtrs[i, j] == 5)
                    {
                        g.DrawLine(okKalemi, yeni_bir_makale[i].x, yeni_bir_makale[i].y, yeni_bir_makale[j].x, yeni_bir_makale[j].y);
                    }

                    if (karsiCiz && grafin_yol_mtrs[i, j] == 4)
                    {

                        Pen yesilKalem = new Pen(Color.FromArgb(50, Color.Green), 0.5f)
                        {
                            CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(3, 3) };

                        g.DrawLine(yesilKalem, yeni_bir_makale[i].x, yeni_bir_makale[i].y, yeni_bir_makale[j].x, yeni_bir_makale[j].y);
                    }
                }


                int cap = 40 + (yeni_bir_makale[i].h_index);//daire çapı
                float x = yeni_bir_makale[i].x - cap / 2;
                float y = yeni_bir_makale[i].y - cap / 2;

                Brush renk = Brushes.Beige;
                if (aktif_mod == Mod.KCore) renk = Brushes.LightBlue;
                if (aktif_mod == Mod.HCore) renk = Brushes.LightPink;

                g.FillEllipse(renk, x, y, cap, cap);
                g.DrawEllipse(Pens.Black, x, y, cap, cap);

                g.DrawString(yeni_bir_makale[i].atif_sayisi.ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new RectangleF(x, y - 5, cap, cap / 2), fmt);
                g.DrawString(yeni_bir_makale[i].KisaId, new Font("Arial", 7), Brushes.DarkSlateGray, new RectangleF(x, y + 10, cap, cap / 2), fmt);
            }
        }

        private void panelgraf_tıkla(object sender, MouseEventArgs e)
        {
            bool tiklama_varmi = false;

            for (int i = 0; i < json_mak_sayisi; i++)
            {

                bool tiklanabilir = false;
                if (aktif_mod == Mod.Genel) tiklanabilir = true;
                else if (aktif_mod == Mod.KCore && yeni_bir_makale[i].k_core_durumu) 
                    tiklanabilir = true;
                else if (aktif_mod == Mod.HCore && h_core_gostlecek.Contains(i)) 
                    tiklanabilir = true;

                if (!tiklanabilir) continue;


                int cap = 40 + (yeni_bir_makale[i].h_index);
                if (Math.Abs(e.X - yeni_bir_makale[i].x) < cap / 2 && Math.Abs(e.Y - yeni_bir_makale[i].y) < cap / 2)
                {
                    tiklama_varmi = true;


                    if (h_core_secim)
                    {

                        h_core_gostlecek.Clear();
                        h_core_gostlecek.Add(i);

                        h_core_ayar(i);

                        aktif_mod = Mod.HCore;
                        h_core_secim = false;

                        ekran_f5();
                        this.Text = $"Mod: H-Core (Merkez ID: {yeni_bir_makale[i].KisaId}) - Genişletmek için başka düğümlere tıklayın";
                    }

                    else if (aktif_mod == Mod.HCore)
                    {

                        if (!h_core_gostlecek.Contains(i)) 
                            h_core_gostlecek.Add(i);

                        h_core_ayar(i);
                        ekran_f5();
                    }

                    else
                    {

                        string mesaj = $"Makale Bilgileri:\n\n" +
                                       $"ID: {yeni_bir_makale[i].KisaId}\n" +
                                       $"H-Index: {yeni_bir_makale[i].h_index}\n" +
                                       $"H-Median: {yeni_bir_makale[i].h_median}\n" + 
                                       $"Betweenness Centrality: {yeni_bir_makale[i].betwnes:F4}";

                        MessageBox.Show(mesaj, "Analiz Sonuçları");
                    }

                    return; 
                }
            }
        }

        private void panelgraf_dolas(object sender, MouseEventArgs e)// Bilgi kartı kısmı
        {
            bool bulundu = false;
            for (int i = 0; i < json_mak_sayisi; i++)
            {

                bool gorunur = (aktif_mod == Mod.Genel) ||
                               (aktif_mod == Mod.KCore && yeni_bir_makale[i].k_core_durumu) ||
                               (aktif_mod == Mod.HCore && h_core_gostlecek.Contains(i));

                if (!gorunur) continue;

                int cap = 40 + (yeni_bir_makale[i].h_index);

                if (Math.Abs(e.X - yeni_bir_makale[i].x) < cap / 2 && Math.Abs(e.Y - yeni_bir_makale[i].y) < cap / 2)
                {
                    if (son_gost_index != i)
                    {

                        string yazarlar = "";
                        if (yeni_bir_makale[i].yazar_lar != null)
                            yazarlar = string.Join(", ", yeni_bir_makale[i].yazar_lar);

                        string bilgi = $"ID: {yeni_bir_makale[i].KisaId}\n" +
                                       $"Başlık: {yeni_bir_makale[i].isim}\n" +
                                       $"Yazar: {yazarlar}\n" +
                                       $"Yıl: {yeni_bir_makale[i].yazilma_yili}\n" +
                                       $"Atıf: {yeni_bir_makale[i].atif_sayisi}\n" +
                                       $"BC: {yeni_bir_makale[i].betwnes:F4}";

                        bilgiKarti.Show(bilgi, (Control)sender, e.X + 15, e.Y + 15);
                        son_gost_index = i;
                    }
                    bulundu = true;
                    break;
                }
            }
            if (!bulundu) { bilgiKarti.Hide((Control)sender); son_gost_index = -1; }
        }


        void ekran_f5() { foreach (Control item in this.Controls) if (item is Panel) item.Invalidate(); }

        void atifsay_hesap()
        {
            for (int i = 0; i < json_mak_sayisi; i++)
            {
                int count = 0;
                for (int j = 0; j < json_mak_sayisi; j++) 
                { if (grafin_yol_mtrs[j, i] == 5) 
                       count++; }
                yeni_bir_makale[i].atif_sayisi = count;
            }
        }

        void h_core_ayar(int merkezIndex)
        {
            List<int> atifYapanlar = new List<int>();
            for (int i = 0; i < json_mak_sayisi; i++) 
            { if (grafin_yol_mtrs[i, merkezIndex] == 5) 
                    atifYapanlar.Add(i);}
            atifYapanlar.Sort((a, b) => yeni_bir_makale[b].atif_sayisi.CompareTo(yeni_bir_makale[a].atif_sayisi));
            int h = yeni_bir_makale[merkezIndex].h_index;
            for (int i = 0; i < atifYapanlar.Count && i < h; i++) 
            { if (!h_core_gostlecek.Contains(atifYapanlar[i])) 
                    h_core_gostlecek.Add(atifYapanlar[i]); }
        }

     
        static string istatstk()
        {
            int toplam_referans = 0;
            int max_alan_idx = -1, max_alan_sayi = -1;
            int max_veren_idx = -1, max_veren_sayi = -1;

 
            for (int i = 0; i < json_mak_sayisi; i++)
            {
                int anlik_reff_ver = 0, anlik_alan = 0;

                for (int j = 0; j < json_mak_sayisi; j++)
                {
  
                    if (grafin_yol_mtrs[i, j] == 5)
                    {
                        anlik_reff_ver++;
                        toplam_referans++;
                    }
                    else if (grafin_yol_mtrs[i, j] == -5)
                    {
                        anlik_alan++;
                    }
                }

                if (anlik_reff_ver > max_veren_sayi)
                {
                    max_veren_sayi = anlik_reff_ver;
                    max_veren_idx = i;
                }

                if (anlik_alan > max_alan_sayi)
                {
                    max_alan_sayi = anlik_alan;
                    max_alan_idx = i;
                }
            }


            string sonuc = "";
            sonuc += "GRAF KISA OZET\n\n";
            sonuc += "Toplam Makale: " + json_mak_sayisi + "\n";
            sonuc += "Toplam Referans: " + toplam_referans + "\n\n";

            sonuc += "** En Çok Ref. Veren **\n";
            if (max_veren_idx != -1)
            {
                sonuc += "ID: " + yeni_bir_makale[max_veren_idx].KisaId + "\n";
                sonuc += "Sayı: " + max_veren_sayi + "\n\n";
            }

            sonuc += "** En Çok Ref. Alan **\n";
            if (max_alan_idx != -1)
            {
                sonuc += "ID: " + yeni_bir_makale[max_alan_idx].KisaId + "\n";
                sonuc += "Sayı: " + max_alan_sayi + "\n";
            }

            return sonuc;
        }
        static List<burdaki_makkale> json_veri_ler(string yazi)
        {
            List<burdaki_makkale> liste = new List<burdaki_makkale>();
            yazi = yazi.Trim();
            if (yazi.StartsWith("[")) yazi = yazi.Substring(1);
            if (yazi.EndsWith("]")) yazi = yazi.Substring(0, yazi.Length - 1);
            string[] str = Regex.Split(yazi, @"\}\s*,\s*\{");
            for (int k = 0; k < str.Length; k++)
            {
                string s = str[k].Trim(); if (!s.StartsWith("{")) s = "{" + s; if (!s.EndsWith("}")) s = s + "}";
                burdaki_makkale m = new burdaki_makkale();
                m.isim = son_par_bul(s, "\"title\":"); m.Id = son_par_bul(s, "\"id\":");
                m.referanc_lst = liste_bul(s, "\"referenced_works\":"); m.yazar_lar = liste_bul(s, "\"authors\":");
                try { m.yazilma_yili = int.Parse(son_par_bul(s, "\"year\":")); }
                catch { m.yazilma_yili = 0; }
                liste.Add(m);
            }
            return liste;
        }
        static void printyap_disarya()
        {
            Console.WriteLine("Toplam makale say: " + json_mak_sayisi + "\n");

            for (int i = 0; i < json_mak_sayisi; i++)
            {
                Console.WriteLine("Index: " + i);
                Console.WriteLine("ID: " + yeni_bir_makale[i].Id);
                Console.WriteLine("Isim: " + yeni_bir_makale[i].isim);
                Console.WriteLine("Yazar(Kisa): " + yeni_bir_makale[i].yazarin_buyukleri);
                Console.WriteLine("Yil: " + yeni_bir_makale[i].yazilma_yili);
                Console.WriteLine("Yazar(Buyuk): " + yeni_bir_makale[i].yazarin_buyukleri);

                Console.Write("Iliskiler: ");
                for (int j = 0; j < json_mak_sayisi; j++)
                {
                    if (grafin_yol_mtrs[i, j] != 0)
                    {
                        Console.Write("[" + j + ":" + grafin_yol_mtrs[i, j] + "] ");
                    }
                }
                Console.WriteLine("H-Index: " + yeni_bir_makale[i].h_index + " | H-Median: " + yeni_bir_makale[i].h_median);
                Console.WriteLine("\n-----------------------------------");
            }
        }

        

        static void BetwCent_Hesap()
        {
            for (int i = 0; i < json_mak_sayisi; i++) yeni_bir_makale[i].betwnes = 0;
            for (int s = 0; s < json_mak_sayisi; s++)
            {
                Stack<int> S = new Stack<int>(); List<int>[] P = new List<int>[json_mak_sayisi];
                for (int k = 0; k < json_mak_sayisi; k++) P[k] = new List<int>();
                int[] sigma = new int[json_mak_sayisi]; int[] d = new int[json_mak_sayisi];
                for (int k = 0; k < json_mak_sayisi; k++) d[k] = -1;
                sigma[s] = 1; d[s] = 0; Queue<int> Q = new Queue<int>(); Q.Enqueue(s);
                while (Q.Count > 0)
                {
                    int v = Q.Dequeue(); S.Push(v);
                    for (int w = 0; w < json_mak_sayisi; w++)
                    {
                        if (v != w && Math.Abs(grafin_yol_mtrs[v, w]) == 5)
                        {
                            if (d[w] < 0) { Q.Enqueue(w); d[w] = d[v] + 1; }
                            if (d[w] == d[v] + 1) { sigma[w] += sigma[v]; 
                                P[w].Add(v); }
                        }
                    }
                }
                double[] delta = new double[json_mak_sayisi];
                while (S.Count > 0)
                {
                    int w = S.Pop(); foreach (int v in P[w]) delta[v] += (double)sigma[v] / sigma[w] * (1.0 + delta[w]);
                    if (w != s) yeni_bir_makale[w].betwnes += delta[w];
                }
            }
        }

        

        static void h_med_ind(int hdf_indx)
        {
            List<int> refrns_liste = new List<int>();
            for (int i = 0; i < json_mak_sayisi; i++)
            {
                if (grafin_yol_mtrs[i, hdf_indx] == 5)
                {
                    int i_alan_ref = 0;
                    for (int k = 0; k < json_mak_sayisi; k++) 
                        if (grafin_yol_mtrs[k, i] == 5) i_alan_ref++;
                    refrns_liste.Add(i_alan_ref);
                }
            }
            refrns_liste.Sort(); refrns_liste.Reverse();
            int h = 0;
            for (int i = 0; i < refrns_liste.Count; i++) { if (refrns_liste[i] >= i + 1) h = i + 1; else break; }
            yeni_bir_makale[hdf_indx].h_index = h;
            if (h > 0)
            {
                List<int> h_core = new List<int>();
                for (int i = 0; i < h; i++) h_core.Add(refrns_liste[i]);
                int n = h_core.Count;
                if (n % 2 != 1) yeni_bir_makale[hdf_indx].h_median = (h_core[-1 * (1 - (n / 2))] + h_core[n / 2]) / 2.0;
                else yeni_bir_makale[hdf_indx].h_median = h_core[n / 2];
            }
        }

        static void graf_mtrs_olustur()
        {
            Array.Clear(grafin_yol_mtrs, 0, grafin_yol_mtrs.Length);
            for (int i = 0; i < json_mak_sayisi - 1; i++) { grafin_yol_mtrs[i + 1, i] = -4; grafin_yol_mtrs[i, i + 1] = 4; }
            for (int i = 0; i < json_mak_sayisi; i++)
            {
                if (yeni_bir_makale[i].referanc_lst != null)
                {
                    foreach (string rfrns_id in yeni_bir_makale[i].referanc_lst)
                    {
                        int istenilen_yer = -10;
                        for (int k = 0; k < json_mak_sayisi; k++) { if (yeni_bir_makale[k].Id == rfrns_id) { istenilen_yer = k; break; } }
                        if (istenilen_yer != -10) { grafin_yol_mtrs[istenilen_yer, i] = -5; grafin_yol_mtrs[i, istenilen_yer] = 5; }
                    }
                }
            }
        }

        static string son_par_bul(string k, string s) 
        { int i = k.IndexOf(s); if (i == -1) return ""; 
            int j = k.IndexOf(":", i) + 1; while (j < k.Length && char.IsWhiteSpace(k[j])) j++; 
            if (k[j] == '"') { int l = k.IndexOf("\"", j + 1); return k.Substring(j + 1, l - j - 1);
            } 
            else { int l = k.IndexOf(",", j), m = k.IndexOf("}", j); 
                if (l == -1 || (m != -1 && m < l)) l = m; 
                return k.Substring(j, l - j).Trim(); 
            } 
        }
        static List<string> liste_bul(string k, string s) { List<string> l = new List<string>(); 
            int i = k.IndexOf(s); if (i == -1) return l; 
            int a = k.IndexOf("[", i), b = k.IndexOf("]", a); 
            string ic = k.Substring(a + 1, b - a - 1); 
            foreach (string p in ic.Split(',')) 
            { string t = p.Trim().Trim('"'); 
                if (t.Length > 0) l.Add(t); } return l;
        }
        static void K_cor_calistir(int k)
        {
            int[] dereceler = new int[json_mak_sayisi]; bool[] aktif = new bool[json_mak_sayisi];
            for (int i = 0; i < json_mak_sayisi; i++) aktif[i] = true;
            bool degisim = true;
            while (degisim)
            {
                degisim = false;
                for (int i = 0; i < json_mak_sayisi; i++)
                {
                    if (!aktif[i]) continue;
                    int drc = 0;
                    for (int j = 0; j < json_mak_sayisi; j++) { if (i != j && aktif[j] && (grafin_yol_mtrs[i, j] == 5 || grafin_yol_mtrs[i, j] == -5)) drc++; }
                    dereceler[i] = drc;
                }
                for (int i = 0; i < json_mak_sayisi; i++)
                {
                    if ((k > dereceler[i]) && aktif[i]) { aktif[i] = false; degisim = true; }
                }
            }
            for (int i = 0; i < json_mak_sayisi; i++) yeni_bir_makale[i].k_core_durumu = aktif[i];
        }
        static string buyuk_leri_al(string s) 
        { string r = ""; foreach (char c in s) 
                if (char.IsUpper(c)) r += c; 
            return r == "" ? s[0].ToString() : r; 
        }
        static int sirala_id(burdaki_makkale x, burdaki_makkale y) 
        { return string.Compare(x.Id, y.Id); }


        private void ProjeEkr_Load(object sender, EventArgs e) { }

        private void btnGenel_Click_1(object sender, EventArgs e)
        {

        }

        private void btnHCore_Click_1(object sender, EventArgs e)
        {

        }
    }
}