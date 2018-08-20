using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Ses.AspNetCore.Entities;
using Ses.AspNetCore.Framework.IService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ses.AspNetCore.Framework.Helper.EPPlus.Core
{
    public class ImportExcelHelper<T> where T : class, IEntityBase<Guid>, new()
    {
        public ImportExcelHelper(string rootPath, IBaseService<T, Guid> baseService)
        {
            _baseService = baseService;
            _rootPaht = rootPath;
            GetColumPropetrys();
        }

        private IBaseService<T, Guid> _baseService;
        /// <summary>
        /// 导入文件路径
        /// </summary>
        private const string _importDirectory = "Excel\\ImportExcel";
        /// <summary>
        /// 导入时错误信息列名
        /// </summary>
        private const string _errorColumnName = "导入行错误信息";
        /// <summary>
        /// 导入时错误信息列宽
        /// </summary>
        private const double _errorColumnWidth = 100;

        /// <summary>
        /// web项目根目录
        /// </summary>
        private string _rootPaht;

        /// <summary>
        /// [列名，列属性信息] 字典类
        /// </summary>
        private Dictionary<string, PropertyInfo> _propertyInfoDictionary = new Dictionary<string, PropertyInfo>();

        /// <summary>
        /// 文件名称
        /// </summary>
        public string TimeName
        {
            get { return DateTime.Now.ToString("yyyyMMddHHmmssfff"); }
        }


        /// <summary>
        ///  初始化 [列名,列属性] 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void GetColumPropetrys()
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            //封装一个属性字典类,通过属性名去查找属性的PropertyType
            for (int i = 0; i < properties.Length; i++)
            {
                _propertyInfoDictionary.Add(properties[i].Name, properties[i]);
            }
        }


        public string Import(IFormFile excel, string name, out string excelName)
        {
            var filename = $"{name}_{TimeName}.xlsx";
            //文件夹路径
            var fileDirectory = Path.Combine(_rootPaht, _importDirectory);
            FileInfo fileInfo = new FileInfo(Path.Combine(fileDirectory, filename));
            // 不存在该路径时
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
            try
            {
                using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Create))
                {
                    excel.CopyTo(fs);
                    fs.Flush();
                }
                using (ExcelPackage packge = new ExcelPackage(fileInfo))
                {
                    //只取第一个sheet
                    ExcelWorksheet worksheet = packge.Workbook.Worksheets[1];
                    //获取列数据序列
                    Dictionary<int, string> columnsIndex = new Dictionary<int, string>();
                    var columnsLength = worksheet.Dimension.Columns;
                    var rowsLength = worksheet.Dimension.Rows;
                    for (int i = 0; i < columnsLength; i++)
                    {
                        columnsIndex.Add(i + 1, worksheet.Cells[1, i + 1].Value.ToString());
                    }

                    //写入错误文档列
                    worksheet.Cells[1, columnsLength + 1].Value = _errorColumnName;
                    worksheet.Cells[1, columnsLength + 1].Style.Font.Bold = true;
                    worksheet.Column(columnsLength + 1).Width = _errorColumnWidth;

                    //除去第一行标题，剩余rowsLength-1行数据
                    for (int i = 0; i < rowsLength - 1; i++)
                    {
                        T t = new T();
                        string message = "success";
                        var flag = true;
                        //每一行进行实体赋值，当其中有一个字段赋值失败，则放弃这一条数据

                        for (int j = 0; j < columnsIndex.Count; j++)
                        {
                            object value = worksheet.Cells[i + 2, j + 1].Value;
                            if (value != null)
                            {
                                var propertyName = columnsIndex[j + 1];
                                var property = _propertyInfoDictionary[propertyName];
                                var type = property.PropertyType;
                                try
                                {
                                    if (type == typeof(bool))
                                    {
                                        value = Convert.ToBoolean(value);
                                    }
                                    else if (type == typeof(DateTime) ||
                                               type == typeof(DateTime?))
                                    {
                                        value = Convert.ToDateTime(value);
                                    }
                                    else if (type == typeof(int))
                                    {
                                        value = Convert.ToInt32(value);
                                    }
                                    else if (type == typeof(double))
                                    {
                                        value = Convert.ToDouble(value);
                                    }
                                    else if (type == typeof(decimal))
                                    {
                                        value = Convert.ToDecimal(value);
                                    }

                                    property.SetValue(t, value);
                                }
                                catch (Exception ex)
                                {
                                    //出现错误，即进行下一行数据的读取
                                    message = "failed,数据赋值出错" + ex.Message;
                                    //写入excel错误文档
                                    worksheet.Cells[i + 2, columnsLength + 1].Value = message;
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        //数据读取成功，将数据存入数据库
                        if (flag)
                        {
                            try
                            {
                                _baseService.Add(t);
                            }
                            catch (Exception ex)
                            {
                                message = "failed--数据存入数据库出错" + ex.Message;
                                //写入excel错误文档
                                worksheet.Cells[i + 2, columnsLength + 1].Value = message;
                            }
                        }
                    }
                    packge.Save();
                }
            }
            catch
            {

            }
            excelName = filename;
            return $"{_importDirectory}\\{filename}";
        }
    }
}
