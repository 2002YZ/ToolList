using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Helper
{
    /// <summary>
    /// ViewModel中的命令类，封装了一个委托来执行命令逻辑
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;  // 定义事件

        public bool CanExecute(object? parameter)    // 定义方法来判断命令是否可以执行
        {
            if (this.CanExecuteFunction == null)
            {
                return true;   // 如果没有存储的委托，则默认命令可以执行
            }
            return this.CanExecuteFunction(parameter);   // 调用存储的委托来判断命令是否可以执行
        }

        public void Execute(object? parameter)    // 定义方法来执行命令
        {
            if (this.ExecuteAction == null)
            {
                return;   // 如果没有存储的委托，则不执行任何操作
            }
            this.ExecuteAction(parameter);    // 调用存储的委托来执行命令逻辑
        }


        public Action<object?> ExecuteAction { get; set; }// 定义一个属性来存储执行命令的委托

        public Func<object?, bool> CanExecuteFunction { get; set; }// 定义一个属性来存储判断命令是否可以执行的委托
    }
}
