using Newtonsoft.Json;
using OnTimeSpeed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnTimeSpeed.Utils
{
    public static class VariousUtils
    {
        public static string SerializeAndEncodeUser(User user)
        {
            var serialized = JsonConvert.SerializeObject(user);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(serialized);
            var encoded = Convert.ToBase64String(plainTextBytes);

            return encoded;
        }

        public static User DecodeAndDeserializeUser(string userString)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(userString);
            var raw = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            var user = JsonConvert.DeserializeObject<User>(raw);

            return user;
        }

    }
}