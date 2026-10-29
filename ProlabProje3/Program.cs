using System;
using System.Windows.Forms;

namespace ProlabProje3
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
       
            Application.Run(new ProjeEkr());
        }
    }
}