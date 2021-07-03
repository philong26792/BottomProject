using System;
using System.Linq;
using System.Text;

namespace Bottom_API.Helpers
{
    public static class CodeUtility
    {
            // *===========================================*=================================================*//
            /// <summary>
            /// Trả về dạng string có n chữ số
            /// </summary>
            /// <param name="length">Độ dài cần nhập </param>
            /// <returns>Trả về dạng string có n chữ số.</returns>
            public static string RandomNumber(int length)
            {
                Random random = new Random();
                const string chars = "0123456789";
                return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            // *===========================================*=================================================*//
            /// <summary>
            /// Trả về dạng string có n chữ cái in Hoa
            /// </summary>
            /// <param name="length">Độ dài cần nhập </param>
            /// <returns>Trả về dạng string có n chữ cái in Hoa.</returns>
            public static string RandomStringUpper(int length)
            {
                Random random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            // *===========================================*=================================================*//
            /// <summary>
            /// Nhập vào 1 string(str) và 1 độ dài (n)
            /// Trả về dạng string = (srt)+(2 chữ số đầu tiên của năm)+(2 chữ số đầu tiên tháng)+(2 chũ số ngày) + Random n chữ cái In Hoa
            /// </summary>
            /// <param name="str" length="size">String và độ dài </param>
            /// <returns>(srt)+(2 chữ số đầu tiên của năm)+(2 chữ số đầu tiên tháng)+(2 chũ số ngày) + Random n chữ cái In Hoa</returns>
            public static string RandomReceiveNo(string str, int size){
                var datetimeNow = DateTime.Now;
                var year = datetimeNow.Year.ToString();
                var month = datetimeNow.Month;
                var day = datetimeNow.Day;
                var arrayYear = year.ToCharArray().Select(c => c.ToString()).ToArray();
                var yearString = arrayYear[2] + arrayYear[3];
                var monthString = "";
                var dayString = "";
                if (month >= 10){
                    monthString = month.ToString();
                }
                else{
                    monthString = "0" + month;
                }

                if (day >= 10){
                    dayString = day.ToString();
                }
                else{
                    dayString = "0" + day;
                }
                StringBuilder builder = new StringBuilder();
                Random random = new Random();
                char ch;
                for (int i = 0; i < size; i++){
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    builder.Append(ch);
                }
                var stringResult = str + yearString + monthString + dayString + builder.ToString().ToUpper();
                return stringResult;
        }

         // *===========================================*=================================================*//
    }
}