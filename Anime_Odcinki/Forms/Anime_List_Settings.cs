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
    public partial class Anime_List_Settings : Form
    {
        public Boolean updated;
        File_and_Data_Manager.Anime_manager am;
        public Anime_List_Settings()
        {
            InitializeComponent();
            Load_list();
            am = new File_and_Data_Manager.Anime_manager();
            this.updated = false;
        }
        private void Load_list() {
            var source = new BindingSource();
            source.DataSource = Data.Data.list.Select(o=>new { image=Get_Image(o.image_url), o.title, o.link_to_anime});
            dataGridView1.DataSource = source;
            dataGridView1.Columns[0].HeaderCell.Value = "Okładka";
            dataGridView1.Columns[1].HeaderCell.Value = "Tytuł";
            dataGridView1.Columns[2].HeaderCell.Value = "Link do anime";
            var source2 = new BindingSource();
            source2.DataSource= Data.Data.list.Select(o => new { o.title });
            comboBox1.DataSource = source2;
            comboBox1.DisplayMember = "title";
        }
        private Image Get_Image(String link) {
            Web.Web wb = new Web.Web();
            MemoryStream stream = new MemoryStream(wb.Get_Image_in_Byte(link));
            Image img = new Bitmap(Image.FromStream(stream), new Size(50, 70));
            return img;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1) {
                textBox1.Text = "";
                return;
            }
            textBox1.Text = Data.Data.list[comboBox1.SelectedIndex].link_to_anime;
            int current_column = dataGridView1.CurrentCell!=null? dataGridView1.CurrentCell.ColumnIndex:0;
            dataGridView1.CurrentCell = dataGridView1.Rows[comboBox1.SelectedIndex].Cells[current_column];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1) {
                label1.Text = "Proszę wybrać anime!";
                return;
            }
            textBox1.Text = am.Add_https(textBox1.Text);
            if (textBox1.Text.Equals("") || am.Is_It_Link_to_Anime(textBox1.Text))
            {
                int current_column = dataGridView1.CurrentCell.ColumnIndex;
                int curren_row = comboBox1.SelectedIndex;
                Data.Data.Anime a1 = new Data.Data.Anime();
                a1 = Data.Data.list[comboBox1.SelectedIndex];
                a1.link_to_anime = textBox1.Text;
                Data.Data.list[comboBox1.SelectedIndex] = a1;
                this.updated = true;
                Load_list();
                label1.Text = "Pomyślnie dodano podany link!";
                dataGridView1.CurrentCell = dataGridView1.Rows[curren_row].Cells[current_column];
                comboBox1.SelectedIndex = curren_row;
            }
            else {
                label1.Text = "Nie udało się dodać podanego linku!";
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            comboBox1.SelectedIndex = row;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) {
                textBox1.Text = am.Add_https(textBox1.Text);
                if (textBox1.Text.Equals("") || am.Is_It_Link_to_Anime(textBox1.Text))
                {
                    int current_column = dataGridView1.CurrentCell.ColumnIndex;
                    int curren_row = comboBox1.SelectedIndex;
                    Data.Data.Anime a1 = new Data.Data.Anime();
                    a1 = Data.Data.list[comboBox1.SelectedIndex];
                    a1.link_to_anime = textBox1.Text;
                    Data.Data.list[comboBox1.SelectedIndex] = a1;
                    this.updated = true;
                    Load_list();
                    label1.Text = "Pomyślnie dodano podany link!";
                    dataGridView1.CurrentCell = dataGridView1.Rows[curren_row].Cells[current_column];
                    comboBox1.SelectedIndex = curren_row;
                }
                else
                {
                    label1.Text = "Nie udało się dodać podanego linku!";
                }
            }
        }
    }
}
