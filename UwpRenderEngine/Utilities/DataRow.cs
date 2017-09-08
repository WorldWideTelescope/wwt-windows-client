using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ShapefileTools
{
    public class DataTable
    {
        public DataTable()
        {

        }

        public List<DataRow> Rows = new List<DataRow>();
        public DataColumnCollection Columns = new DataColumnCollection();

        public DataRow NewRow()
        {
            DataRow row = new DataRow(this);
            foreach(string col in Columns.columns.Keys)
            {
                row[col] = null;
            }
            return row;
        }
    }


    public class DataColumn
    {
        private string columnName;
        private Type columnType;

        public DataColumn(string columnName, Type columnType)
        {
            this.columnName = columnName;
            this.columnType = columnType;
        }

        public string ColumnName
        {
            get
            {
                return columnName;
            }
        }
    }
    public class DataRow
    {
        DataTable table;

        public DataTable Table

        {
            get
            {
                return table;
            }
        }

        public DataRow(DataTable table)
        {
            this.table = table;
        }

        Dictionary<string, object> data = new Dictionary<string, object>();

        public object this[string name]
        {
            get
            {
                return data[name];
            }
            set
            {
                data[name] = value;
            }
        }

        public object this[int index]
        {
            get
            {
                return data[table.Columns.columnList[index].ColumnName];
            }
            set
            {
                data[table.Columns.columnList[index].ColumnName] = value;
            }
        }

        public Object[] ItemArray
        {
            get
            {
                List<object> list = new List<object>();

                foreach(var col in table.Columns.columnList)
                {
                    list.Add(data[col.ColumnName]);
                }
                return list.ToArray();
            }
        }
    }
  
    public class DataColumnCollection : IEnumerable<DataColumn>
    {
        public  Dictionary<string, DataColumn> columns = new Dictionary<string, DataColumn>();
        public List<DataColumn> columnList = new List<DataColumn>();

        //public IEnumerable<DataColumn> GetEnumerator()
        //{
        //    return columnList as IEnumerable<DataColumn>;
        //}

        public DataColumn this[string name]
        {
            get
            {
                return columns[name];
            }
            set
            {
                columns[name] = value;
            }
        }

        public DataColumn this[int index]
        {
            get
            {
                return columnList[index];
            }
            set
            {
                columnList[index] = value;
            }
        }

        protected List<DataColumn> List
        {
            get
            {
                return columns.Values.ToList();
            }
        }

    
        public void Add(DataColumn column)
        {
            columnList.Add(column);
            columns[column.ColumnName] = column;
        }

        public IEnumerator<DataColumn> GetEnumerator()
        {
            return ((IEnumerable<DataColumn>)columnList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<DataColumn>)columnList).GetEnumerator();
        }

        //public DataColumn Add(string columnName, Type type);

        //public DataColumn Add(string columnName, Type type, string expression);

        //public DataColumn Add(string columnName);

        //public void AddRange(DataColumn[] columns);

        //public bool CanRemove(DataColumn column);

        //public void Clear();

        //public bool Contains(string name);

        //public void CopyTo(DataColumn[] array, int index);

        //public int IndexOf(DataColumn column);

        //public int IndexOf(string columnName);

        //public void Remove(DataColumn column);

        //public void Remove(string name);

        //public void RemoveAt(int index);
    }
}