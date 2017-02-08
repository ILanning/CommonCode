using System;

namespace CommonCode.Windows
{
    public interface IDataDisplay
    {
        string FullName { get; }
        bool BindData(DataProvider provider);
        void UpdateData();
    }
}