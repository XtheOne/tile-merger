namespace ImageMerger
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text.RegularExpressions;

    public class ImageLoader
    {
        private ImageLoader()
        {
            throw new Exception("No need to create an object of this class, all its methods are static.");
        }


        public static void NumericalSort(string[] ar)
        {
            Regex rgx = new Regex("([^0-9]*)([0-9]+)");
            Array.Sort(ar, (a, b) =>
            {
                var ma = rgx.Matches(a);
                var mb = rgx.Matches(b);
                for (int i = 0; i < ma.Count; ++i)
                {
                    int ret = ma[i].Groups[1].Value.CompareTo(mb[i].Groups[1].Value);
                    if (ret != 0)
                        return ret;

                    ret = int.Parse(ma[i].Groups[2].Value) - int.Parse(mb[i].Groups[2].Value);
                    if (ret != 0)
                        return ret;
                }

                return 0;
            });
        }
        public static void DisposeImages(List<Bitmap> images)
        {
            foreach (Bitmap bitmap in images)
            {
                bitmap.Dispose();
            }
        }

        public static List<string> ListFiles(string folderPath, string filter)
        {
            string[] files = Directory.GetFiles(folderPath);
            NumericalSort(files);
            List<string> list = new List<string>();
            foreach (string str in files)
            {
                if (filter.Length > 0)
                {
                    if (str.ToLower().Contains(filter))
                    {
                        list.Add(str);
                    }
                }
                else
                {
                    list.Add(str);
                }
            }
            return list;
        }

        public static List<Bitmap> LoadImages(List<string> sortedFiles)
        {
            List<Bitmap> list = new List<Bitmap>();
            foreach (string str in sortedFiles)
            {
                if (File.Exists(str))
                {
                    try
                    {
                        Bitmap item = new Bitmap(str);
                        list.Add(item);
                    }
                    catch (Exception)
                    {
                    }
                    Console.WriteLine("{0} loaded.", str);
                }
                else
                {
                    Console.WriteLine("{0} is not a valid file or directory.", str);
                }
            }
            return list;
        }
    }
}

