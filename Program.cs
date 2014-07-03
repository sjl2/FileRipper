using System;
using System.Linq;
using System.Net;


namespace FileRipper
{
    /** Program
     * FileRipper's central Class
     */
    class Program
    {
        #region Helpers 

        /** GetInput
         * @param prompt - prompt for the user
         * Returns the user's input based on the given prompt
         */
        static string GetInput(string prompt)
        {
            try
            {  
                Console.Write(prompt);
                var result = "";
                var line = Console.ReadLine();

                // Process Line
                if (!String.IsNullOrEmpty(line))
                {
                    result = line.Trim();
                }

                return result;
            }
            catch (OverflowException e)
            {
                Console.WriteLine(e.Message + @" Please try again.");
                return GetInput(prompt); 
            }
        }

        #endregion Helpers

        /** Main
         * @param args - Potential args that could be a directory or extension
         * Executes and Processes FileRip depending on how user calls program 
         */
        static void Main(string[] args)
        {
                if (args == null) throw new ArgumentNullException("args");

                var writer = LogWriter.Instance();
                writer.LogFileRip("INFO", "Beginning FileRipper", Dns.GetHostName());
            try
            {
                // Initialize FileRip Class 
                var fileRip = new FileRip();
                var directory = "";
                var extensions = "";
                var ext = new string[0]; 

                // Two Arguments
                switch (args.Count())
                {
                    case 2:
                    {
                        // Arguments
                        directory = args[0];
                        extensions = args[1];
                        if (extensions != null) ext = new [] {extensions};
                    }
                        break;
                    case 1:
                    {
                        // Argument
                        directory = args[0];
                    }
                        break;
                    case 0:
                    {
                        // No Arguments? Prompt User!
                        Console.WriteLine(@"Hello! Welcome to File Rip.");

                        // Directory Prompt
                        Console.WriteLine(@"Please input a Directory and hit enter.");
                        directory = GetInput("Directory: ");

                        // Extension Prompt
                        Console.WriteLine(@"Please input an extension and hit enter.");
                        Console.WriteLine(@"   To search all extensions, leave blank.");
                        Console.WriteLine(@"   To search multiple extensions, separate each one by a comma. (EX: ext1,ext2)");
                        extensions = GetInput("Extension: ");

                        if (extensions != null)
                        {                           
                            ext = extensions.Split(new [] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
                        }

                        // Proccess extensions string for logging
                        for (var i = 0; i < ext.Count(); i++)
                        {
                            if (i == 0)
                            {
                                extensions = ext[0]; // First term
                            }
                            else
                            {
                                extensions += (", " + ext[i]); // Other terms
                            }
                        }
                    }
                        break;
                    default:
                        if (args.Count() < 0) // Incorrect Number of Arguments
                        {
                            Console.WriteLine(@"Incorrect Number of Arguments.");
                            Console.WriteLine(@"FileRipper <directory path> <extension>");
                            writer.LogFileRip("ERROR", "Incorrect number of arguments used.", Dns.GetHostName());
                        }
                        else
                        {
                            // Initialize ext
                            ext = new string[args.Count()-1];

                            // Define Directory
                            directory = args[0];

                            // For each extension
                            for (var i = 1; i < args.Count(); i++)
                            {
                                ext[i - 1] = args[i];

                                if (i == 1) // First Term in series
                                {
                                    extensions = args[i].Trim();
                                }
                                else // Everything else needs commas
                                {
                                    extensions = extensions + ", " + args[i].Trim();
                                }
                            }
                        }
                        break;
                }

                // Changes to all extensions 
                if (extensions != null && extensions.Equals(""))
                {
                    extensions = "All";
                }

                Console.WriteLine(@"Directory: {0} Extension(s): {1}", directory, extensions);
                writer.LogFileRip("INFO", string.Format("Directory: {0}\tExtension(s): {1}", directory, extensions), Dns.GetHostName());

                // Finds the Files (Returns List of FileRips) 
                fileRip.Rip(directory, ext);

                Console.WriteLine(@"Done!");
                writer.LogFileRip("INFO", "Done!", Dns.GetHostName());
                Console.Read();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(@"ERROR: Null was used as input. " + e.Message);
                writer.LogFileRip("ERROR", "Null was used as input. " + e.Message, Dns.GetHostName());
            }
        }
    }
}
