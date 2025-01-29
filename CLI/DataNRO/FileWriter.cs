using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataNRO.Interfaces;

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

        public void WriteMobImg(int templateID, byte[] data)
        {
            string path = $"{Path.GetDirectoryName(session.Data.Path)}\\MobImg";
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

        public void DeleteTempFiles()
        {
#if !DEBUG
            string path = $"{Path.GetDirectoryName(session.Data.Path)}\\BigIcons";
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            path = $"{Path.GetDirectoryName(session.Data.Path)}\\MobImg";
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            path = $"{Path.GetDirectoryName(session.Data.Path)}\\Resources";
            if (Directory.Exists(path))
                Directory.Delete(path, true);
#endif
        }
    }
}
