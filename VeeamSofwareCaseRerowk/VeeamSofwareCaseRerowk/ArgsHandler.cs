using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeeamSoftwareFirstCase
{
    class ArgsHandler
    {
        private string[] _args;
        public ArgsHandler(string[] args)
        {
            _args = args;
        }
        internal bool ReadArgs()
        {
            if (_args.Count<string>() != 2)
            {
                Console.WriteLine("Введите аргументы VeeamSoftwareFirstCase <path> <blockSize>");
                return false;
            }
            if (!Int32.TryParse(_args[1], out Program.BlockSize))
            {
                Console.WriteLine("Размер блока указан с ошибками. Невозможно преобразовать в число");
                return false;
            }
            if (!File.Exists(_args[0]))
            {
                Console.WriteLine("Файл по адресу {0} не существует", _args[0]);
                return false;
            }
            Program.Path = _args[0];
            return true;
        }
    }
}