using MiniBanking.Core.Helper.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace MiniBanking.Core.Helper
{
    public static class Utility
    {
        public static ResponseModel Success(this ResponseModel model, string message = "Data saved successfully.", object data = null)
        {
            model.Success = true;
            model.Message = message;
            model.Data = data;

            return model;
        }

        public static ResponseModel Error(this ResponseModel model, string message = null, object data = null)
        {
            model.Success = false;
            model.Message = message;
            model.Data = data;

            return model;
        }

        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static IEnumerable<SelectListItem> EnumsToSelectList<T>(object setdValue = null, string defalutText = "", string defalutValue = "")
        {
            var items = Enum.GetValues(typeof(T)).Cast<T>().Select(v => new SelectListItem
            {
                Text = v.ToDescription(),
                Label = v.ToDescription(),
                Value = Convert.ToInt32(v).ToString()
            }).ToList();

            if (defalutText.IsNotNullOrEmpty())
            {
                items.Insert(0, new SelectListItem { Text = defalutText, Value = defalutValue });
            }

            if (setdValue != null)
            {
                items.SetSelected(setdValue);
            }

            return items;
        }

        public static string ToDescription<T>(this T status)
        {
            Type enumType = typeof(T);

            MemberInfo memberInfo =
                enumType.GetMember(status.ToString()).First();
            var descriptionAttribute =
                memberInfo.GetCustomAttribute<DescriptionAttribute>();

            var description = descriptionAttribute.Description;

            return description;
        }

        public static List<SelectListItem> SetSelected(this List<SelectListItem> items, object value)
        {
            if (items != null && value != null)
            {
                var v = value.ToString();
                if (items.Any())
                {
                    foreach (var s in items.Where(o => o.Selected == true))
                    {
                        s.Selected = false;
                    }

                    var item = items.Where(o => o.Value == v).FirstOrDefault();
                    if (item != null)
                    {
                        item.Selected = true;
                    }
                }
            }
            return items;
        }


        public static string GetConnectionString(this AppClaim appClaim)
        {
            return $"Server={Constants.SqlServer};Initial Catalog={appClaim.DBName};Persist Security Info=False;User ID={Encription.Decrypt(appClaim.DBUserId)};Password={Encription.Decrypt(appClaim.DBUserPassword)};MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }

        public static string GetSpecificClaim(this ClaimsIdentity claimsIdentity, string claimType)
        {
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == claimType);

            return (claim != null) ? claim.Value : string.Empty;
        }

        public static AppClaim ParseClaim(this System.Security.Claims.ClaimsIdentity claimsIdentity)
        {
            var appClaim = new AppClaim
            {
                UserId = claimsIdentity.GetSpecificClaim("UserId"),
                UserTypeId = Convert.ToString(claimsIdentity.GetSpecificClaim("UserTypeId")),
                CustomerId = claimsIdentity.GetSpecificClaim("CustomerId"),
                AccountId = Convert.ToString(claimsIdentity.GetSpecificClaim("AccountId")),
                PersonalCode = claimsIdentity.GetSpecificClaim("PersonalCode"),
                Email = Convert.ToString(claimsIdentity.GetSpecificClaim("Email"))
            };
            
            //appClaim.SubscriptionId = claimsIdentity.GetSpecificClaim("SubscriptionId");
            //if (appClaim.SubscriptionId.IsNotNullOrEmpty())
            //{
            //    appClaim.DBName = claimsIdentity.GetSpecificClaim("DBName");
            //    appClaim.DBUserId = claimsIdentity.GetSpecificClaim("DBUserId");
            //    appClaim.DBUserPassword = claimsIdentity.GetSpecificClaim("DBUserPassword");
            //}

            return appClaim;
        }

        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 12,
                RequiredUniqueChars = 6,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
                    "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                    "abcdefghijkmnopqrstuvwxyz",    // lowercase
                    "0123456789",                   // digits
                    "!@$?_-"                        // non-alphanumeric
                };
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        public static string Encrypt(this string text)
        {
            return Encription.Encrypt(text);
        }

        public static string Decrypt(this string text)
        {
            return Encription.Decrypt(text);
        }

        public static string NoTilde(this string text)
        {
            return text.Replace("~", "");
        }

        public static string FirstCharToUpper(this string s)
        {
            var str = string.Empty;
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            str = char.ToUpper(s[0]) + s.Substring(1);
            return str;
        }

    }
}
