using ProfileCutter.Model;
using ProfileCutter.Model.MACH3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProfileCutter.Configuration
{
    internal static class Config
    {
        private static string path = "Config.xml";

        public static void Save(CutterModel cutter)
        {
            XDocument document = new XDocument();
            XElement root = new XElement("Cutter");
            XElement mach3 = new XElement("Mach3");
            XElement axises = new XElement("Axises");
            axises.Add(cutter.X.GetAxisXElement());
            axises.Add(cutter.Y.GetAxisXElement());
            axises.Add(cutter.Z.GetAxisXElement());
            mach3.Add(axises);

            XElement programms = new XElement("Programms");
            programms.Add();

            root.Add(programms);
            root.Add(mach3);
            document.Add(root);
            document.Save(path);
        }



        public static void Load(CutterModel cutter)
        {
            if (File.Exists(path) == true)
            {
                XDocument document = XDocument.Load(path);
                XElement root = document.Element("Cutter");
                XElement mach3 = root.Element("Mach3");
                XElement axises = mach3.Element("Axises");
                cutter.X.LoadXElement(axises.Element("X"));
                cutter.Y.LoadXElement(axises.Element("Y"));
                cutter.Z.LoadXElement(axises.Element("Z"));
            }
        }
    }
}
