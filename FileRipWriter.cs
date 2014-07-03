using System;
using System.Collections.Generic;
using System.Linq;

namespace FileRipper
{
    class FileRipWriter
    {
        // Private Constructor 
        private FileRipWriter()
        {
            _fileRipQueue = new List<FileRip>(); 
        }

        #region Private Properties
        private static FileRipWriter _instance;
        private static List<FileRip> _fileRipQueue;
        #endregion  Private Properties

        // Public "Constructor"
        public static FileRipWriter Instance()
        {
            // instantiate if not instantiate then return instantiation
            return _instance ?? (_instance = new FileRipWriter());
        }

        // Public Method for Logging
        public void WriteFileRip(List<FileRip> alor)
        {
            foreach (var rip in alor)
            {
                _fileRipQueue.Add(rip); 
            }
            Flush(false);
        }

        // Public Method for Logging
        public void WriteFileRip(FileRip rip)
        {
            _fileRipQueue.Add(rip);
            Flush(false);
        }

        // Flushes Queue 
        public void Flush(Boolean isEnd)
        {
            // Thousand Count Limit
            if (_fileRipQueue.Count() < 1000 && !isEnd) return;

            foreach (var rip in _fileRipQueue)
            {
                // Merges
                MergeFileRip(rip);
            }
            // Clears List
            _fileRipQueue.Clear();
        }

        private static void MergeFileRip(FileRip file)
        {
            // Calls stored proc sp_MergeFileRip 
            SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_MergeFileRip", file);
        }
    }
}
