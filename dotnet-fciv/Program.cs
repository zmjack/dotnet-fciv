using Ink;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NStandard;
using NStandard.Flows;
using System.Security.Cryptography;

namespace dotnet_fciv
{
    public class Program
    {
        private static readonly Lazy<MD5> _md5 = new(() => MD5.Create());
        private static readonly Lazy<SHA1> _sha1 = new(() => SHA1.Create());
        private static readonly Lazy<SHA256> _sha256 = new(() => SHA256.Create());
        private static readonly Lazy<SHA384> _sha384 = new(() => SHA384.Create());
        private static readonly Lazy<SHA512> _sha512 = new(() => SHA512.Create());

        public static void Main(string[] args)
        {
            if (!args.Any() || args.Contains("/?") || args.Contains("-?"))
            {
                PrintUsage();
                return;
            }

            var isDirectory = Directory.Exists(args[0]);
            if (isDirectory)
            {
                var dir = args[0];
                var pattern = args[1];

                var options = args.Skip(2).ToArray();
                var recursive = options.Contains("-r");
                var hashTypes = GetHashTypes(options);

                if (recursive)
                    PrintResult(dir, Directory.GetFiles(dir, pattern, SearchOption.AllDirectories), hashTypes);
                else PrintResult(dir, Directory.GetFiles(dir, pattern), hashTypes);
            }
            else
            {
                var file = args[0];
                var options = args.Skip(1).ToArray();
                var hashTypes = GetHashTypes(options);

                PrintResult(Path.GetDirectoryName(file) ?? string.Empty, new[] { file }, hashTypes);
            }
        }

        private static HashTypes GetHashTypes(string[] args)
        {
            HashTypes hashTypes = HashTypes.None;
            if (args.Contains("-md5")) hashTypes |= HashTypes.MD5;
            if (args.Contains("-sha1")) hashTypes |= HashTypes.SHA1;
            if (args.Contains("-sha256")) hashTypes |= HashTypes.SHA256;
            if (args.Contains("-sha384")) hashTypes |= HashTypes.SHA384;
            if (args.Contains("-sha512")) hashTypes |= HashTypes.SHA512;
            return hashTypes;
        }

        private static void PrintUsage()
        {
            Echo.Line()
                .Line("Usage: dotnet fciv FileOrDirectory [FilePatten] [Options]")
                .Line()
                .NoBorderTable(new[]
                {
                    new { Options = "-r", Description = "Includes the current directory and all its subdirectories in a search operation." },
                    new { Options = "-md5", Description = "Compute the file using the MD5 hash algorithm." },
                    new { Options = "-sha1", Description = "Compute the file using the SHA1 hash algorithm." },
                    new { Options = "-sha256", Description = "Compute the file using the SHA256 hash algorithm." },
                    new { Options = "-sha384", Description = "Compute the file using the SHA384 hash algorithm." },
                    new { Options = "-sha512", Description = "Compute the file using the SHA512 hash algorithm." },
                })
                .Line();
        }

        private static void PrintResult(string dir, IEnumerable<string> files, HashTypes hashTypes)
        {
            var flags = hashTypes.GetFlags();
            var headers = new string[flags.Length + 1];
            headers[0] = "File";
            foreach (var (index, flag) in flags.AsIndexValuePairs())
            {
                headers[index + 1] = flag.ToString();
            }
            var colLength = flags.Length + 1;

            var hashAlgorithms = new HashAlgorithm[flags.Length];
            foreach (var (index, flag) in flags.AsIndexValuePairs())
            {
                hashAlgorithms[index] = flag switch
                {
                    HashTypes.MD5 => _md5.Value,
                    HashTypes.SHA1 => _sha1.Value,
                    HashTypes.SHA256 => _sha256.Value,
                    HashTypes.SHA384 => _sha384.Value,
                    HashTypes.SHA512 => _sha512.Value,
                    _ => throw new NotImplementedException(),
                };
            }

            var list = new List<string[]>();
            foreach (var file in files)
            {
                var line = new string[colLength];
                var fileShortName = file.Substring(dir.Length + 1);

                line[0] = File.Exists(file) ? fileShortName : $"NotFound: {file}";

                foreach (var (index, flag) in flags.AsIndexValuePairs())
                {
                    using var stream = new FileStream(file, FileMode.Open);
                    line[index + 1] = BytesFlow.HexString(hashAlgorithms[index].ComputeHash(stream));
                }

                list.Add(line);
            }

            Echo.Line()
                .NoBorderTable(headers, list.ToArray())
                .Line();
        }

    }
}