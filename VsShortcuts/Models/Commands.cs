using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VsShortcuts.Models
{
    public class Commands
    {
        public static int ShortcutCount = 0;

        public static Dictionary<string, List<Binding>> GenerateList(string version)
        {
            ShortcutCount = 0;

            var UrlString = @"P:\Users\jesse\AppData\2019.xml";

            Dictionary<string, List<Binding>> dic = new Dictionary<string, List<Binding>>();
            //XmlDocument doc = XmlDocument.Load(HostingEnvironment.MapPath(UrlString));

            XmlTextReader reader = new XmlTextReader(UrlString);

            while (reader.Read())
            {
                var name = reader.GetAttribute("name");
                var shortcut = reader.GetAttribute("shortcut");

                if (string.IsNullOrEmpty(name))
                    continue;


                Binding prev = new Binding();

                //foreach (XmlElement node in doc.Descendants("command"))
                //{
                //string name = node.Attribute("name").Value;
                //string shortcut = node.Attribute("shortcut").Value;

                Binding binding = prev.FullName == name ? prev : CreateBinding(dic, name, shortcut);

                if (!binding.Shortcuts.Contains(shortcut))
                    prev.Shortcuts.Add(shortcut);

                prev = binding;
                ShortcutCount += 1;
            }

            return dic;
        }

        private static Binding CreateBinding(Dictionary<string, List<Binding>> dic, string name, string shortcut)
        {
            int index = name.IndexOf('.');
            string displayName = index > 0 ? name.Substring(name.LastIndexOf('.') + 1) : name;
            string prefix = index > 0 ? CleanName(name.Substring(0, index)) : "Misc";
            Binding binding = new Binding(name, CleanName(displayName), shortcut);

            if (!dic.ContainsKey(prefix))
                dic[prefix] = new List<Binding>();

            dic[prefix].Add(binding);

            return binding;
        }

        public static string CleanName(string name)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(name[0]);

            for (int i = 1; i < name.Length; i++)
            {
                char c = name[i];

                if (char.IsUpper(c) && !char.IsUpper(name[i - 1]))
                {
                    sb.Append(" ");
                }

                sb.Append(c);
            }

            return sb.ToString();
        }
    }

    public class Binding
    {
        public Binding(string name, string displayName, string shortcut)
        {
            FullName = name;
            DisplayName = displayName;
            Shortcuts.Add(shortcut);
        }

        public Binding()
        {
            FullName = string.Empty;
        }

        public string FullName;
        public string DisplayName;
        public List<string> Shortcuts = new List<string>();
    }
}
