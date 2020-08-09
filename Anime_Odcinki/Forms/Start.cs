using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Anime_Odcinki.Loading
{
    public partial class Start : Form
    {
        Web.Web wb;
        File_and_Data_Manager.Files_manager fm;
        File_and_Data_Manager.Anime_manager am;
        public Start()
        {
            InitializeComponent();
            wb = new Web.Web();
            fm = new File_and_Data_Manager.Files_manager();
            am = new File_and_Data_Manager.Anime_manager();
            tableLayoutPanel1.Visible = false;
        }
        private void Operations() {
            settingsToolStripMenuItem.Enabled = false;
            Set_Text("Sprawdzanie dostepności strony MyAnimeList.net");
            if (!wb.MAL_Available()) {
                Set_Text("Nie udało się połączyć z stroną MyAnimeList.net");
                return;
            }
            Set_Text("Sprawdzanie dostepności strony animezone.pl");
            if (!wb.AnimeZone_Available())
            {
                Set_Text("Nie udało się połączyć z stroną animezone.pl");
                return;
            }
            settingsToolStripMenuItem.Enabled = true;
            Data.Data.location= Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\AOI\\" + Data.Data.file_name;
            Set_Text("Sprawdzam czy istnieje plik z danymi");
            Data.Data.file_exist = fm.File_Exist(Data.Data.location);
            if (Data.Data.file_exist)
            {
                Set_Text("Sprawdzam poprawność danych");
                Data.Data.nick_MAL = fm.Nick_MAL(Data.Data.location);
                if (!wb.MAL_Acc_Exist(Data.Data.nick_MAL)) Add_MAL();
                else Show_List();
            }
            else Add_MAL();
            
        }
        private void Add_MAL() {
            settingsToolStripMenuItem.Enabled = false;
            Set_Text("");
            label2.Visible = true;
            textBox1.Visible = true;
            button1.Visible = true;
            tableLayoutPanel1.Visible = false;


        }
        private void Refresh_list() {
            Clear_Current_List();
            Operations();
        }
        private void Clear_Current_List() {
            Data.Data.new_episodes.Clear();
            Data.Data.list.Clear();
            Set_Text("");
            tableLayoutPanel1.Visible = false;
            for (int i = tableLayoutPanel1.Controls.Count - 1; i > 3; i--)
            {
                tableLayoutPanel1.Controls.RemoveAt(i);
            }
            tableLayoutPanel1.AutoScroll = false;
            tableLayoutPanel1.AutoScroll = true;
        }
        private void Show_List() {
            textBox1.Visible = false;
            button1.Visible = false;
            label2.Visible = false;
            am.Load_anime_list(wb.ListHtmlCode());
            if(Data.Data.file_exist) am.Load_anime_from_file(fm.File_Read(Data.Data.location));
            Load_Anime_List();
            tableLayoutPanel1.Refresh();
            tableLayoutPanel1.Visible = true;
            this.Refresh();
            fm.Save_to_File(am.Text_to_Save_in_File());
        }


        private void Load_Anime_List() {
            List<Data.Data.Anime> list2 = new List<Data.Data.Anime>(Data.Data.list);
            int num_anime_not_found = 0;
            foreach (Data.Data.Anime a1 in list2) {
                Set_Text("Sprawdzanie anime o tytule: "+a1.title);
                if (!Check_Anime(a1)) num_anime_not_found++;
            }
            int row = 1;
            foreach (Data.Data.Anime a1 in Data.Data.new_episodes) {
                byte[] imageData =wb.Get_Image_in_Byte(a1.image_url);
                if (imageData != null)
                {
                    MemoryStream stream = new MemoryStream(imageData);
                    Image img = Image.FromStream(stream);
                    tableLayoutPanel1.Controls.Add(new PictureBox { Image = img, Height = 70, Width = 50, Anchor = AnchorStyles.Top, SizeMode = PictureBoxSizeMode.StretchImage }, 0, row);
                }
                else {
                    tableLayoutPanel1.Controls.Add(new Label { Text = "Brak okładki", AutoSize = true, ForeColor = Color.Orange, Font = new Font(Font.FontFamily, 14), Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter }, 0, row);
                }
                tableLayoutPanel1.Controls.Add(new Label { Text = a1.title, AutoSize = true, ForeColor = Color.Yellow, Font = new Font(Font.FontFamily, 14), Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter }, 1, row);
                tableLayoutPanel1.Controls.Add(new Label { Text = (a1.num_watched + 1).ToString(), AutoSize = false, ForeColor = Color.Yellow, Font = new Font(Font.FontFamily, 14), Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter }, 2, row);
                LinkLabel l1 = new LinkLabel { Text = "Link do odcinka", AutoSize = true, ForeColor = Color.Yellow, Font = new Font(Font.FontFamily, 14), Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter };
                l1.Click+= ((sender, args) => System.Diagnostics.Process.Start(a1.link_to_epiosde));
                tableLayoutPanel1.Controls.Add(l1, 3, row);
                row++;
            }
            tableLayoutPanel1.Refresh();
            Set_Text("Statystyki   Nowe odcinki: "+Data.Data.new_episodes.Count()+"   Ilość Anime na MALu: "+Data.Data.list.Count()+"   Nie znalezione anime: "+num_anime_not_found);

        }
        private Boolean Check_Anime(Data.Data.Anime a1) {
            int num_episode;
            if (a1.saved_in_file)
            {
                num_episode = am.Last_Polish_Episode(a1.link_to_anime);
                if (a1.num_watched < num_episode)
                {
                    a1.link_to_epiosde = a1.link_to_anime.Replace("/anime/", "/odcinek/") + "/" + (a1.num_watched+1);
                    Data.Data.new_episodes.Add(a1);
                }
                if (a1.link_to_anime.Equals("")) return false;
                else return true;
            } else {
                String link = am.Link_to_Anime(a1);
                   num_episode =am.Last_Polish_Episode(link);
                Data.Data.Anime a2 = a1;
                a2.link_to_anime = link;
                Data.Data.list[Data.Data.list.IndexOf(a1)] = a2;
                if (a1.num_watched < num_episode)
                {
                    a2.link_to_epiosde =link.Replace("/anime/", "/odcinek/") + "/" + (a1.num_watched + 1);
                    Data.Data.new_episodes.Add(a2);
                }
                if (a2.link_to_anime.Equals("")) return false;
                else return true;
            }
        }

        private void Set_Text(String text) {
            label1.Text=text;
            this.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (wb.MAL_Acc_Exist(textBox1.Text))
            {
                Data.Data.nick_MAL = textBox1.Text;
                Show_List();
                settingsToolStripMenuItem.Enabled = true;
            }
            else Set_Text("Nie istnieje podane konto w serwisie MAL");
        }

        private void Start_Shown(object sender, EventArgs e)
        {
            Operations();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Refresh_list();
        }

        private void changeAccMALToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Data.Data.file_exist = false;
            Data.Data.nick_MAL = "";
            Clear_Current_List();
            Add_MAL();
        }

        private void animeLinksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Anime_List_Settings als = new Anime_List_Settings();
            als.ShowDialog();
            if (als.updated)
            {
                fm.Save_to_File(am.Text_to_Save_in_File());
                Refresh_list();
            }
            als.Dispose();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) {
                if (wb.MAL_Acc_Exist(textBox1.Text))
                {
                    Data.Data.nick_MAL = textBox1.Text;
                    Show_List();
                    settingsToolStripMenuItem.Enabled = true;
                }
                else Set_Text("Nie istnieje podane konto w serwisie MAL");
            }
        }
    }
}
