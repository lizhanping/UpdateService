using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace UpdateService
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //启动判断参数是否合法，不合法直接退出
            if (e.Args == null || e.Args.Length != 1)
            {
                //参数不合法，直接退出
                App.Current.Shutdown();
                Environment.Exit(0);
            }
            //解析参数是否合法
            string[] args = e.Args[0].Split('*');
            if (args.Length != 3)
            {
                //参数不合法，直接退出
                App.Current.Shutdown();
                Environment.Exit(0);
            }
            GlobalInfo.url = args[0];
            GlobalInfo.file = args[1];
            GlobalInfo.exefile = args[2];
            var folder = Path.GetTempPath() + "UpdateService_x_y_z";
            if(!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            GlobalInfo.downloadFolder = folder;
            GlobalInfo.localFolder = Path.GetDirectoryName(GlobalInfo.exefile);
            GlobalInfo.localUpdateFile = Path.Combine(GlobalInfo.localFolder, GlobalInfo.file);

            //先检查需不需要升级，再确定是否要弹窗
            var rst = Util.CheckUpdate();
            if (rst == 0)
            {
                //不需要更新，退出
                Environment.Exit(0);
            }
            else if(rst==-1)
            {
                //下载文件失败了
                Environment.Exit(-1);
            }
            else
            {
                base.OnStartup(e);
            }
        }
    }
}
