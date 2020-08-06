using PhotoOrganizer.Utils;
using System;
using System.Collections.Generic;

namespace PhotoOrganizer.Tests.Utils
{
    public class TestConsoleWrapper : IConsoleWrapper
    {
        private int _readKeyIndex = 0;
        private readonly List<ConsoleKeyInfo> _readKeyList;

        private int _readLineIndex = 0;
        private readonly List<string> _readLineList;

        public TestConsoleWrapper(List<ConsoleKeyInfo> readKeyList, List<string> readLineList)
        {
            _readKeyList = readKeyList;
            _readLineList = readLineList;
        }

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            if (_readKeyList is null || _readKeyList.Count == 0)
            {
                return new ConsoleKeyInfo((char)ConsoleKey.Enter, ConsoleKey.Enter, false, false, false);
            }
            return _readKeyList[_readKeyIndex++];
        }

        public string ReadLine()
        {
            if (_readLineList is null || _readLineList.Count == 0)
            {
                return null;
            }
            return _readLineList[_readLineIndex++];
        }

        public void Write(string value)
        {
            Console.Write(value);
        }

        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}
