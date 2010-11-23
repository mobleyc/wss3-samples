using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Collections.Specialized;

namespace DynamicList
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Error: url required");
            }

            var url = args[0];
            var listName = "Dynamic List";

            using (var site = new SPSite(url))
            using (var web = site.OpenWeb())
            {
                var existingList = findList(web, listName);
                if (null != existingList)
                {
                    Console.WriteLine("List already exists");
                    return;
                }

                var guid = web.Lists.Add(listName, listName, SPListTemplateType.GenericList);
                Console.WriteLine("List added");

                var dynamicList = web.Lists[guid];
                dynamicList.Fields.Add("Value", SPFieldType.Text, true);
                dynamicList.Update();

                Console.WriteLine("Value field added");

                var viewFields = new StringCollection
                {
                    "Title",
                    "Value"
                };
                string query = "<Where><Gt><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Gt></Where>";
                dynamicList.Views.Add("Custom View", viewFields, query, 100, false, false);

                Console.WriteLine("Custom view added");
            }
        }

        private static SPList findList(SPWeb web, string listName)
        {
            foreach (SPList list in web.Lists)
            {
                if (list.Title.Equals(listName, StringComparison.OrdinalIgnoreCase))
                {
                    return list;
                }
            }

            return null;
        }
    }
}