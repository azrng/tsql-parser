namespace ConsoleApp
{
    internal class AnalysisSqlDetailsDto
    {  /// <summary>
       /// 查询字段
       /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 正则操作符
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 条件的值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 是否是正则
        /// </summary>
        public bool IsRegular { get; set; }

        /// <summary>
        /// 是否是否定操作符
        /// </summary>
        public bool IsDenyOperator { get; set; }

        /// <summary>
        /// 是否是非结构化
        /// </summary>
        public bool IsNotStuct { get; set; }
    }
}