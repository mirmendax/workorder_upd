using System;
using System.Windows.Forms;
using System.IO;

namespace WorkUpdate
{
    
    public partial class Form1 : Form
    {
        public string fileExec = "WorkOrder.exe";
        public string fileupdate = "updateWorkOrder.exe";
        public string filedeltemp = "DelTempUpdFile.exe";
        AutoCompleteStringCollection collecFile = new AutoCompleteStringCollection();
        
        SaveLoadFileUpd SLFiles = new SaveLoadFileUpd();

        public Form1()
        {
            InitializeComponent();
            SLFiles.Upd.OnRewrite += Upd_OnRewrite;
            SLFiles.OnRewrite += Upd_OnRewrite;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileUpd f = SLFiles.Upd.AddFile(openFileDialog1.FileName, openFileDialog1.SafeFileName);
                MessageBox.Show("Added file "+f.Name+" Size: "+f.Data.Length.ToString());
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool pr1 = false;
            bool pr2 = false;
            if (SLFiles.Upd.isPatched)
                if (SLFiles.Upd.NamePatchFile != "")
                    pr1 = true;
                else
                    pr1 = false;
            else
                pr1 = true;

            if (SLFiles.Upd.isDelFile)
                if (SLFiles.Upd.NameDelFile != "")
                    pr2 = true;
                else
                    pr2 = false;
            else
                pr2 = true;
            if (pr1 && pr2)
                SLFiles.Save();
            else MessageBox.Show("Error input parametrs.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Application.StartupPath;
        }

        void Upd_OnRewrite()
        {
            listBox1.Items.Clear();
            collecFile.Clear();
            foreach (string s in SLFiles.Upd.ListFiles())
            {
                listBox1.Items.Add(s);
                collecFile.Add(s);
            }
            
            textBox1.Text = SLFiles.Upd.version;
            numericUpDown1.Value = SLFiles.Upd.pack;
            label4.Text = "Size Update: " + (int)SLFiles.Upd.DataSize/1000 + " Kb";

            checkBox1.Checked = SLFiles.Upd.isPatched;
            checkBox2.Checked = SLFiles.Upd.isDelFile;

            textBox2.Enabled = SLFiles.Upd.isPatched;
            textBox3.Enabled = SLFiles.Upd.isDelFile;

            textBox2.Text = SLFiles.Upd.NamePatchFile;
            textBox3.Text = SLFiles.Upd.NameDelFile;

            textBox2.AutoCompleteCustomSource = collecFile;
            textBox3.AutoCompleteCustomSource = collecFile;
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (File.Exists(SaveLoadFileUpd.FILE))
            {
                SLFiles.Load();
                SLFiles.OnRewrite += Upd_OnRewrite;
                SLFiles.Upd.OnRewrite += Upd_OnRewrite;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int count = listBox1.SelectedIndex;
            if (count != -1)
            {
                string namefile = (listBox1.Items[count] as string);
                if (MessageBox.Show("Удалить файл "+namefile+"?","Удалить???",MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SLFiles.Upd.RemoveFile(namefile);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int count = listBox1.SelectedIndex;
            if (count != -1)
            {
                string s = listBox1.Items[count].ToString();
                if (MessageBox.Show("Заменить файл " + s + "?", "Заменить???", MessageBoxButtons.YesNo) == DialogResult.Yes)
                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    bool f = SLFiles.Upd.ReWriteFile(s ,openFileDialog1.FileName, openFileDialog1.SafeFileName);
                    if (f)
                        MessageBox.Show("ReWrite file " + openFileDialog1.SafeFileName);
                    else
                        MessageBox.Show("Error rewrited file");

                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear all to list?", "Clear?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SLFiles.Upd.ClearFiles();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bool ispath = (sender as CheckBox).Checked;
            SLFiles.Upd.isPatched = ispath;
            textBox2.Enabled = ispath;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            bool isdel = (sender as CheckBox).Checked;
            SLFiles.Upd.isDelFile = isdel;
            textBox3.Enabled = isdel;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SLFiles.Upd.UnPackAllFiles();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i != -1)
            {
                SLFiles.Upd.UnPack((listBox1.Items[i] as string));
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SLFiles.Upd.version = (sender as TextBox).Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            SLFiles.Upd.NamePatchFile = (sender as TextBox).Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            SLFiles.Upd.NameDelFile = (sender as TextBox).Text;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            SLFiles.Upd.pack = (int)(sender as NumericUpDown).Value;
        }

        
    }
}
