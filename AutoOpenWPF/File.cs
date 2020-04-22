using System;
using System.Collections.Generic;
using System.Text;

namespace AutoOpen
{
    class File
    {
        private string _fileName;
        private string _filePath;
        private bool _isSelected;

        public bool isSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public string fileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public string filePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        public File(string fileName, string filePath)
        {
            _isSelected = false;
            _fileName = fileName;
            _filePath = filePath;
        }
    }
}
