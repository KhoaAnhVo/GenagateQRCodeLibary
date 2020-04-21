using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using QRCoder;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.IO;
using System.Drawing.Imaging;

namespace QrCodeManager
{
    public class QrCodeControl
    {
        #region  Setup Code
        /// <summary>
        /// Năm, Tháng, Ngày tạo Code
        /// </summary>
        public string DateCreate { get; set; } = DateTime.Now.ToString("yyMMdd");

        /// <summary>
        /// Đối tượng mà code ráng cho. Mật định = 2;
        /// 0 - Pallet; 1 - Carton; 2 - Product
        /// </summary>
        public string UseCase { get; set; } = "2";

        /// <summary>
        /// Loại sản phẩm.
        /// 0 - Round; 1 - Square; 2 - Sachet
        /// </summary>
        public string ProType { get; set; } = "0";

        /// <summary>
        /// Số hiệu của Line.
        /// range (0-9)
        /// </summary>
        public string LineNumber { get; set; } = "5";//1 char

        /// <summary>
        /// Số lần tạo code trong ngày
        /// </summary>
        public string CreateNo { get; set; } = "01"; //2 chars

        /// <summary>
        /// Tổng số code tạo ra trong 1 lần tạo.
        /// </summary>
        public string RangeNumber { get; set; } = "000000100";
        #endregion

        #region More info about code
        public string AGIcode { get; set; }
        public string ProductName { get; set; }
        public string PackSize { get; set; }
        public enum PackingLine
        {
            Line1 = 1,
            Line2 = 2
        }
        #endregion

        /// <summary>
        /// Get a single code form index of range number
        /// </summary>
        /// <param name="rangenumber"></param>
        /// <returns></returns>
        public string CreateSingleCode(string IndexInRangeNumber)
        {
            string code = "";
            code += DateCreate;
            code += ProType;
            code += LineNumber;
            code += CreateNo;
            string buff = "0";
            for (int j = 0; j <= (7 - IndexInRangeNumber.ToString().Length); j++) //Cần cải thiện vòng lập này.
            {
                buff += "0";
            }
            code += buff;
            code += IndexInRangeNumber;

            return code;
        }

        /// <summary>
        /// Tạo danh sách code 
        /// </summary>
        /// <param name="rangenumber"></param>
        /// <returns></returns>
        public List<string> CreateListQrCode(string range)
        {
            Int32 rangenumber = Convert.ToInt32(range);
            List<string> result = new List<string>();
            DateCreate = DateTime.Now.ToString("yyMMdd");
            RangeNumber = range;
            for (Int32 i = 1; i <= rangenumber; i++)
            {
                result.Add(CreateSingleCode(i.ToString()));
            }
            return result;
        }

        /// <summary>
        /// Tạo danh sách hình ảnh QR code
        /// </summary>
        /// <param name="source">List code</param>
        /// <param name="pixels">Kích thước code</param>
        /// <returns></returns>
        public List<Bitmap> CreateListQrImages(List<string> source, Int32 pixels)
        {
            List<Bitmap> result = new List<Bitmap>();
            foreach (string item in source)
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(item, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(pixels);
                //add bitmap vao trong list
                result.Add(qrCodeImage);
            }
            return result;
        }

        #region Lưu dữ liệu code vào DataTable với các cột: Index, Code; HashcodeMD5; HashCodeSHA1, HascodeSHA256

        /// <summary>
        /// The list of 20 uinique chars
        /// </summary>
        protected List<string> DataCode { get; }
        /// <summary>
        /// Content the MD5 hash code of the DataCode
        /// </summary>
        protected List<string> DataCodeMD5 { get; }
        /// <summary>
        /// Content the SHA1 hash code of the DataCode
        /// </summary>
        protected List<string> DataCodeSHA1 { get; }
        /// <summary>
        /// Content the SHA256 hash code of the DataCode
        /// </summary>
        protected List<string> DataCodeSHA256 { get; }

