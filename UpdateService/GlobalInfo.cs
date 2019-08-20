/*
*---------------------------------
*|		All rights reserved.
*|		author: lizhanping
*|		version:1.0
*|		File: GlobalInfo.cs
*|		Summary: 
*|		Date: 2019/8/15 18:14:18
*---------------------------------
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace UpdateService
{
    public class GlobalInfo
    {
        /// <summary>
        /// 可执行文件
        /// </summary>
        public static string exefile { get; set; } 
        /// <summary>
        /// 升级URL
        /// </summary>
        public static string url { get; set; }
        /// <summary>
        /// 升级文件
        /// </summary>
        public static string file { get; set; }     
        
        ///下载文件存放文件夹         
        public static string downloadFolder { get; set; }  
        /// <summary>
        /// 本地文件夹
        /// </summary>
        public static string localFolder { get; set; }   
        /// <summary>
        /// 远程升级文件
        /// </summary>
        public static string remoteUpdateFile { get; set; }   
        /// <summary>
        /// 本地升级文件
        /// </summary>
        public static string localUpdateFile { get; set; }
    }
}
