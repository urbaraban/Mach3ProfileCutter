using ProfileCutter.Model;
using ProfileCutter.Model.MACH3;
using ProfileCutter.Model.Programms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace ProfileCutter.Configuration
{
    internal static class ReadWrite
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
            foreach (CutConfiguration programm in cutter.CutConfigs.Configs)
            {
                programms.Add(new XElement("Programm",
                    new XAttribute("Guid", programm.Id),
                    new XAttribute("Name", programm.Name),
                    new XAttribute("Width", programm.Width),
                    new XAttribute("Length", programm.Length),
                    new XAttribute("Interval", programm.Interval),
                    new XAttribute("Height", programm.Height)));
            }
            
            mach3.Add(programms);

            XElement profiles = new XElement("Profiles");
            foreach(Profile prof in cutter.CutConfigs.Profiles)
            {
                profiles.Add(new XElement("Profile",
                    new XAttribute("Guid", prof.Id),
                    new XAttribute("Name", prof.Name),
                    new XAttribute("Width", prof.Width),
                    new XAttribute("Length", prof.Length)));
            }
            mach3.Add(profiles);

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

                XElement profile = mach3.Element("Profiles");
                foreach (XElement prof in profile.Elements("Profile"))
                {
                    if (prof.FirstAttribute != null)
                    {
                        cutter.CutConfigs.Profiles.Add(
                            new Profile()
                            {
                                Id = Guid.Parse(prof.Attribute("Guid").Value),
                                Name = prof.Attribute("Name").Value,
                                Width = double.Parse(prof.Attribute("Width").Value.Replace(',', '.')),
                                Length = double.Parse(prof.Attribute("Length").Value.Replace(',', '.')),
                            });
                    }
                }

                XElement programms = mach3.Element("Programms");
                foreach (XElement prog in programms.Elements("Programm"))
                {
                    cutter.CutConfigs.Configs.Add(
                        new CutConfiguration()
                        {
                            Id = Guid.Parse(prog.Attribute("Guid").Value),
                            Name = prog.Attribute("Name").Value,
                            Width = double.Parse(prog.Attribute("Width").Value.Replace(',', '.')),
                            Interval = double.Parse(prog.Attribute("Interval").Value.Replace(',', '.')),
                            Length = double.Parse(prog.Attribute("Length").Value.Replace(',', '.')),
                            Height = double.Parse(prog.Attribute("Height").Value.Replace(',', '.'))
                        });
                }

                mach3.Add(programms);

            }
        }
    }
}
