using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.ComponentModel;
using Microsoft.Office.Interop.Excel;


namespace Common
{
    public static class Extension
    {
        /// <summary>
        /// Extension method to write list data to excel.
        /// </summary>
        /// <typeparam name="T">Ganeric list</typeparam>
        /// <param name="list"></param>
        /// <param name="PathToSave">Path to save file.</param>
        public static string ToExcel<T>(this IList<T> list,string fileName="DefaultName.xls",string groupBy="")
        {
            #region Declarations


            if (string.IsNullOrEmpty(Config.Instance.FilePath))
            {
                throw new Exception("Invalid file path.");
            }
            var di = new DirectoryInfo(Config.Instance.FilePath);
            if (!di.Exists)
            {
                di.Create();
            }
            var pathToSave = di.FullName + fileName;

            if (list == null || list.Count == 0)
            {
                return string.Empty;
            }



            var groupList =(string.IsNullOrEmpty(groupBy)?list.GroupBy(d => groupBy): list.GroupBy(d => d.GetType().GetProperty(groupBy).GetValue(d, null).ToString()));
           

            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Microsoft.Office.Interop.Excel.Workbooks books = null;
            Microsoft.Office.Interop.Excel._Workbook book = null;
            Microsoft.Office.Interop.Excel.Sheets sheets = null;
            Microsoft.Office.Interop.Excel._Worksheet sheet = null;
            Microsoft.Office.Interop.Excel.Range range = null;
            Microsoft.Office.Interop.Excel.Font font = null;
            // Optional argument variable
            object optionalValue = Missing.Value;

            string strHeaderStart = "A1";
            string strDataStart = "A2";
            #endregion

            #region Processing


            try
            {
                #region Init Excel app.


                excelApp = new Microsoft.Office.Interop.Excel.Application();
                books = (Microsoft.Office.Interop.Excel.Workbooks)excelApp.Workbooks;
                book = (Microsoft.Office.Interop.Excel._Workbook)(books.Add(optionalValue));
                sheets = (Microsoft.Office.Interop.Excel.Sheets)book.Worksheets;
               
                        
                 
                

                #endregion

                int sheetNum = 1;
                foreach (var groupData in groupList)
                {
                    //foreach (var data in groupData)
                    //{
                        sheet = (Microsoft.Office.Interop.Excel._Worksheet)(sheets.get_Item(sheetNum));
                        if (!string.IsNullOrEmpty(groupData.Key))
                        {
                            sheet.Name = groupData.Key.ToString();
                        }
                #region Creating Header


                Dictionary<string, string> objHeaders = new Dictionary<string, string>();

                PropertyInfo[] headerInfo = typeof(T).GetProperties();


                foreach (var property in headerInfo)
                {
                    var attribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                                            .Cast<DisplayNameAttribute>().FirstOrDefault();
                    objHeaders.Add(property.Name, attribute == null ?
                                        property.Name : attribute.DisplayName);
                }


                range = sheet.get_Range(strHeaderStart, optionalValue);
                range = range.get_Resize(1, objHeaders.Count);

                range.set_Value(optionalValue, objHeaders.Values.ToArray());
                range.BorderAround(Type.Missing, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);

                font = range.Font;
                font.Bold = true;
                //range.Interior.Color = Color.LightGray.ToArgb();

                #endregion

                #region Writing data to cell


                int count = groupData.Count();
                object[,] objData = new object[count, objHeaders.Count];

                int j=0;
                foreach (var data in groupData)
                {
                    var item = data;
                    int i = 0;
                    foreach (KeyValuePair<string, string> entry in objHeaders)
                    {
                        var y = typeof(T).InvokeMember(entry.Key.ToString(), BindingFlags.GetProperty, null, item, null);
                        objData[j, i++] = (y == null) ? "" : (y.GetType() == typeof(DateTime) ? ((DateTime)y).ToString("yyyy-MM-dd") : y.ToString());
                    }
                    j++;
                }


                range = sheet.get_Range(strDataStart, optionalValue);
                range = range.get_Resize(count, objHeaders.Count);

                range.set_Value(optionalValue, objData);
                range.BorderAround(Type.Missing, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);

                range = sheet.get_Range(strHeaderStart, optionalValue);
                range = range.get_Resize(count + 1, objHeaders.Count);
                range.Columns.AutoFit();


                #endregion
                sheetNum++;
                    //}
                }

                #region Saving data and Opening Excel file.


                if (string.IsNullOrEmpty(pathToSave) == false)
                    book.SaveAs(pathToSave);

                //excelApp.Visible = true;
                excelApp.Quit();
                #endregion

                #region Release objects

                try
                {
                    if (sheet != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                    sheet = null;

                    if (sheets != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(sheets);
                    sheets = null;

                    if (book != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
                    book = null;

                    if (books != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(books);
                    books = null;

                    if (excelApp != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    
                    excelApp = null;
                }
                catch (Exception ex)
                {
                    sheet = null;
                    sheets = null;
                    book = null;
                    books = null;
                    excelApp = null;
                }
                finally
                {
                    GC.Collect();
                }

                return fileName;

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion
        }
    }
}
