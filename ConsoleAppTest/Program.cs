using System;
using QrCodeManager;
using System.Collections.Generic;
using System.Drawing;
using SQLServerSupportLib;

namespace ConsoleCoreTest
{
    class Program
    {

        public static void Main(string[] args)
        {
            #region Fiels
            QrCodeControl QrControl = new QrCodeControl();
            Console.WriteLine("Start");
            string cmd = "";
            string result = "";
            string path = @"K:\DebugTest\Convert\";
            List<string> list_result;
            Bitmap bmp;
            byte[] byteData;
            #endregion Fiels

            #region FOR TEST
            QrCodeDataBase QrDB = new QrCodeDataBase();
        H:
            var DB = QrControl.CreateBytePngQrCode("VO ANH KHOA");
            var A = QrControl.ConvertBytesToImage(DB);
            proCS.saveImage("K:\\","test",A);
            proCS.OpenFolder("K:\\");
            Console.ReadKey();
            goto H;
        #endregion FOR TEST


        CMD:
            #region COMMAND
            cmd = "";
            result = "";
            Console.WriteLine();
            Console.Write("CMD >> ");
            cmd = Console.ReadLine().ToLower();
            switch (cmd)
            {
                case "csc":
                    proCS.printF1("Create a single code\n");
                    goto CSC;

                case "crc":
                    proCS.printF1("Create a range full code\n");
                    goto CRC;
                case "clc":
                    proCS.printF1("Create a list range code\n");
                    goto CLC;
                case "sci":
                    proCS.printF1("Show code info\n");
                    goto SCI;
                case "simg":
                    proCS.printF1("Create single code image .png and save\n");
                    goto SIMG;
                case "abc":
                    proCS.printF1("Test\n");
                    goto ABC;


                case "help":
                    proCS.printF1("HELP\n");
                    goto HELP;
                case "exit":
                    proCS.printF1("EXIT");
                    goto EXIT;
                case "clear":
                    Console.Clear();
                    goto CMD;
            }
            proCS.printF1("Sai cú pháp!\n");

            goto CMD;
        #endregion COMMAND

        SCI:
            #region Modified info code
            //print code info present
            ShowCodeInfo(QrControl);
            //Update info

            //ShowCodeInfo(QrControl);
            goto CMD;
        #endregion Modified info code

        HELP:
            #region HELP
            string help =
                "       csc     : Create a single code\n" +
                "       crc     : Create a range code\n" +
                "       clc     : Create a list range code\n" +
                "       mic     : Show code info\n" +
                "       simg    : Creat single code + Create bitmap + Save image .png\n" +
                "           " +
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
            proCS.printF1("Index of code: ");
            cmd = Console.ReadLine();
            result = QrControl.CreateSingleCode(cmd);
            proCS.printResult(result);
            goto CMD;
        #endregion CREATE A SINGLE CODE

        CRC:
            #region Create A Range Code
            proCS.printF1("Range input: ");
            cmd = Console.ReadLine();
            List<string> RS = QrControl.CreateListQrCode(cmd);
            var RSTb = QrControl.CreateGroupCodeTable(RS);
            proCS.PrintTable(RSTb);
            goto CMD;
        #endregion CreateRangeCode

        CLC:
            #region Create list code
            proCS.printF1("Index of code: ");
            cmd = Console.ReadLine();
            list_result = QrControl.CreateListQrCode(cmd);
            proCS.PrintList(list_result);
            goto CMD;
        #endregion Create list code

        SIMG:
            #region Create Code ==> Bitmap ==> Save .png
            proCS.printF1("Index code input: ");
            cmd = Console.ReadLine();

            try
            {
                path = @"K:\DebugTest\SingleCode\";
                cmd = QrControl.CreateSingleCode(cmd);
                bmp = QrControl.CreateBitmapQrCode(cmd);
                proCS.saveImage(path, cmd, bmp);
                proCS.OpenFolder(path);
            }
            catch (Exception e)
            {
                proCS.printResult(e.ToString() + "\n");
            }
            goto CMD;
        #endregion SIMG

        ABC:
            #region TEST
            proCS.printF1("Code index input:\n");
            cmd = Console.ReadLine();
            // create a string code
            cmd = QrControl.CreateSingleCode(cmd);
            proCS.printF2("Code Created", cmd + "\n");

            // create bitmap by code string
            bmp = QrControl.CreateBitmapQrCode(cmd);
            proCS.printF2("Create bitmap data", "OK\n");

            //save to K:\DebugTest\Convert
            proCS.saveImage(path, cmd, bmp);
            proCS.printF2("Save QR Code Image to", path + "\n");

            //Create byte data
            byteData = QrControl.CreateBytePngQrCode(cmd);
            proCS.printF2("Create QRcode type png Byte[]", "OK\n");

            //Create byte data
            bmp = QrControl.ConvertBytesToImage(byteData);
            proCS.printF2("Convert Byte[] Array to Bitmap", "OK\n");

            //save to K:\DebugTest\Convert
            proCS.saveImage(path, "C" + cmd, bmp);
            proCS.printF2("Save QR Code Image to", path + "\n");

            //save byte[] data to .txt
            proCS.printF2("Byte Array Data", "OK\n");

            proCS.writeTextToFile(path, cmd + ".txt", byteData);

            proCS.OpenFolder(@"K:\DebugTest\Convert\");
            goto CMD;
        #endregion TEST

        EXIT:
            Console.ReadKey();
        }

        static void ShowCodeInfo(QrCodeControl qrCodeControl)
        {
            String cmd;
            Console.WriteLine("     Unique char array format: <[1](6 chars) [2](1 char) [3](1 char) [4](1 char) [5](2char) [6](9 chars)>");
            Console.WriteLine("     Code: " + qrCodeControl.CreateSingleCode(qrCodeControl.RangeNumber));
            cmd = qrCodeControl.DateCreate;
            proCS.printF2("[1] Date Create", cmd);

            cmd = qrCodeControl.UseCase;
            proCS.printF2("[2] Use Case", cmd);

            cmd = qrCodeControl.ProType;
            proCS.printF2("[3] Product type", cmd);

            cmd = qrCodeControl.LineNumber;
            proCS.printF2("[4] Line No", cmd);

            cmd = qrCodeControl.CreateNo;
            proCS.printF2("[5] Create No", cmd);

            cmd = qrCodeControl.RangeNumber;
            proCS.printF2("[6] Range", cmd);
        }

        static void test_CreateListCode2(QrCodeControl qrCodeControl)
        {
        H:
            string cmd;
            proCS.printF1("Range input:");
            string abc;
            abc = Console.ReadLine();
            cmd = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var L = qrCodeControl.CreateListQrCode1(cmd, abc);
            proCS.PrintList(L);
            Console.WriteLine();
            Console.ReadKey();
            goto H;
        }


        static void test_UpdateDbGroupCodeInfo()
        {
            QrCodeDataBase QrDB = new QrCodeDataBase();
            try
            {
                QrDB.UpdateDbGroupCodeInfo("1234567890", "NV007", "AGI code", "1010", DateTime.Now.ToString());

            }
            catch (Exception e)
            {
                proCS.printF1(e.ToString());
            }
            Console.ReadKey();
        }
    }
}

