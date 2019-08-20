/*
*---------------------------------
*|		All rights reserved.
*|		author: lizhanping
*|		version:1.0
*|		File: Util.cs
*|		Summary: 
*|		Date: 2019/8/19 14:03:31
*---------------------------------
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace UpdateService
{
    public class Util
    {
        public static ImageSource ConverterFromBitmap(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap =
                 Imaging.CreateBitmapSourceFromHBitmap(
                      hBitmap, IntPtr.Zero, Int32Rect.Empty,
                      BitmapSizeOptions.FromEmptyOptions());
            return wpfBitmap;
        }

        public static int DownloadFile(string url, string localfile)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(url, localfile);
                }
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 检查是否需要更新
        /// </summary>
        /// <returns></returns>
        public static int CheckUpdate()
        {
            //根据URL地址及File，拉取到临时文件
            //然后跟File比较，看看Version是否有差别，
            //无差别则不更新，返回0：有则返回1
            string tempfile = GlobalInfo.downloadFolder + "\\" + GlobalInfo.file;
            string url = GlobalInfo.url + GlobalInfo.file;
            string localFile = GlobalInfo.localUpdateFile;
            try
            {
                XmlDocument remoteDoc = new XmlDocument();
                remoteDoc.Load(url);
                XmlNode verNode = remoteDoc.SelectSingleNode(@"AutoUpdater/Application/Version");
                string version = verNode.InnerText.ToString();
                //加载本地的version
                XmlDocument localDoc = new XmlDocument();
                localDoc.Load(localFile);
                verNode = localDoc.SelectSingleNode(@"AutoUpdater/Application/Version");
                if (version.Equals(verNode.InnerText))
                {
                    //说明版本相等
                    return 0;
                }
                else
                {
                    //版本不一致，现在到本地
                    remoteDoc.Save(tempfile);
                    GlobalInfo.remoteUpdateFile = tempfile;
                    return 1;
                }
            }
            catch
            {
                return -1;
            }
        }
    }

}
