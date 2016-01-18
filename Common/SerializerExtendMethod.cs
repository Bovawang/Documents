public static class SerializerExtendMenthod
{
    ///
    /// 将DataTable转换为List<T>
    ///
    public static List<T> ToList<T>(this DataTable dt)
    {
        var columnNames = dt.Columns.Cast<DataColumn>()
            .Select(c=>c.ColumnName)
            .ToList();
            
        var properties = typeof(T).GetProperties();
        
        return dt.AsEnumerable().Select(row=>
        {
           var destObj = Activator.CreateInstance<T>();
           
           foreach (var prop in properties)
           {
                if (columnNames.Contains(prop.Name))
                {
                    prop.SetValue(destObj, row[prop.Name] == DBNull.Value ? string.Empty : row[prop.Name].ToString(),null);
                }
           }
           
           return destObj;
        }).ToList();
    }    
    
    ///
    /// 将DataTable转换为Json字符串(Json.Net组件进行 Json数据序列化)
    ///
    public static string ToJson(this DataTable dt)
    {
        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
        foreach (DataRow row in dt.Rows)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (DataColumn col in row.Columns)
            {
                dict[col.ColumnName] = row[col];
            }
            list.Add(dict);
        }
        
        return JsonConvert.Serialize(list);
    }
}