/*
*---------------------------------
*|		All rights reserved.
*|		author: lizhanping
*|		version:1.0
*|		File: MainViewModel.cs
*|		Summary: 
*|		Date: 2019/8/19 13:42:14
*---------------------------------
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml;

namespace UpdateService
{
    public class MainViewModel:ViewModelBase
    {

        private string tempFolder = string.Empty;
        private string updateFile = string.Empty;

        public MainViewModel()
        {
            Icon icon=Icon.ExtractAssociatedIcon(GlobalInfo.exefile);
            IconSource = Util.ConverterFromBitmap(icon.ToBitmap());
            Status = 0;
            Progress = 0;
            Log = ReadLog();
            CurrentVersion = "当前版本："+ReadVersion(GlobalInfo.localUpdateFile);
        }

        #region Property
        /// <summary>
        /// ICON图标
        /// </summary>
        private ImageSource iconsource;
        public ImageSource IconSource
        {
            get
            {
                return iconsource;
            }
            set
            {
                iconsource = value;
                RaisePropertyChanged(nameof(IconSource));
            }
        }

        /// <summary>
        /// 当前版本
        /// </summary>
        private string currentVersion;
        public string CurrentVersion
        {
            get
            {
                return currentVersion;
            }
            set
            {
                currentVersion = value;
                RaisePropertyChanged(nameof(CurrentVersion));
            }
        }

        /// <summary>
        /// 日志
        /// </summary>
        private string log;
        public string Log
        {
            get
            {
                return log;
            }
            set
            {
                log = value;
                RaisePropertyChanged(nameof(Log));
            }
        }

        /// <summary>
        /// 当前进度
        /// </summary>
        private int progress;
        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
                RaisePropertyChanged(nameof(Progress));
            }
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        private int status;
        public int Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                RaisePropertyChanged(nameof(Status));
            }
        }

        /// <summary>
        /// 更新结果
        /// </summary>
        private bool result;
        public bool Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
                RaisePropertyChanged(nameof(Result));
            }
        }
        #endregion

        #region Command
        /// <summary>
        /// Close Command
        /// </summary>
        private RelayCommand closeCommand;
        public RelayCommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                    closeCommand = new RelayCommand(() => {
                        if(status==2)//更新完毕
                        {
                            Environment.Exit(0);
                        }
                        else
                        {
                            Environment.Exit(-1);
                        }
                    });
                return closeCommand;
            }
            set
            {
                closeCommand = value;
                RaisePropertyChanged(nameof(CloseCommand));
            }
        }

        /// <summary>
        /// 更新命令
        /// </summary>
        private RelayCommand updateCommand;
        public RelayCommand UpdateCommand
        {
            get
            {
                if(updateCommand==null)
                {
                    updateCommand = new RelayCommand(() => UpdateOperate());
                    
                }
                return updateCommand;
            }
            set
            {
                updateCommand = value;
                RaisePropertyChanged(nameof(UpdateCommand));
            }
        }

        private void UpdateOperate()
        {
            if(status==0)
            {
                //开始更新,先杀掉当前程序
                string name = Path.GetFileNameWithoutExtension(GlobalInfo.exefile);
                
                Process[] p = Process.GetProcessesByName(name);
                foreach (var item in p)
                {
                    item.Kill();
                }   
                Status = 1;               
                System.Threading.Thread updateThread = new System.Threading.Thread(new System.Threading.ThreadStart(Update));
                updateThread.Start();
                return;
            }
            if(status==1)
            {
                //正在更新的话，直接返回
                return;
            }
            if(status==2)
            {
                //更新完成,如果更新成功，则重启启动
                if(Result)
                {
                    Process p = new Process();
                    p.StartInfo.CreateNoWindow = false;
                    p.StartInfo.FileName = GlobalInfo.exefile;
                    p.Start();
                }
                Environment.Exit(0);
            }
        }

        //更新
        private void Update()
        {
            //比对XML文件中的MD5值，获取需要更新的总文件个数
            var list = GetFileList();
            if(list.Count==0)
            {
                //没有文件更新，但是版本号更新了，可以直接返回，并显示更新完成,但需要移动配置文件
                try
                {
                    File.Copy(GlobalInfo.remoteUpdateFile, GlobalInfo.localUpdateFile, true);
                    Status = 2;
                    Result = true;
                    return;
                }
                catch
                {
                    Status = 2;
                    Result = false;
                    return;
                }

            }
            int step = 95 / list.Count;//留5%用来清除文件
            foreach (var item in list)
            {
                //都下载到本地
                string url = GlobalInfo.url+ item;
                string local = GlobalInfo.downloadFolder + "\\" + item;
                int cnt = 0;//重试次数3
                while(cnt<3)
                {
                    var rst = Util.DownloadFile(url, local);
                    if(rst==1)
                    {
                        cnt = 4;
                        Progress += step;
                    }
                    else
                    {
                        cnt++;
                    }
                }
                if(cnt==3)
                {
                    //说明都没有下载成功，需要提示更新失败
                    Status = 2;
                    Result = false;//失败
                    return;
                }
            }
            //全部下载成功，开始移动文件
            foreach (string file in list)
            {
                string src = Path.Combine(GlobalInfo.downloadFolder,file );
                string dest = Path.Combine(GlobalInfo.localFolder, file);
                try
                {
                    File.Copy(src, dest, true);
                }
                catch
                {
                    //拷贝失败
                    Status = 2;
                    Result = false;
                    return;
                }
            }
            //移动update.xml文件
            try
            {
                File.Copy(GlobalInfo.remoteUpdateFile, GlobalInfo.localUpdateFile, true);
            }
            catch
            {
                //拷贝失败
                Status = 2;
                Result = false;
                return;
            }
            Progress = 100;
            Status = 2;
            Result = true;
            //更新完成后，删除临时文件夹和临时文件
            Clear();
        }

        /// <summary>
        /// 获取需要更新的文件列表
        /// </summary>
        /// <returns></returns>
        private List<string> GetFileList()
        {
            List<string> list = new List<string>();
            XmlDocument remoteDoc = new XmlDocument();
            XmlDocument localDoc = new XmlDocument();
            remoteDoc.Load(GlobalInfo.remoteUpdateFile);
            localDoc.Load(GlobalInfo.localUpdateFile);
            XmlNode rNode = remoteDoc.SelectSingleNode("AutoUpdater/Files");
            XmlNode lNode = localDoc.SelectSingleNode("AutoUpdater/Files");
            var rList = rNode.ChildNodes;
            var lList = lNode.ChildNodes;
            foreach (XmlNode item in rList)
            {
                string name = item.Attributes["Name"].Value.ToString();
                string md5 = item.Attributes["Md5"].Value.ToString();
                bool exist = false;
                foreach(XmlNode node in lList)
                {
                    if(node.Attributes["Name"].Value.Equals(name))
                    {
                        exist = true;                        
                        if(!node.Attributes["Md5"].Value.Equals(md5))
                        {
                            list.Add(name);//存在文件，但是MD5不一致
                        }
                    }
                }
                if(!exist)
                {
                    list.Add(name);
                }
            }
            return list;
        }

        /// <summary>
        /// 清除临时数据
        /// </summary>
        private void Clear()
        {
            //清除操作
            try
            {
                Directory.Delete(GlobalInfo.downloadFolder, true);
            }
            catch
            {

            }
        }
        #endregion

        #region 其他
        /// <summary>
        /// 加载日志
        /// </summary>
        /// <returns></returns>
        private string ReadLog()
        {
            string log = "发现新版本：";
            log += ReadVersion(GlobalInfo.remoteUpdateFile);
            log += "\n";    
            if(File.Exists(GlobalInfo.remoteUpdateFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(GlobalInfo.remoteUpdateFile);
                XmlNode node = doc.SelectSingleNode("AutoUpdater/Application/Log");
                string[] strs = node?.InnerText.Split(new char[] { '；', ';' });
                foreach(var item in strs)
                {
                    log += item;
                    log += "\n";
                }
                return log;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        /// <summary>
        /// 读取Version信息
        /// </summary>
        /// <returns></returns>
        private string ReadVersion(string file)
        {
            string version = string.Empty;
            if (File.Exists(file))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);
                XmlNode node = doc.SelectSingleNode("AutoUpdater/Application/Version");
                version= node?.InnerText;
                return version;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        #endregion
    }
}
