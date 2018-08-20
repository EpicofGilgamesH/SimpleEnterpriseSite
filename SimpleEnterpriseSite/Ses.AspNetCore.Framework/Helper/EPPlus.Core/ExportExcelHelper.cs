using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ses.AspNetCore.Framework.Helper.EPPlus.Core
{
    public class ExportExcelHelper<T>
    {
        public ExportExcelHelper(string rootPath)
        {
            _rootPaht = rootPath;
            GetColumPropetrys();
        }

        private const string _exportDirectory = "Excel\\ExportExcel";
        //private const string _importDirectory = "Excel\\ImportExcel";
        /// <summary>
        /// web项目根目录
        /// </summary>
        private string _rootPaht;
        /// <summary>
        /// [列名,列宽] 二维数组
        /// </summary>
        private string[,] _columPropetrys;
        /// <summary>
        /// [列名，列属性信息] 字典类
        /// </summary>
        private Dictionary<string, PropertyInfo> _propertyInfoDictionary;

        /// <summary>
        /// 文件名称
        /// </summary>
        public string TimeName
        {
            get { return DateTime.Now.ToString("yyyyMMddHHmmssfff"); }
        }

        #region private
        /// <summary>
        /// 设置sheet的标题列内容
        /// </summary>
        /// <param name="columnProproty">标题-列宽 二维数组</param>
        /// <param name="worksheet"> sheet</param>
        private void SetSheetColumn(string[,] columnProproty, ExcelWorksheet worksheet)
        {
            // 因为是二维数组，所以行数即为 总长度/2 [ (columnProproty.GetUpperBound(0) + 1) + 1 ]
            for (int i = 1; i < columnProproty.Length / 2 + 1; i++)
            {
                worksheet.Cells[1, i].Value = columnProproty[i - 1, 0];
                worksheet.Cells[1, i].Style.Font.Bold = true;
                worksheet.Column(i).Width = Convert.ToDouble(columnProproty[i - 1, 1]);
                var type = _propertyInfoDictionary[columnProproty[i - 1, 0]].PropertyType;
                if (type == typeof(DateTime) ||
                    type == typeof(DateTime?))
                    worksheet.Column(i).Style.Numberformat.Format = "YYYY/M/D H:MM";
            }
        }

        /// <summary>
        /// 设置sheet正文内容
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="columnProproty">标题-列宽 二维数组</param>
        /// <param name="list">导出数据</param>
        /// <param name="worksheet">sheet</param>
        private void SetSheetBody(string[,] columnProproty, IList<T> list, ExcelWorksheet worksheet)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < columnProproty.Length / 2; j++)
                {
                    //每一条list,都是获取相同结构的属性的值,将反射提前处理
                    object value = null;
                    //var property = typeof(T).GetProperty(columnProproty[j - 1, 0], BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                    //T类型属性不为null
                    var pTypeInfo = _propertyInfoDictionary[columnProproty[j, 0]];
                    if (pTypeInfo != null)
                    {
                        value = pTypeInfo.GetValue(list[i]);
                        if (value != null)
                        {
                            var pType = pTypeInfo.PropertyType;
                            //if (value is DateTime time)
                            //    worksheet.Cells[2 + i, j + 1].Value = time.ToString("yyyyMMddHHmm");
                            //else if (pType == typeof(bool))
                            //{
                            //    var flag = Convert.ToBoolean(value) ? "是" : "否";
                            //    worksheet.Cells[2 + i, j + 1].Value = flag;
                            //}
                            //else
                            //{
                            //    worksheet.Cells[2 + i, j + 1].Value = value;
                            //}
                            if (pType == typeof(bool))
                            {
                                var flag = Convert.ToBoolean(value) ? "是" : "否";
                                worksheet.Cells[2 + i, j + 1].Value = flag;
                            }
                            else
                            {
                                worksheet.Cells[2 + i, j + 1].Value = value;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        ///  初始化 [名称,宽度] 二维数组，初始化 [列名，列属性信息] 字典类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void GetColumPropetrys()
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            //封装一个属性字典类,通过属性名去查找属性的PropertyType
            var cps = new string[properties.Length, 2];
            var ppi = new Dictionary<string, PropertyInfo>();
            for (int i = 0; i < properties.Length; i++)
            {
                cps[i, 0] = properties[i].Name;
                cps[i, 1] = GetColumnWidth(properties[i].Name);
                ppi.Add(properties[i].Name, properties[i]);
            }
            _columPropetrys = cps;
            _propertyInfoDictionary = ppi;
        }

        /// <summary>
        /// 获取列宽
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string GetColumnWidth(string columnName)
        {
            string width = "25";
            var resource = columnName.ToLower();
            if (resource.Contains("time") ||
                resource.Contains("id") ||
                resource.Contains("name") ||
                resource.Contains("url"))
                width = "50";
            return width;
        }

        /// 导出excel
        /// </summary>
        /// <typeparam name="T">导出数据类型</typeparam>
        /// <param name="name">导出文件名</param>
        /// <param name="list">导出的数据源</param>
        /// <param name="filePath">文档地址全路径</param>
        public bool Export(string name, IList<T> list, out string filePath, out string excelName)
        {
            return Export(name, _columPropetrys, list, out filePath, out excelName);
        }

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <typeparam name="T">导出数据类型</typeparam>
        /// <param name="name">导出文件名</param>
        /// <param name="columnProproty">[列名,列宽]</param>
        /// <param name="list">导出的数据源</param>
        public bool Export(string name, string[,] columnProproty, IList<T> list, out string filePath, out string excelName)
        {
            try
            {
                // fileName string变量被赋值后，引用路径中即存在该值
                var fileName = $"{name}_{TimeName}.xlsx";
                // 文件夹路径
                var fileDirectory = Path.Combine(_rootPaht, _exportDirectory);
                // 文件全路径
                FileInfo fileinfo = new FileInfo(Path.Combine(fileDirectory, fileName));

                // 不存在该路径时
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }
                // 文件夹中存在相同文件时
                if (fileinfo.Exists)
                {
                    fileinfo.Delete();
                    fileinfo = new FileInfo(Path.Combine(fileDirectory, fileName));
                }

                using (ExcelPackage packge = new ExcelPackage(fileinfo))
                {
                    // 创建sheet名
                    ExcelWorksheet worksheet = packge.Workbook.Worksheets.Add(name);
                    // 写入sheet的标题列
                    SetSheetColumn(columnProproty, worksheet);
                    // 写入sheet正文
                    SetSheetBody(columnProproty, list, worksheet);
                    packge.Save();
                }
                filePath = $"{_exportDirectory}\\{fileName}";
                excelName = fileName;
                return true;
            }
            catch (Exception ex)
            {
                filePath = ex.Message;
                excelName = string.Empty;
                return false;
            }
        }

    }
}
