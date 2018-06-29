using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WorkUpdate
{
    [Serializable]
    public class Log
    {
        public static string FILE_LOG = "errorupd.log";


        public Log(string sLog)
        {
            if (!File.Exists(FILE_LOG))
            {
                StreamWriter createfile = File.CreateText(FILE_LOG);
                createfile.Close();
            }
            StreamWriter addlog = File.AppendText(FILE_LOG);

            addlog.WriteLine("[" + DateTime.Now.ToString("d.MM.yyyy") + "]" + sLog);
            addlog.Close();
        }
    }
    [Serializable]
    public class SaveLoadFileUpd
    {
        public static string FILE = "data.upd";

        public FilesListUpd Upd;

        //Delegate
        public delegate void onRewrite();
        [field: NonSerialized]
        public event onRewrite OnRewrite;

        public SaveLoadFileUpd()
        {
            Upd = new FilesListUpd();
        }

        /*public SaveLoadFileUpd(string version, int pack)
        {
            Upd = new FilesListUpd(version, pack);
        }
        */
        public void Save()
        {
            try
            {
                if (File.Exists(FILE))
                    File.Delete(FILE);
                BinaryFormatter data = new BinaryFormatter();
                FileStream file = File.Open(FILE, FileMode.Create, FileAccess.ReadWrite);
                data.Serialize(file, Upd);
                file.Close();
            }
            catch (Exception e)
            {
                new Log("Error save file: " + e.Message);
            }
        }

        public void Load()
        {
            BinaryFormatter data = new BinaryFormatter();
            if (!File.Exists(FILE))
            {
                FileStream f = File.Open(FILE, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                f.Close();
            }

            FileStream file = File.Open(FILE, FileMode.Open, FileAccess.ReadWrite);
            try
            {
                Upd = (data.Deserialize(file) as FilesListUpd);
                file.Close();
            }

            catch (System.Runtime.Serialization.SerializationException e)
            {
                new Log("Error load file: " + e.Message);
            }
            //file.Close();
            OnRewrite();
        }
    }

    [Serializable]
    public class FilesListUpd
    {

        public List<FileUpd> Files = new List<FileUpd>();
        public string version = "";
        public int pack = 0;
        public bool isPatched = false;
        public string NamePatchFile = "";
        public bool isDelFile = false;
        public string NameDelFile = "";

        //Delegate при обновлении содержимого
        public delegate void onRewrite();
        [field: NonSerialized]
        public event onRewrite OnRewrite;

        public List<string> ListFiles()
        {
            List<string> result = new List<string>();
            foreach (FileUpd f in Files)
            {
                result.Add(f.Name);
            }
            return result;
        }

        public FilesListUpd(string _ver, int _pack)
        {
            Files = new List<FileUpd>();
            this.version = _ver;
            this.pack = _pack;
            this.isPatched = false;
            this.isDelFile = false;
            this.NamePatchFile = "";
            this.NameDelFile = "";
        }

        public FilesListUpd()
        {
            Files = new List<FileUpd>();
            version = "";
            pack = 0;
        }
        /// <summary>
        /// Размер пакета обновления
        /// </summary>
        public int DataSize
        {
            get
            {
                int c = 0;
                foreach (FileUpd f in Files)
                {
                    c += f.Data.Length;
                }
                return c;
            }
        }
        /// <summary>
        /// Распаковка файла из пакета обновления
        /// </summary>
        /// <param name="name">Имя файла</param>
        public void UnPack(string name)
        {
            FileUpd unPFile = FindFile(name);
            if (unPFile.Name != "")
            {
                FileStream file = File.Open(name, FileMode.Create, FileAccess.ReadWrite);
                file.Write(unPFile.Data, 0, unPFile.Data.Length);
                file.Close();
            }
        }
        /// <summary>
        /// Отчистка пакета обновления
        /// </summary>
        public void ClearFiles()
        {
            Files.Clear();
            OnRewrite();
        }
        /// <summary>
        /// Упаковка файла в оболочку типа FileUpd
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="namefile">Имя файла в пакете</param>
        /// <returns></returns>
        private FileUpd Pack(string file, string namefile)
        {
            FileUpd result = new FileUpd();
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                byte[] b = new byte[fs.Length];
                fs.Read(b, 0, (int)fs.Length);
                fs.Close();
                result = new FileUpd(namefile, b);

            }
            catch (Exception e)
            {
                new Log("Error packs " + e.Message);
            }
            return result;
        }
        /// <summary>
        /// Добавление файла в пакет обновления
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="namefile">Имя файла в пакете</param>
        /// <returns></returns>
        public FileUpd AddFile(string file, string namefile)
        {

            FileUpd result = new FileUpd();
            FileUpd dublFile = FindFile(namefile);
            if (dublFile.Name == "")
            {
                result = Pack(file, namefile);

                if (result.Name != "")
                {
                    Files.Add(result);
                }
            }
            OnRewrite();
            return result;
        }
        /// <summary>
        /// Поиск файла в пакете
        /// </summary>
        /// <param name="fname">Имя файла</param>
        /// <returns></returns>
        public FileUpd FindFile(string fname)
        {
            FileUpd result = new FileUpd();
            foreach (FileUpd f in Files)
            {
                if (f.Name == fname)
                {
                    result = f;
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// Перезапись файла в пакете обновления
        /// </summary>
        /// <param name="oldname">Старое имя файла</param>
        /// <param name="file">Новый файл обновления</param>
        /// <param name="namefile">Новое имя файла</param>
        /// <returns></returns>
        public bool ReWriteFile(string oldname, string file, string namefile)
        {
            bool result = false;
            FileUpd oldFile = FindFile(oldname);
            if (oldFile.Name != "")
            {
                FileUpd newfile = Pack(file, namefile);
                if (newfile.Name != "")
                {
                    int countold = Files.IndexOf(oldFile);
                    bool b = Files.Remove(oldFile);
                    Files.Insert(countold, newfile);
                    if (b)
                    {
                        result = true;
                    }
                }
            }
            OnRewrite();
            return result;
        }
        /// <summary>
        /// Удаление файла из пакета обновления
        /// </summary>
        /// <param name="namefile">Имя файла</param>
        /// <returns></returns>
        public bool RemoveFile(string namefile)
        {
            bool result = false;
            FileUpd rfile = FindFile(namefile);
            if (rfile.Name != "")
            {
                result = Files.Remove(rfile);
            }
            OnRewrite();
            return result;
        }
        /// <summary>
        /// Распоковка всех файлов пакета обновления
        /// </summary>
        public void UnPackAllFiles()
        {
            foreach (FileUpd f in Files)
            {
                FileStream fs = new FileStream(f.Name, FileMode.Create, FileAccess.Write);
                fs.Write(f.Data, 0, f.Data.Length);
                fs.Close();

            }
        }


    }

    /// <summary>
    /// Основной класс файла пакета
    /// </summary>
    [Serializable]
    public class FileUpd
    {
        public string Name;
        public byte[] Data;

        public override bool Equals(object obj)
        {
            return (this.Name) == (obj as FileUpd).Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public FileUpd(string name, byte[] data)
        {
            Data = new byte[data.Length];
            Name = name;
            Data = data;
        }

        public FileUpd()
        {
            Name = "";
            Data = new byte[0];
        }

    }
}

