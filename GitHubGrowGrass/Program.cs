using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubGrowGrass
{
    class Program
    {
        /// <summary>
        /// READMEファイル名
        /// </summary>
        const string README_FILE = @"README.md";

        static void Main(string[] args)
        {
            // リターンコード
            var returnCd = 0;

            if (args.Length != 1)
            {
                //引数が1つでない場合はエラー
                returnCd = 1;
                System.Environment.Exit(returnCd);
            }

            //引数からディレクトリパスを取得
            var dir = args[0];
            if (!Directory.Exists(dir))
            {
                //存在しないディレクトリの場合はエラー
                returnCd = 1;
                System.Environment.Exit(returnCd);
            }

            var filePath = Path.Combine(dir, README_FILE);
            if (!File.Exists(filePath))
            {
                //ファイルが存在しない場合はエラー
                returnCd = 1;
                System.Environment.Exit(returnCd);
            }

            //Git Pull
            GitCommand(@"pull", dir);

            //README編集
            {
                var data = File.ReadAllText(filePath);
                if (data.Substring(data.Length - 2, 2) == "\r\n")
                {
                    //文末が改行（CRLF）であれば削除
                    data = data.Remove(data.Length - 2, 2);
                }
                data += @"+";
                File.WriteAllText(filePath, data);
            }

            //Git Commit
            GitCommand(@"commit -a -m ""GitHubGrowGrass""", dir);
            //Git Push
            GitCommand(@"push", dir);

            System.Environment.Exit(returnCd);
        }

        static int ExecuteCommand(string command, string workingDirectory, string arguments = "")
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo(command)
                {
                    Arguments = arguments,
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectory,
                }
            };
            process.Start();
            process.WaitForExit();
            return process.ExitCode;
        }

        static void GitCommand(string arguments, string workingDirectory)
        {
            if (ExecuteCommand("git", workingDirectory, arguments) != 0)
            {
                throw new Exception("git command failed.");
            }
        }

    }
}
