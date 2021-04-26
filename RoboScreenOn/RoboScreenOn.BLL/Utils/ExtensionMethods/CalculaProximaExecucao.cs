using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboScreenOn.BLL.Utils.ExtensionMethods
{
    public static class CalculaProximaExecucao
    {
        public static string Diferenca(this string dataHora, string dataHoraAtual)
        {
            if (!string.IsNullOrEmpty(dataHora) && !string.IsNullOrEmpty(dataHoraAtual))
            {
                var data1 = DateTime.Parse(dataHora);
                var data2 = DateTime.Parse(dataHoraAtual);
                var tempoExecucao = data1 - data2;
                return tempoExecucao.ToString();
            }
            return "00:00:00";
        }

        public static TimeSpan CalcularProximaExecucao(this DateTime dataHora, int minuto)
        {
            var time1 = new TimeSpan(dataHora.Hour, dataHora.Minute, dataHora.Second);
            var time2 = new TimeSpan(00, minuto, 0);

            return time1.Add(time2);
        }
    }
}
