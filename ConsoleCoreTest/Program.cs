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
            List<string> list_result;
            #region FORTEST

        //printF1("Range input: ");
        //cmd = Console.ReadLine();
        //List<string> RS = QrControl.ListQRCodeBuff(cmd);
        //var RSTb = QrControl.QRCodeTable(RS, false);
        //PrintTable(RSTb);
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
                    goto CSC;
                
                case "crc":
                    printF1("Create a range full code\n");
                    goto CRC;
                case "clc":
                    printF1("Create a list range code\n");
                    goto CLC; ;



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
            string help = 
                "       csc     : Create a single code\n" +
                "       crc     : Create a range code\n" +
                "       clc     : Create a list range code\n" +
                "       " +
                "" +
                "" +
                "\n" +
                "       clear   : clear grean!\n" +
                "       help    : go to help!\n" +
                "       exit    : exit app!\n";
            Console.Write(help);
            goto CMD;
        #endregion HELP

        CSC:
            #region CREATE A SINGLE CODE
            printF1("Index of code: ");
            cmd = Console.ReadLine();
            result = QrControl.GetSingleCode(cmd);
            printResult(result);
            goto CMD;
        #endregion CREATE A SINGLE CODE

        CRC:
            #region Create A Range Code
            printF1("Range input: ");
            cmd = Console.ReadLine();
            List<string> RS = QrControl.ListQRCodeBuff(cmd);
            var RSTb = QrControl.QRCodeTable(RS, false);
            PrintTable(RSTb);
            goto CMD;
        #endregion CreateRangeCode


        CLC:
            #region Create list code
            printF1("Index of code: ");
            cmd = Console.ReadLine();
            list_result = QrControl.ListQRCodeBuff(cmd);
            PrintList(list_result);
            goto CMD;
        #endregion Create list code
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
                    printResult(item.ToString());
                }
            }
        }
        static void PrintList(List<string> data)
        {
            foreach (string item in data)
            {
                printResult(item);
            }
        }
        
    }
}

