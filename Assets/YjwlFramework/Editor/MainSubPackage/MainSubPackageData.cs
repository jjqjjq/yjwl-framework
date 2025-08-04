using JQEditor.Excel;

namespace JQEditor.MainSubPackage
{
    public class MainSubPackageData
    {
        
        [JQExcelColumn]
        public string Name { get; set; }//资源名
        [JQExcelColumn]
        public int isInMainPackage { get; set; }//是否在主包 1是首包，2是分包，0是未设置
        [JQExcelColumn]
        public int DownloadPriority { get; set; }//下载优先级
        
        
    }
}