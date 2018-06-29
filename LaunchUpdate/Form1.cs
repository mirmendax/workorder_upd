using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace WorkUpdate
{
    public partial class Form1 : Form
    {
        public SaveLoadFileUpd SLFile = new SaveLoadFileUpd();
        public static string execFile = "WorkOrder.exe";
        public static string fileVersion = "ver.dat";
        
        public Form1()
        {
            InitializeComponent();
        }
        
        private void updatebtn_Click(object sender, EventArgs e)
        {
            try
            {
                SLFile.Upd.UnPackAllFiles();
                if (SLFile.Upd.isPatched)
                {
                    Process pr = Process.Start(SLFile.Upd.NamePatchFile);
                    pr.WaitForExit();
                    File.Delete(SLFile.Upd.NamePatchFile);
                }
                if (SLFile.Upd.isDelFile)
                {
                    Process p = Process.Start(SLFile.Upd.NameDelFile);
                    p.WaitForExit();
                    File.Delete(SLFile.Upd.NameDelFile);
                }
                FileStream fversion = File.Open(fileVersion, FileMode.Create, FileAccess.ReadWrite);
                byte[] verdata = Encoding.ASCII.GetBytes(SLFile.Upd.version+"."+SLFile.Upd.pack.ToString());
                
                fversion.Write(verdata, 0, verdata.Length);
                
                fversion.Close();
                File.Delete(SaveLoadFileUpd.FILE);
                Process.Start(execFile);
                Close();
                
            }
            catch (Exception ex)
            {
                new Log("Error update: " + ex.Message);
            }
        }

       

        private void Form1_Load(object sender, EventArgs e)
        {

            if (File.Exists(SaveLoadFileUpd.FILE) && File.Exists(execFile))
            {
                updatebtn.Enabled = true;
                SLFile.OnRewrite += SLFile_OnRewrite;
                SLFile.Upd.OnRewrite += SLFile_OnRewrite;
                SLFile.Load();                
                listBox1.Items[1] = string.Format("Version: {0}. Repack: {1}. Size: {2} kB", SLFile.Upd.version, SLFile.Upd.pack.ToString(), (SLFile.Upd.DataSize / 1000).ToString());
            }
                
        }

        void SLFile_OnRewrite()
        {
            
        }
    }
}
