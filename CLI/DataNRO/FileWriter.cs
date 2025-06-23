using DataNRO.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataNRO
{
    public class FileWriter
    {
        ISession session;

        public FileWriter(ISession session)
        {
            this.session = session;
        }

        public void WriteIcon(int iconId, byte[] data)
        {
            string path = $"{Path.GetDirectoryName(session.Data.Path)}\\Icons";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!session.Data.CanOverwriteIcon(iconId) && File.Exists($"{path}\\{iconId}.png"))
                return;
            File.WriteAllBytes($"{path}\\{iconId}.png", data);
        }

        public void WriteBigIcon(string fileName, byte[] data)
        {
            string path = $"{Path.GetDirectoryName(session.Data.Path)}\\BigIcons";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllBytes($"{path}\\{fileName}.png", data);
        }

        public void WriteEffectDataMobImg(int templateID, byte[] data)
        {
            string path = $"{Path.GetDirectoryName(session.Data.Path)}\\EffectDataMob";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllBytes($"{path}\\{templateID}.png", data);
        }

        public void WriteResource(string name, byte[] data)
        {
            string path = $"{Path.GetDirectoryName(session.Data.Path)}\\Resources";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllBytes($"{path}\\{name}", data);
        }

        public void WriteEffectData(short id, byte[] data)
        {
            string path = $"{Path.GetDirectoryName(session.Data.Path)}\\EffectData";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllBytes($"{path}\\{id}", data);
        }

        public void WriteEffectDataImg(short id, byte[] data)
        {
            string path = $"{Path.GetDirectoryName(session.Data.Path)}\\EffectData";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllBytes($"{path}\\{id}.png", data);
        }

        public void WriteEffectDataMob(int templateID, byte[] data)
        {
#if DEBUG
            string path = $"{Path.GetDirectoryName(session.Data.Path)}\\EffectDataMob";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllBytes($"{path}\\{templateID}", data);
#endif
        }

        public void WriteImgByName(string name, byte[] data, byte nFrame)
        {
            string path = $"{Path.GetDirectoryName(session.Data.Path)}\\ImgByName";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            name = Path.ChangeExtension(name, ".png");
            File.WriteAllBytes($"{path}\\{name}", data);
            name = Path.ChangeExtension(name, ".nFrame");
            File.WriteAllText($"{path}\\{name}", nFrame.ToString());
        }

        public void DeleteTempFiles()
        {
#if !DEBUG
            string path = $"{Path.GetDirectoryName(session.Data.Path)}\\BigIcons";
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            path = $"{Path.GetDirectoryName(session.Data.Path)}\\EffectDataMob";
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            path = $"{Path.GetDirectoryName(session.Data.Path)}\\Resources";
            if (Directory.Exists(path))
                Directory.Delete(path, true);
#endif
        }
    }
}
