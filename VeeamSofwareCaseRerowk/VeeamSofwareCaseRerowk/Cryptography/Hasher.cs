using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VeeamSoftwareFirstCase.Cryptography
{
    class Hasher : IDisposable
    {
        private readonly SHA256 _sha256;

        public Hasher()
        {
            _sha256 = new SHA256CryptoServiceProvider();
        }

        public byte[] CreateHash(byte[] input)
        {
            return _sha256.ComputeHash(input);
        }

        public static string ToString(byte[] hash)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var @byte in hash)
            {
                builder.Append(@byte.ToString("x2"));
            }
            return builder.ToString();
        }

        public void Dispose()
        {
            _sha256.Dispose();
        }
    }
}