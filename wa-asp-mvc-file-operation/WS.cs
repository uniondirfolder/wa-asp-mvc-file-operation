using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using wa_asp_mvc_file_operation.Data.Models;

namespace wa_asp_mvc_file_operation
{
    public static class WS
    {
        public static bool IsGoodString(string value) => (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value));
        public static List<ServerFile> GetListServerFilesUser(HttpContextBase context) 
        {
            if (context != null && context.Session.Count>0) 
            {
                string basePathUser = context.Session["currPath"].ToString();
                if (IsGoodString(basePathUser)) 
                {
                    try
                    {
                        List<ServerFile> list = new List<ServerFile>();

                        foreach (string directory in Directory.GetDirectories(basePathUser))
                        {
                            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                            list.Add(new ServerFile
                            {
                                Name = directoryInfo.Name,
                                FileType = FileType.Folder,
                                Path = directory.Replace(context.Session["rootPath"].ToString(), "")
                            });
                        }

                        foreach (string file in Directory.GetFiles(basePathUser))
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            list.Add(new ServerFile
                            {
                                Name = fileInfo.Name,
                                FileType = FileType.File,
                                Path = file.Replace(context.Session["rootPath"].ToString(), "")
                            });
                        }

                        return list;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        
                    }
                }
            }
            return null;
        }
        public static bool CreateFolder(string folderName, HttpContextBase context) 
        {
            if (!IsGoodString(folderName) && context!=null && context.Session.Count > 0) 
            {
                string path = Path.Combine(context.Session["currPath"].ToString(), folderName);

                try
                {
                    Directory.CreateDirectory(path);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            return false;
        }
        public static bool UploadFiles(HttpContextBase context)
        {
            if (context!=null && context.Session.Count > 0)
            {
                string basePath = context.Session["currPath"].ToString();
                if (IsGoodString(basePath) && Directory.Exists(basePath))
                {
                    try
                    {
                        for (int i = 0; i < context.Request.Files.Count; i++)
                        {
                            HttpPostedFileBase file = context.Request.Files[i];
                            if (file != null)
                            {
                                try
                                {
                                    file.SaveAs(Path.Combine(basePath, file.FileName));
                                }
                                catch (Exception ex)
                                {

                                    throw new Exception(ex.Message + $"File name {file} & index in array{i}");
                                }
                            }
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }

            return false;
        }
        public static bool RemoveFile(string fileName, HttpContextBase context) 
        {
            if(IsGoodString(fileName) && context != null && context.Session.Count > 0) 
            {
                try
                {
                    fileName = Encoding.Unicode.GetString(HttpServerUtility.UrlTokenDecode(fileName));

                    string basePath = context.Session["rootPath"].ToString();
                    string filePath = basePath + fileName;

                    if (IsGoodString(filePath) && File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return false;
        }
        public static bool RemoveFolder(string folderName, HttpContextBase context)
        {
            if(IsGoodString(folderName)&& context!= null && context.Session.Count > 0) 
            {
                var basePath = context.Session["rootPath"].ToString();
                var folderPath = basePath + Encoding.Unicode.GetString(HttpServerUtility.UrlTokenDecode(folderName));

                if (Directory.Exists(folderPath)) 
                {
                    try
                    {
                        if (ClearFolder(folderPath)) 
                        {
                            Directory.Delete(folderPath);
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {

                        Debug.WriteLine(ex);
                    }
                }
            }

            return false;
        }
        public static bool ClearFolder(string path)
        {
            try
            {
                foreach (string directory in Directory.GetDirectories(path))
                {
                    ClearFolder(directory);
                    Directory.Delete(directory);
                }

                foreach (string file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }
        public static bool Rename(string oldName, string fileType, string newName, HttpContextBase contex) 
        {
            if(IsGoodString(oldName) && IsGoodString(newName) && IsGoodString(fileType) && contex != null && contex.Session.Count > 0) 
            {
                Enum.TryParse(fileType, out FileType type);

                string basePath = contex.Session["rootPath"].ToString();
                oldName = Encoding.Unicode.GetString(HttpServerUtility.UrlTokenDecode(oldName));
                
                switch (type)
                {
                    case FileType.File:
                        string oldFilePath = basePath + oldName;

                        if (IsGoodString(Path.GetExtension(newName)))
                        {
                            newName += Path.GetExtension(oldName);
                        }

                        if (File.Exists(oldFilePath))
                        {
                            try
                            {
                                File.Move(oldFilePath, basePath + oldName.Replace(Path.GetFileName(oldName), "") + newName);
                                return true;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }
                        }
                        break;
                    case FileType.Folder:
                        string olfFolderPath = basePath + oldName;

                        if (Directory.Exists(olfFolderPath))
                        {
                            try
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(olfFolderPath);
                                string path = directoryInfo.Parent?.FullName.Replace(basePath, "");
                                Directory.Move(olfFolderPath, Path.Combine(basePath + path, newName));
                                return true;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return false;
        }
        public static void FindFile(string path, List<ServerFile> files, string fileName, HttpContextBase contex)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                FindFile(directory, files, fileName, contex);

                DirectoryInfo directoryInfo = new DirectoryInfo(directory);

                if (directoryInfo.Name.ToLower().Contains(fileName))
                {
                    files.Add(new ServerFile
                    {
                        Name = directoryInfo.Name,
                        FileType = FileType.Folder,
                        Path = directoryInfo.FullName.Replace(contex.Session["rootPath"].ToString(), "")
                    });
                }
            }

            foreach (string file in Directory.GetFiles(path))
            {
                FileInfo fileInfo = new FileInfo(file);

                if (fileInfo.Name.ToLower().Contains(fileName))
                {
                    files.Add(new ServerFile
                    {
                        Name = fileInfo.Name,
                        FileType = FileType.File,
                        Path = file.Replace(contex.Session["rootPath"].ToString(), "")
                    });
                }
            }
        }
        public static void GetFolderThree(string path, List<string> folderList, HttpContextBase contex)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                folderList.Add(directory.Replace(contex.Session["rootPath"].ToString(), ""));
                GetFolderThree(directory, folderList,contex);
            }
        }
        public static bool MoveFile(string fileName, string fileType, string newPath, HttpContextBase contex)
        {
            if (IsGoodString(fileName) && IsGoodString(fileType) && IsGoodString(newPath) && contex!=null && contex.Session.Count > 0)
            {
                Enum.TryParse(fileType, out FileType type);

                string basePath = contex.Session["rootPath"].ToString();
                fileName = Encoding.Unicode.GetString(HttpServerUtility.UrlTokenDecode(fileName));

                switch (type)
                {
                    case FileType.File:

                        string filePath = basePath + fileName;

                        if (File.Exists(filePath) &&
                            Directory.Exists(newPath.Equals("\\") ? contex.Session["rootPath"].ToString() : basePath + newPath))
                        {
                            try
                            {
                                string newFilePath = newPath.Equals("\\") ? Path.Combine(contex.Session["rootPath"].ToString(), Path.GetFileName(fileName)) : Path.Combine(basePath + newPath, Path.GetFileName(fileName));

                                if (!filePath.Equals(newFilePath))
                                { File.Move(filePath, newFilePath); return true; }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }
                        }
                        break;
                    case FileType.Folder:
                        string oldFolderPath = basePath + fileName;

                        if (Directory.Exists(oldFolderPath) &&
                            Directory.Exists(newPath.Equals("\\") ? contex.Session["rootPath"].ToString() : basePath + newPath))
                        {
                            try
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(oldFolderPath);
                                string newFolderPath = newPath.Equals("\\") ? Path.Combine(contex.Session["rootPath"].ToString(), directoryInfo.Name) : Path.Combine(basePath + newPath, directoryInfo.Name);

                                if (!oldFolderPath.Equals(newFolderPath))
                                { Directory.Move(oldFolderPath, newFolderPath); return true; }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }
                        }
                        break;
                }
            }

            return false;
        }
    }
}