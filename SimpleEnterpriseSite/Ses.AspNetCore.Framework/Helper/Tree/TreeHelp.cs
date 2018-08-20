using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Ses.AspNetCore.Framework.Helper.Tree
{
    public static class TreeHelp
    {
        private static string _defaultId = Guid.Empty.ToString();

        public static string CreateTreeJson(List<SesTreeModel> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(CreateTreeJson(data, _defaultId, ""));
            sb.Append("]");
            return sb.ToString();
        }

        public static string CreateTreeJson(List<SesTreeModel> data, string parentId, string blank)
        {
            StringBuilder sb = new StringBuilder();
            string defaultId = Guid.Empty.ToString();
            var childNodeList = data.FindAll(x => x.ParentId == parentId);
            //子节点 缩进
            var tabLine = "";
            if (parentId != defaultId)
            {
                tabLine = "   ";
            }
            if (childNodeList.Count > 0)
            {
                tabLine += blank;
            }
            foreach (SesTreeModel item in childNodeList)
            {
                item.Text += tabLine;
                string strJson = item.Serialize();
                sb.Append(strJson);
                sb.Append(CreateTreeJson(data, item.Id, tabLine));
            }
            return sb.ToString().Replace("}{", "},{");
        }

        public static void CreateTreeModel<T>(List<T> data, string parentId, int level, Func<T, string> idSelector, Func<T, string> parentIdSelector, ref List<SesTreeModel> list, Func<T, int?> order = null)
        {
            var childNodeList = data.Where(x => parentIdSelector(x) == parentId);

            if (order != null)
            {
                childNodeList = childNodeList.OrderBy(x => order(x));
            }
            foreach (var item in childNodeList)
            {
                SesTreeModel stm = new SesTreeModel
                {
                    Id = idSelector(item),
                    ParentId = parentId,
                    Level = level,
                    Data = item,
                    HasChildren = data.Any(x => parentIdSelector(x) == idSelector(item))
                };
                list.Add(stm);
                CreateTreeModel(data, stm.Id, level + 1, idSelector, parentIdSelector, ref list, order);
            }
        }

        /// <summary>
        /// 树形结构查询，在创建树形结构之前进行数据查询.找到所有满足查询的项，并追溯其所有的父项，最后以树形结构展示出来
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">源数据</param>
        /// <param name="where">查询条件Predicate委托</param>
        /// <param name="idSelector">选择T类型的Id属性委托</param>
        /// <param name="parentSelector">选择T类型的ParentId属性委托</param>
        /// <returns></returns>
        public static List<T> TreeWhere<T>(List<T> data, Predicate<T> @where, Func<T, string> idSelector, Func<T, string> parentSelector)
        {
            List<T> currentList = data.FindAll(@where);
            List<T> resultList = new List<T>();

            foreach (var item in currentList)
            {
                resultList.Add(item); //将当前项放入集合中
                var currentItem = item;

                //循环查找其父项,达到递归的目的
                while (true)
                {
                    var parentId = parentSelector(currentItem);
                    if (parentId == _defaultId)
                        break;
                    var prevItem = data.Find(x => idSelector(x) == parentId);
                    if (prevItem != null) //当前项存在父项
                    {
                        resultList.Add(prevItem);
                        currentItem = prevItem;
                    }
                    else
                    {
                        break;
                    }

                }
            }
            return resultList.Distinct().ToList();
        }



    }
}