        /// <summary>
        /// Create buffer table content code, the hash code and the QRcode data
        /// </summary>
        /// <param name="buff_list_code">List code buffer</param>
        /// <returns></returns>
        public DataTable CreateQrCodeTable(List<string> buff, bool IsQRimageData)
        {
            //Create a table have 4 columns
            using (DataTable table = new DataTable())
            {
                table.Columns.Add("Id", typeof(Int32));
                table.Columns.Add("CodeData", typeof(string));
                table.Columns.Add("MD5CodeData", typeof(string));
                table.Columns.Add("SHA1CodeData", typeof(string));
                table.Columns.Add("SHA256CodeData", typeof(string));
                if (IsQRimageData)
                {
                    table.Columns.Add("QRImageData", typeof(Bitmap));
                    foreach (string item in buff)
                    {
                        table.Rows.Add(
                            buff.IndexOf(item)
                            , item
                            , ToCodeMD5(item)
                            , ToCodeSHA1(item)
                            , ToCodeSHA256(item)
                            , CreateBitmapQrCode(item)
                            );
                    }
                }
                else
                {
                    table.Columns.Add("QRImageData", typeof(byte[]));
                    foreach (string item in buff)
                    {
                        table.Rows.Add(
                            buff.IndexOf(item)
                            , item
                            , ToCodeMD5(item)
                            , ToCodeSHA1(item)
                            , ToCodeSHA256(item)
                            , ConvertImageToByte(CreateBitmapQrCode(item))
                            );

                    }
                }
                return table;
            }
        }

        #endregion

    #region QRcode genarate
        /// <summary>
        /// Seting size of QR code image
        /// </summary>
        public int pixelsPerModule_QRpara { get; set; } = 4;

