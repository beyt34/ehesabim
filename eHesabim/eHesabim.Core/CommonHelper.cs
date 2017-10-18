using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace eHesabim.Core {
    public class CommonHelper {
        private static AspNetHostingPermissionLevel? trustLevel;

        public static bool OneToManyCollectionWrapperEnabled { get; set; }

        public static bool IsValidEmail(string email) {
            if (string.IsNullOrEmpty(email)) {
                return false;
            }

            email = email.Trim();
            var result = Regex.IsMatch(
                email,
                @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            return result;
        }

        public static bool IsValidUrl(string url) {
            if (string.IsNullOrEmpty(url)) {
                return false;
            }

            var result = Regex.IsMatch(
                url,
                @"^(http(s?):\/\/)(www.)?(\w|-)+(\.(\w|-)+)*((\.[a-zA-Z]{2,3})|\.(aero|coop|info|museum|name))+(\/)?$");
            return result;
        }

        public static string GetUrl(string url) {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(url)) {
                retVal = url;
                if (!retVal.Contains("http:")) {
                    retVal = string.Format("http://{0}", retVal);
                }

                if (!IsValidUrl(retVal)) {
                    retVal = string.Empty;
                }
            }

            return retVal;
        }

        public static string GenerateRandomDigitCode(int length) {
            var random = new Random();
            var str = string.Empty;
            for (var i = 0; i < length; i++) {
                str = string.Concat(str, random.Next(10).ToString(CultureInfo.InvariantCulture));
            }

            return str;
        }

        public static int GenerateRandomInteger(int min = 0, int max = 2147483647) {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        public static string EnsureMaximumLength(string str, int maxLength) {
            if (string.IsNullOrEmpty(str)) {
                return str;
            }

            if (str.Length > maxLength) {
                return str.Substring(0, maxLength);
            }

            return str;
        }

        public static string EnsureNumericOnly(string str) {
            if (string.IsNullOrEmpty(str)) {
                return string.Empty;
            }

            var result = new StringBuilder();
            foreach (var c in str) {
                if (char.IsDigit(c)) {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        public static string EnsureNotNull(string str) {
            if (str == null) {
                return string.Empty;
            }

            return str;
        }

        public static bool AreNullOrEmpty(params string[] stringsToValidate) {
            var result = false;
            Array.ForEach(
                stringsToValidate,
                str => {
                    if (string.IsNullOrEmpty(str)) {
                        result = true;
                    }
                });
            return result;
        }

        public static AspNetHostingPermissionLevel GetTrustLevel() {
            if (!trustLevel.HasValue) {
                // set minimum
                trustLevel = AspNetHostingPermissionLevel.None;

                // determine maximum
                foreach (var t in
                    new[]
                        {
                            AspNetHostingPermissionLevel.Unrestricted, AspNetHostingPermissionLevel.High,
                            AspNetHostingPermissionLevel.Medium, AspNetHostingPermissionLevel.Low,
                            AspNetHostingPermissionLevel.Minimal
                        }) {
                    try {
                        new AspNetHostingPermission(t).Demand();
                        trustLevel = t;
                        break; // we've set the highest permission we can
                    }
                    catch (System.Security.SecurityException) {
                    }
                }
            }

            return trustLevel.Value;
        }

        public static void SetProperty(object instance, string propertyName, object value) {
            if (instance == null) {
                throw new ArgumentNullException("instance");
            }

            if (propertyName == null) {
                throw new ArgumentNullException("propertyName");
            }

            var instanceType = instance.GetType();
            var pi = instanceType.GetProperty(propertyName);
            if (pi == null) {
                if (!pi.CanWrite) {
                    if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType)) {
                        value = To(value, pi.PropertyType);
                    }
                }
            }

            pi.SetValue(instance, value, new object[0]);
        }

        public static TypeConverter GetNopCustomTypeConverter(Type type) {
            return TypeDescriptor.GetConverter(type);
        }

        public static object To(object value, Type destinationType) {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        public static object To(object value, Type destinationType, CultureInfo culture) {
            if (value != null) {
                var sourceType = value.GetType();

                var destinationConverter = GetNopCustomTypeConverter(destinationType);
                var sourceConverter = GetNopCustomTypeConverter(sourceType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType())) {
                    return destinationConverter.ConvertFrom(null, culture, value);
                }

                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType)) {
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                }

                if (destinationType.IsEnum && value is int) {
                    return Enum.ToObject(destinationType, (int)value);
                }

                if (!destinationType.IsInstanceOfType(value)) {
                    return Convert.ChangeType(value, destinationType, culture);
                }
            }

            return value;
        }

        public static T To<T>(object value) {
            return (T)To(value, typeof(T));
        }

        public static string ConvertEnum(string str) {
            var result = string.Empty;
            var letters = str.ToCharArray();
            foreach (var c in letters) {
                if (c.ToString(CultureInfo.InvariantCulture) != c.ToString(CultureInfo.InvariantCulture).ToLower()) {
                    result += " " + c.ToString(CultureInfo.InvariantCulture);
                }
                else {
                    result += c.ToString(CultureInfo.InvariantCulture);
                }
            }

            return result;
        }

        public static string EncodePassword(string password) {
            var bytePassword = Encoding.Unicode.GetBytes(password);
            var bytePasswordSalt = Convert.FromBase64String("AQWSEDRF");
            var byteBuffer = new byte[bytePasswordSalt.Length + bytePassword.Length];
            Buffer.BlockCopy(bytePasswordSalt, 0, byteBuffer, 0, bytePasswordSalt.Length);
            Buffer.BlockCopy(bytePassword, 0, byteBuffer, bytePasswordSalt.Length, bytePassword.Length);
            var byteEncryptSha1 = new SHA1CryptoServiceProvider().ComputeHash(byteBuffer);
            return Convert.ToBase64String(byteEncryptSha1);
        }

        public static string GenerateSalt() {
            var data = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);
        }

        public static int? CheckNullable(int? value) {
            return (value > 0) ? value : null;
        }

        public static decimal? CheckNullable(decimal? value) {
            return (value != 0) ? value : null;
        }

        public static Guid? CheckNullable(Guid? value) {
            return (value != Guid.Empty) ? value : null;
        }

        public static string CheckNullable(string value, int len = 0) {
            if (string.IsNullOrEmpty(value)) {
                return null;
            }

            value = value.TrimEnd();

            if (len == 0) {
                len = value.Length;
            }

            return value.Length > len ? value.Substring(0, len) : value;
        }

        public static string FormatTitle(string title) {
            // tolower
            title = title.ToLower().Trim();

            // remove html tags
            title = Regex.Replace(title, "<.*?>", "-");

            // remove non html characters
            title = title.Replace(" ", "-").Replace("&", string.Empty).Replace("?", string.Empty).Replace(":", string.Empty).Replace("/", string.Empty).Replace("%", string.Empty);

            // remove duplicate "--", "-"
            title = title.Replace("--", "-");

            // remove turkish characters
            title = title.Replace("ı", "i").Replace("ş", "s").Replace("ğ", "g").Replace("ü", "u").Replace("ç", "c").Replace("ö", "o");

            // return 
            return title;
        }

        public static string GetCurrencyString(string number, char decimalSeperator, string[] currencyCodes) {
            if (Convert.ToDecimal(number) == 0) {
                return string.Format("Sıfır {0} Sıfır {1}", currencyCodes[0], currencyCodes[1]);
            }

            var arrPostNames = new string[7];
            arrPostNames[0] = string.Empty;
            arrPostNames[1] = "Bin";
            arrPostNames[2] = "Milyon";
            arrPostNames[3] = "Milyar";
            arrPostNames[4] = "Trilyon";
            arrPostNames[5] = "Trilyar";
            arrPostNames[6] = "Katrilyon";

            number = number.Replace(decimalSeperator, '&');
            var arrBulkChars = new[] { ',', '.', ' ' };
            for (byte i = 0; i < arrBulkChars.Length; i++) {
                number = number.Replace(arrBulkChars[i].ToString(CultureInfo.InvariantCulture), string.Empty);
            }

            var values = number.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            var arrResult = new string[values.Length];
            for (byte j = 0; j < values.Length; j++) {
                number = values[j];
                number = Convert.ToInt64(number, CultureInfo.CurrentCulture).ToString(CultureInfo.CurrentCulture);

                if (number.Length % 3 != 0) {
                    number = (number.Length % 3 == 1 ? "00" : "0") + number;
                }

                for (byte i = 0; i < number.Length / 3; i++) {
                    var tmp = GetThreeDigits(number.Substring(number.Length - ((i + 1) * 3), 3));
                    arrResult[j] = (tmp == "Bir" && i < 3 ? (!string.IsNullOrEmpty(tmp) ? arrPostNames[i] : string.Empty) : tmp + (!string.IsNullOrEmpty(tmp) ? arrPostNames[i] : string.Empty)) + arrResult[j];
                }

                arrResult[j] += " " + currencyCodes[j] + " ";
            }

            var result = string.Empty;
            for (byte i = 0; i < arrResult.Length; i++) {
                result += arrResult[i];
            }

            return result;
        }

        public static string TrimEndChar(string value) {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Substring(0, value.Length - 1);
        }

        public static string GetSeoTitle(string title) {
            if (string.IsNullOrEmpty(title)) {
                return "tanimsiz";
            }

            // tolower
            title = title.ToLower().Trim();

            // replace turkish characters
            title = title.Replace("ı", "i").Replace("ş", "s").Replace("ğ", "g").Replace("ü", "u").Replace("ç", "c").Replace("ö", "o");

            // remove accent character
            string normalized = title.Normalize(NormalizationForm.FormKD);
            var removal = Encoding.GetEncoding(Encoding.ASCII.CodePage, new EncoderReplacementFallback(string.Empty), new DecoderReplacementFallback(string.Empty));
            byte[] bytes = removal.GetBytes(normalized);
            title = Encoding.ASCII.GetString(bytes);

            // remove html tags
            title = Regex.Replace(title, "<.*?>", string.Empty);

            // remove non html characters
            title = title
                .Replace(" ", "-")
                .Replace(",", "-")
                .Replace("?", string.Empty)
                .Replace(":", string.Empty)
                .Replace("/", string.Empty)
                .Replace("%", string.Empty)
                .Replace(".", "-")
                .Replace("+", "-")
                .Replace("\\", "-")
                .Replace("&", string.Empty)
                .Replace("|", string.Empty)
                .Replace("'", string.Empty)
                .Replace(char.ConvertFromUtf32(34), string.Empty)
                .Replace("#", string.Empty)
                .Replace("--", "-")
                .Replace("*", string.Empty)
                .Replace("<", string.Empty)
                .Replace(">", string.Empty)
                .Replace("”", string.Empty)
                .Replace("“", string.Empty)
                .Replace("â", string.Empty)
                .Replace("ó", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty);

            return title;
        }

        public static string GetDetail(object value, int len) {
            if (value == null) {
                return string.Empty;
            }

            var retVal = Regex.Replace(value.ToString(), "<.*?>", string.Empty);
            if (retVal.Length > len) {
                retVal = retVal.Substring(0, len) + "...";
            }

            return retVal;
        }

        public static string Encrypt(string queryString, bool isCookie = false) {
            try {
                const string Key = "!#s$64?1";
                byte[] yKey = Encoding.UTF8.GetBytes(Key);
                byte[] yIv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                var des = new DESCryptoServiceProvider();

                byte[] yInput = Encoding.UTF8.GetBytes(queryString);
                var smsStream = new MemoryStream();
                var scsStream = new CryptoStream(smsStream, des.CreateEncryptor(yKey, yIv), CryptoStreamMode.Write);
                scsStream.Write(yInput, 0, yInput.Length);
                scsStream.FlushFinalBlock();

                string encode = Convert.ToBase64String(smsStream.ToArray());

                if (!isCookie) {
                    //// Added for UrlRewring / problem, may cause problem
                    encode = encode.Replace("/", "%2e");  // convert "."
                    encode = encode.Replace("+", "%2D");  // convert "-"
                    encode = encode.Replace(" ", "%7C");  // convert "|"
                }

                return encode;
            }
            catch (Exception) {
                return string.Empty;
            }
        }

        public static string Encrypt(IEnumerable<object> objValues) {
            string parameters = objValues.Aggregate(string.Empty, (current, objValue) => current + (objValue + ";"));

            if (!string.IsNullOrEmpty(parameters)) {
                parameters += "200911191200000"; ////DateTime.Now.ToString("yyyyMMddHHmmssmmm"); //Add TimeStamp
                return Encrypt(parameters);
            }

            return string.Empty;
        }

        public static string Decrypt(string queryString, bool isCookie = false) {
            try {
                const string Key = "!#s$64?1";
                byte[] yKey = Encoding.UTF8.GetBytes(Key);
                byte[] yIv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                var des = new DESCryptoServiceProvider();

                string temp = queryString;

                if (!isCookie) {
                    temp = temp.Replace(" ", "+");  //// ????

                    ////Added for UrlRewring / problem, may cause problem
                    temp = temp.Replace("-", "+");
                    temp = temp.Replace("%2D", "+");

                    temp = temp.Replace(".", "/");
                    temp = temp.Replace("%2e", "/");

                    temp = temp.Replace("|", " ");
                    temp = temp.Replace("%7C", " ");
                }

                byte[] input = Convert.FromBase64String(temp);

                var memStream = new MemoryStream();
                var cryptoStream = new CryptoStream(memStream, des.CreateDecryptor(yKey, yIv), CryptoStreamMode.Write);
                cryptoStream.Write(input, 0, input.Length);
                cryptoStream.FlushFinalBlock();

                Encoding enc = Encoding.UTF8;
                return enc.GetString(memStream.ToArray());
            }
            catch {
                return string.Empty;
            }
        }

        public static string GetPrice(decimal price) {
            return price.ToString("N2", CultureInfo.GetCultureInfo("tr-TR"));
        }

        private static string GetThreeDigits(string number) {
            var arrDigits = new string[10];
            var arrPowersOfTen = new string[10];
            var result = string.Empty;

            arrDigits[0] = string.Empty;
            arrDigits[1] = "Bir";
            arrDigits[2] = "İki";
            arrDigits[3] = "Üç";
            arrDigits[4] = "Dört";
            arrDigits[5] = "Beş";
            arrDigits[6] = "Altı";
            arrDigits[7] = "Yedi";
            arrDigits[8] = "Sekiz";
            arrDigits[9] = "Dokuz";

            arrPowersOfTen[0] = string.Empty;
            arrPowersOfTen[1] = "On";
            arrPowersOfTen[2] = "Yirmi";
            arrPowersOfTen[3] = "Otuz";
            arrPowersOfTen[4] = "Kırk";
            arrPowersOfTen[5] = "Elli";
            arrPowersOfTen[6] = "Altmış";
            arrPowersOfTen[7] = "Yetmiş";
            arrPowersOfTen[8] = "Seksen";
            arrPowersOfTen[9] = "Doksan";

            result += arrPowersOfTen[Convert.ToInt16(number.Substring(1, 1), CultureInfo.CurrentCulture)];
            result += arrDigits[Convert.ToInt16(number.Substring(number.Length - 1), CultureInfo.CurrentCulture)];
            if (number.Substring(0, 1) == "0") {
                return result;
            }

            if (number.Substring(0, 1) == "1") {
                result = "Yüz" + result;
            }
            else {
                result = arrDigits[Convert.ToInt16(number.Substring(0, 1), CultureInfo.CurrentCulture)] + "Yüz" + result;
            }

            return result;
        }
    }
}
