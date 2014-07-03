using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace FileRipper
{
    /** FileRip - Obtains and stores file information
     * @param Id - Id of file given in database
     * @param Name - Name of file including extension
     * @param Directory - Path to file
     * @param Server - Server file is on
     * @param DateCreate - date file created
     */

    internal class FileRip
    {
        #region Private Properties
        private LogWriter _logWriter;
        private FileRipWriter _fileRipWriter;
        #endregion Private Properties

        #region Public Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string Directory { get; set; }
        public string Server { get; set; }
        public DateTime LastWrite { get; set; }
        #endregion Public Properties 

        // Wrapper for FindTheFiles 
        public void Rip(string directory, string[] ext)
        {
            // Instantiate Writers
            _logWriter = LogWriter.Instance();
            _fileRipWriter = FileRipWriter.Instance();

            // Find the Files
            FindTheFiles(directory, ext);

            // Final Flush 
            _fileRipWriter.Flush(true);
            _logWriter.Flush(true);
        }

        #region Helpers

        private void FindTheFiles(string directory, string[] ext)
        {
            try
            {
                var info = new DirectoryInfo(directory);
                var results = new List<FileRip>();

                // If empty Array, then check all files
                if (!ext.Any()) ext = new[] {"*"};

                // For each extension given
                foreach (var extension in ext)
                {
                    // Obtains file info
                    var files = info.GetFiles("*." + extension);

                    // Print Extensions All Pretty Like
                    var fileType = extension + " ";
                    if (extension.Equals("*")) fileType = "";

                    // Prints Helpful Message
                    Console.WriteLine(@"There are {0} {1}files in {2}", files.Count(), fileType, info.FullName);

                    // Logs Files and General Stats
                    _logWriter.LogFileRip("INFO",
                        string.Format("There are {0} {1}files in {2}", files.Count(), fileType, info.FullName),
                        Dns.GetHostName());

                    // Rips All Files in Directory
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var fileInfo in files)
                    {
                        results.Add(new FileRip
                        {
                            Name = fileInfo.Name,
                            Directory = fileInfo.DirectoryName,
                            LastWrite = fileInfo.LastWriteTime,
                            Server = Dns.GetHostName()
                        });
                    }
                }

                // Recursively Rips all subdirectories

                foreach (var directoryInfo in info.GetDirectories())
                {
                    FindTheFiles(directoryInfo.FullName, ext);
                }


                AddToDataBase(results);

            }
            catch (PathTooLongException e)
            {
                Console.WriteLine("ERROR: {0} is too long. " + e.Message, directory);
                _logWriter.LogFileRip("ERROR", directory + " is too long. " + e.Message, Dns.GetHostName());
            }
            catch (NotSupportedException e)
            {
                _logWriter.LogFileRip("ERROR", "Not Supported: " + e.Message, Dns.GetHostName());
                Console.WriteLine(@"ERROR: Not Supported. " + e.Message);
                //return new List<FileRip>();
            }
            catch (ArgumentException e)
            {
                _logWriter.LogFileRip("ERROR", "Invalid Argument: " + e.Message, Dns.GetHostName());
                Console.WriteLine(@"ERROR: Invalid Argument. " + e.Message);
                //return new List<FileRip>();
            }
            catch (DirectoryNotFoundException e)
            {
                _logWriter.LogFileRip("ERROR", "File Not Found: " + e.Message, Dns.GetHostName());
                Console.WriteLine(@"ERROR: File Not Found. " + e.Message);
                //return new List<FileRip>();
            }
            catch (UnauthorizedAccessException e)
            {
                _logWriter.LogFileRip("ERROR", "Unauthorized Access to a directory or subdirectory: " + e.Message, Dns.GetHostName());
                Console.WriteLine(@"ERROR: Unauthorized Access to a directory or subdirectory. " + e.Message);
                //return new List<FileRip>();
            }
        }


        #region Database

        public void AddToDataBase(List<FileRip> rips)
        {
            _fileRipWriter.WriteFileRip(rips);
        }

        #endregion Database

        // Overwritten for more useful information
        public override string ToString()
        {
            return "Name: " + Name + "\nDirectory: " + Directory + "\nDate Created: " + LastWrite + 
                "\nServer: " + Server;
        }

        #endregion Helpers

    }
}