        /// <summary>
        /// Create QRcode Image form text data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Bitmap CreateBitmapQrCode(string data)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(pixelsPerModule_QRpara);
                return qrCodeImage;
            }
        }

        public byte[] CreateBytePngQrCode(string data)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(pixelsPerModule_QRpara);
                return qrCodeAsPngByteArr;
            }
        }


        /// <summary>
        /// Convert data image byte array to bitmap
        /// </summary>
        /// <param name="data">byte array data image</param>
        /// <returns></returns>
        public Bitmap ConvertBytesToImage(byte[] data)
        {
            //using (MemoryStream stream = new MemoryStream(data))
            //{
            //    stream.Position = 0;
            //    Bitmap bmp = new Bitmap(stream,true);
            //    return bmp;
            //}
            Bitmap bmp = new Bitmap(new System.IO.MemoryStream(data));
            return bmp;
        }

        
        
        /// <summary>
        /// Convert bitmap data to byte array
        /// </summary>
        /// <param name="bmp">bitmap data</param>
        /// <returns></returns>
        public byte[] ConvertImageToByte(Bitmap bmp)
        {
            var size = bmp.Width * bmp.Height / 8;
            var buffer = new byte[size];

            var i = 0;
            for (var y = 0; y < bmp.Height; y++)
            {
                for (var x = 0; x < bmp.Width; x++)
                {
                    var color = bmp.GetPixel(x, y);
                    if (color.B != 255 || color.G != 255 || color.R != 255)
                    {
                        var pos = i / 8;
                        var bitInByteIndex = x % 8;

                        buffer[pos] |= (byte)(1 << 7 - bitInByteIndex);
                    }
                    i++;
                }
            }

            return buffer;
        }
    #endregion QRcode genarate
        #region Mã hóa và giải mã code bằng thuật toán MD5
        /// <summary>
        /// Key for encrypt and decrypt MD5 algorithm
        /// </summary>
        static string key_MD5 { get; set; } = "A!9HHhi%XjjYY4YP2@Nob009X";

        /// <summary>
        /// Encrypt code by MD5 algorithm
        /// </summary>
        /// <param name="text">Text code to Encrypt</param>
        /// <returns></returns>
        public static string EncryptMD5(string text)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key_MD5));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        //byte[] bytes = transform.TransformFinalBlock(textBytes, 0, 20);
                        return Convert.ToBase64String(bytes, 0, bytes.Length);
                        //return Convert.ToBase64String(bytes, 0, 20);
                    }
                }
            }
        }

        /// <summary>
        /// Decrypt code by MD5 algorithm
        /// </summary>
        /// <param name="text">Text code to Encrypt</param>
        /// <returns></returns>
        public static string DecryptMD5(string cipher)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key_MD5));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipher);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        //byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, 20);
                        return UTF8Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }
        #endregion

        #region Hash code MD5, SHA1, SHA256
        /// <summary>
        /// Hash the text by MD5 algorithm
        /// </summary>
        /// <param name="text"></param>
        /// <returns>A string 31 chars</returns>
        public string ToCodeMD5(string text)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(text);
            bs = md5.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x1").ToLower());
            }
            text = s.ToString();
            return text;
        }
        /// <summary>
        /// Hash the text by SHA1 algorithm
        /// </summary>
        /// <param name="text"></param>
        /// <returns>A string 40 chars</returns>
        public string ToCodeSHA1(string text)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(text);
            bs = sha1.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x1").ToLower());
            }
            text = s.ToString();
            return text;
        }
        /// <summary>
        /// Hash the text by SHA256 algorithm
        /// </summary>
        /// <param name="text"></param>
        /// <returns>A string 64 chars</returns>
        public string ToCodeSHA256(string text)
        {
            SHA256CryptoServiceProvider sha1 = new SHA256CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(text);
            bs = sha1.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x1").ToLower());
            }
            text = s.ToString();
            return text;
        }

        #endregion
    }
    /// <summary>
    /// Xử lý giao tiếp với Database
    /// </summary>
    public class QrCodeDataBase
    {
        //Tạo thông tin kết nối đến csdl 
        SqlConnectionStringBuilder SqlBuilder = new SqlConnectionStringBuilder();
        public QrCodeDataBase()
        {
            SqlBuilder.DataSource = "localhost";
            SqlBuilder.UserID = "Admin_Khoa";
            SqlBuilder.Password = "khoanhvo";
            SqlBuilder.InitialCatalog = "Syngenta_test";
        }
        /// <summary>
        /// Excute query cmd form text
        /// </summary>
        /// <param name="cmd_txt">Text Command</param>
        public bool SqlCmdExcuteQuery(StringBuilder cmd_txt)
        {
            bool result;
            try
            {
                using (SqlConnection conn = new SqlConnection(SqlBuilder.ConnectionString))
                {
                    conn.Open(); // thuc hien ket noi voi sql
                    using (SqlCommand cmd = new SqlCommand(cmd_txt.ToString(), conn))
                    {
                        cmd.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            catch (SqlException ex)
            {
                //MessageBox.Show(ex.ToString());
                result = false;
            }
            return result;
        }
        /// <summary>
        /// Excute query cmd from _.sql file save in direction source folder..\bin\Debug
        /// </summary>
        /// <param name="query_file_name">File .sql name</param>
        /// <returns></returns>
        public bool SqlCmdExcuteQuery(string query_file_name)
        {
            bool result;
            //get direction to source code
            string Path = Environment.CurrentDirectory;
            string cmd_txt = File.ReadAllText(Path + @"\" + query_file_name);
            try
            {
                using (SqlConnection conn = new SqlConnection(SqlBuilder.ConnectionString))
                {
                    conn.Open(); // thuc hien ket noi voi sql
                    using (SqlCommand cmd = new SqlCommand(cmd_txt, conn))
                    {
                        cmd.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            catch (SqlException ex)
            {
                //MessageBox.Show(ex.ToString());
                result = false;
            }
            return result;
        }
        /// <summary>
        /// Get list of tables in database
        /// </summary>
        /// <returns></returns>
        public List<string> GetListTableDB()
        {
            List<string> result = new List<string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(SqlBuilder.ConnectionString))
                {
                    string cmd_txt = string.Format(@"select TABLE_NAME from information_schema.tables;");
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(cmd_txt, conn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {

            }
            return result;
        }
        //Save dữ liệu lên 
    }
}
