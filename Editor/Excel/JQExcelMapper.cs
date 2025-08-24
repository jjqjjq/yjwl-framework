using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OfficeOpenXml;
using UnityEngine;

namespace JQEditor.Excel
{
    public class JQExcelMapper<T>
    {
        private Dictionary<string, JQExcelColumnInfo> _columnInfoDic = new Dictionary<string, JQExcelColumnInfo>();
        private List<T> _dataList = new List<T>();
        private string _excelPath;
        private int _nameRow;
        private int _firstDataRow;

        public JQExcelMapper(string excelPath, int firstRow, int nameRow)
        {
            _excelPath = excelPath;
            _firstDataRow = firstRow;
            _nameRow = nameRow;
            AnalyzeType();
            initExcel();
        }

        public List<T> DataList
        {
            get { return _dataList; }
            set { _dataList = value; }
        }


        private void initExcel()
        {
            FileInfo fileInfo = new FileInfo(_excelPath);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int colEnd = worksheet.Dimension.End.Column;

                //获取字段名称并记录字段所在列
                for (int i = 1; i <= colEnd; i++)
                {
                    string colName = worksheet.Cells[_nameRow, i].Value.ToString();
                    if (_columnInfoDic.ContainsKey(colName))
                    {
                        _columnInfoDic[colName].ColumnIndex = i;
                    }
                    else
                    {
                        Debug.LogError($"字段名{colName}在类{typeof(T).Name}中不存在");
                    }
                }
            }
        }

        public void WriteOneRowToExcel(T excelData, int id)
        {
            FileInfo fileInfo = new FileInfo(_excelPath);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                foreach (KeyValuePair<string, JQExcelColumnInfo> keyValuePair in _columnInfoDic)
                {
                    JQExcelColumnInfo jqExcelColumnInfo = keyValuePair.Value;
                    worksheet.Cells[id + _firstDataRow - 1, jqExcelColumnInfo.ColumnIndex].Value = jqExcelColumnInfo.GetProperty(excelData);
                }
                package.Save();
            }
        }

        public void WriteToExcel()
        {
            FileInfo fileInfo = new FileInfo(_excelPath);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                
                for (int i = 0; i < _dataList.Count; i++)
                {
                    T excelData = _dataList[i];
                    foreach (KeyValuePair<string, JQExcelColumnInfo> keyValuePair in _columnInfoDic)
                    {
                        JQExcelColumnInfo jqExcelColumnInfo = keyValuePair.Value;
                        worksheet.Cells[i + _firstDataRow, jqExcelColumnInfo.ColumnIndex].Value = jqExcelColumnInfo.GetProperty(excelData);
                    }
                }

                for (int i = _dataList.Count; i < worksheet.Cells.Rows; i++)
                {
                    worksheet.DeleteRow(i);
                }

                package.Save();
            }
        }

        public void ReadFromExcel()
        {
            FileInfo fileInfo = new FileInfo(_excelPath);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int rowEnd = worksheet.Dimension.End.Row;
                for (int i = _firstDataRow; i <= rowEnd; i++)
                {
                    T excelData = Activator.CreateInstance<T>();
                    foreach (KeyValuePair<string, JQExcelColumnInfo> keyValuePair in _columnInfoDic)
                    {
                        JQExcelColumnInfo jqExcelColumnInfo = keyValuePair.Value;
                        jqExcelColumnInfo.SetProperty(excelData, worksheet.Cells[i, jqExcelColumnInfo.ColumnIndex].Value);
                    }

                    _dataList.Add(excelData);
                }
            }
        }


        private void AnalyzeType()
        {
            Type type = typeof(T);
            PropertyInfo[] props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo prop in props)
            {
                Attribute[] attribs = Attribute.GetCustomAttributes(prop, typeof(JQExcelColumnAttribute), false);

                if (attribs.Any())
                {
                    JQExcelColumnInfo ci = new JQExcelColumnInfo(prop.Name, prop);
                    _columnInfoDic[prop.Name] = ci;
                }
            }
        }
    }
}