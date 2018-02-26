using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using ReverseGeocoding;
using Microsoft.Data.Sqlite;

namespace ImageOrganizer
{
    class Program
    {


        static void Main(string[] args)
        {
            string baseDirectory = @"D:\Fotos\Takeout\Google Fotos";
            string workingDirectory = @"D:\Fotos\Takeout\Organization";
            string[] dirs = System.IO.Directory.GetDirectories(baseDirectory);//new string[] { "2018-01-14", "2017-12-21" };
            //var geocoder = new ReverseGeocoder(@"D:\cities1000.txt");
            //SqliteConnection Connection = new SqliteConnection("Data Source=hydrodb.sqlite;");
            //Connection.Open();
            //string createTable = ("CREATE TABLE hyddnev (Station UNSIGNED INT(5) NOT NULL, Dat datetime NOT NULL, Stoej int(5) DEFAULT NULL, Vkol UNSIGNED FLOAT(7,3) DEFAULT NULL, PRIMARY KEY (Station, Dat))");
            //SqliteCommand createHydDnev = new SqliteCommand(createTable, Connection);
            //createHydDnev.ExecuteNonQuery();

            ////var query = new SqliteCommand("select name,latitude,longitude,country_code,feature_code from geoloc order by ((latitude - @lat) * (latitude - @lat) + (longitude - @lng) * (longitude - @lng)) LIMIT 1, cn");
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach (string dir in dirs)
            {
                string photoAccessDir = Path.Combine(baseDirectory, dir);
                foreach (string file in System.IO.Directory.GetFiles(photoAccessDir))
                {
                    if (file.EndsWith("json"))
                        continue;
                    string imagePath = Path.Combine(photoAccessDir, file);
                    try
                    {
                        IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(imagePath);

                        var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                        DateTime dateTime = DateTime.MinValue;
                        try
                        {
                            dateTime = subIfdDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);

                        }
                        catch (Exception)
                        {
                            dateTime = File.GetCreationTime(imagePath);

                        }

                        //var subifdir = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                        //var model = subifdir.GetString(ExifDirectoryBase.TagModel);
                        //var gps = directories
                        //         .OfType<GpsDirectory>()
                        //         .FirstOrDefault();

                        //var location = gps.GetGeoLocation();
                        string destinationimageDirectory = Path.Combine(workingDirectory, dateTime.ToString("yyyyMM"));
                        if (!dictionary.ContainsKey(destinationimageDirectory))
                        {
                            dictionary.Add(destinationimageDirectory, 1);
                        }
                        string destinationimagePath = Path.Combine(destinationimageDirectory, "IMG-" + dictionary[destinationimageDirectory].ToString("D4") + Path.GetExtension(imagePath));
                        dictionary[destinationimageDirectory]++;
                        if (!System.IO.Directory.Exists(destinationimageDirectory))
                        {
                            System.IO.Directory.CreateDirectory(destinationimageDirectory);
                        }
                        File.Copy(imagePath, destinationimagePath, true);
                        Console.WriteLine(destinationimagePath);
                    }catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }


        }
    }
}
