using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace sdsetup_manifest5to6 {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("SDSetup Manifest v5 to v6 Converter");

            Manifest manifest = JsonConvert.DeserializeObject<Manifest>(File.ReadAllText(Environment.CurrentDirectory + "\\manifest5.json"));

            Dictionary<string, NewPlatform> platforms = new Dictionary<string, NewPlatform>();
            foreach (Platform plat in manifest.Platforms.Values) {
                Dictionary<string, NewPackageSection> sections = new Dictionary<string, NewPackageSection>();

                foreach(PackageSection sec in plat.PackageSections) {
                    Dictionary<string, NewPackageCategory> categories = new Dictionary<string, NewPackageCategory>();

                    foreach (PackageCategory cat in sec.Categories) {
                        Dictionary<string, NewPackageSubcategory> subcategories = new Dictionary<string, NewPackageSubcategory>();

                        foreach (PackageSubcategory sub in cat.Subcategories) {
                            foreach (Package p in sub.Packages) {


                                NewPackage np = new NewPackage(p.ID, p.Name, p.DisplayName, plat.ID, sec.ID, cat.ID, sub.ID, p.Authors, new Dictionary<string, string> { { "latest", p.Version } }, p.Source, p.DLSource, 0, 0, p.EnabledByDefault, p.Visible, true, p.Description, p.When, p.WhenMode, p.Warning, p.Dependencies, new List<string>());
                                Directory.CreateDirectory(Environment.CurrentDirectory + "\\platforms\\" + np.Platform + "\\" + np.ID);
                                File.WriteAllText(Environment.CurrentDirectory + "\\platforms\\" + np.Platform + "\\" + np.ID + "\\info.json", JsonConvert.SerializeObject(np, Formatting.Indented));
                            }
                            subcategories[sub.ID] = new NewPackageSubcategory(sub.ID, sub.Name, sub.DisplayName, sub.Visible, sub.When, sub.WhenMode, new Dictionary<string, NewPackage>());
                        }
                        categories[cat.ID] = new NewPackageCategory(cat.ID, cat.Name, cat.DisplayName, cat.Visible, cat.When, cat.WhenMode, subcategories);
                    }
                    sections[sec.ID] = new NewPackageSection(sec.ID, sec.Name, sec.DisplayName, sec.ListingMode, sec.Visible, sec.When, sec.WhenMode, categories, sec.Footer);
                }
                platforms[plat.ID] = new NewPlatform(plat.Name, plat.MenuName, plat.HomeIcon, plat.ID, plat.Color, plat.Visible, sections, plat.Bundles);
            }

            NewManifest nmanifest = new NewManifest(manifest.Version, manifest.Copyright, platforms, manifest.Message);

            File.WriteAllText(Environment.CurrentDirectory + "\\manifest6.json", JsonConvert.SerializeObject(nmanifest, Formatting.Indented));

            Console.WriteLine("Done! Press any key to exit");
            Console.ReadKey();
        }
    }
}
