using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BogdanovUtilitisLib.Logger.Tests.Cases
{
    public class TestClasses1
    {
        public void AnalyzeLog(bool isException)
        {
            try
            {
                LogsWrapper.TmpLogError.Start("A");
                if (isException)
                {
                    LogsWrapper.TmpLogError.Add("A", "Лог перед исключением");
                    throw new Exception();
                    LogsWrapper.TmpLogError.Add("A", "Лог после исключения");
                }
                else 
                { 
                
                }
                LogsWrapper.TmpLogError.Stop("A");
            }
            catch (Exception err)
            {
                LogsWrapper.Logger.Error(LogsWrapper.TmpLogError.FinishTrace("A")
                    .ToString() + err.Message);
            }
        }

        public void AnalyzeLog2(bool isException)
        {
            try
            {
                LogsWrapper.TmpLogError.Start("A");
                if (isException)
                {
                    LogsWrapper.TmpLogError.Add("A", "Лог перед исключением");
                    InnerAnalyzer();
                    throw new Exception();
                    LogsWrapper.TmpLogError.Add("A", "Лог после исключения");
                }
                else
                {

                }
                LogsWrapper.TmpLogError.Stop("A");
            }
            catch (Exception err)
            {
                LogsWrapper.Logger.Error(LogsWrapper.TmpLogError.FinishTrace("A")
                    .ToString() + err.Message);
            }
        }

        void InnerAnalyzer()
        {
            LogsWrapper.TmpLogError.Add("A", "Начало метода");
            LogsWrapper.TmpLogError.Add("A", "Окончание метода");
        }

    }
}
