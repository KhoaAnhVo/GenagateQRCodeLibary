using System;
using QrCodeManager;
using System.Collections.Generic;
using System.Data;

namespace ConsoleCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {
            QrCodeControl QrControl = new QrCodeControl();
            Console.WriteLine("Start");
            //fiels
            string cmd="";
            string result="";
            #region FORTEST
            printF1("Range input: ");
            cmd = Console.ReadLine();
            List<string> RS = QrControl.ListQRCodeBuff(cmd);
            var RSTb = QrControl.QRCodeTable(RS, false);
            PrintTable(RSTb);
        #endregion FORTEST


        CMD:
            #region COMMAND
            cmd = "";
            result = "";
            Console.WriteLine();
            Console.Write("CMD >> ");
            cmd = Console.ReadLine().ToLower();
            switch (cmd)
            {
                case "help":
                    printF1("HELP\n");
                    goto HELP;
                case "csc": //Create single code
                    printF1("Create a single code\n");
                    goto CreateASingleCode;
                
                case "crc":
                    printF1("Create a range code\n");
                    goto CreateRangeCode;

                case "exit":
                    printF1("EXIT");
                    goto EXIT;
                case "clear":
                    Console.Clear();
                    goto CMD;
            }
            printF1("Sai cú pháp!\n");
            
            goto CMD;
        #endregion COMMAND
            
        HELP:
            #region HELP
            Console.ReadLine();
            goto CMD;
        #endregion HELP

        CreateASingleCode:
            #region CREATESINGLECODE
            printF1("Index of code: ");
            cmd = Console.ReadLine();
            result = QrControl.GetSingleCode(cmd);
            printResult(result);
            goto CMD;
        #endregion CREATESINGLECODE

        CreateRangeCode:
            #region CreateRangeCode
            printF1("Range input: ");
            cmd = Console.ReadLine();
            List<string> List_result = QrControl.ListQRCodeBuff(cmd);
            foreach (string item in List_result)
            {
                printF1(item + "\n");
            }
        #endregion CreateRangeCode

        EXIT:
            Console.ReadKey();
        }

        static void printF1(string txt)
        {
            Console.Write(string.Format(">> {0}", txt));
        }
        static void printResult(string result)
        {
            Console.WriteLine(string.Format("   Result: {0}",result));
        }
        static void PrintTable(DataTable data)
        {
            foreach (DataRow dataRow in data.Rows)
            {
                foreach (var item in dataRow.ItemArray)
                {
                    Console.WriteLine(item);
                }
            }
        }
        static void PrintList(List<string> data)
        {
            foreach (string item in data)
            {
                printF1(item + "\n");
            }
        }
        
    }
}

