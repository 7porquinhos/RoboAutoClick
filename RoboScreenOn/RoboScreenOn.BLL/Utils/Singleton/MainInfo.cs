using RoboScreenOn.BLL.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboScreenOn.BLL.Utils.Singleton
{
    public class MainInfo
    {
        public string KeyPress { get; set; }

        public string KeyPressPass { get; set; }

        public Point MousePosition { get; set; }

        public int Timer { get; set; }


        #region [DELEGATES]
        public delegate void DelegateEscreverConsoleLog(string text, TipoConsoleLog tipoConsoleLog, bool gravarLog = false, string nomeProcesso = "", Exception exception = null);
        public DelegateEscreverConsoleLog EscreverConsoleLog { get; set; }
        #endregion

        private MainInfo() { }

        public static MainInfo FCInfo()
        {
            return CriaInstancia.instancia;
        }

        private class CriaInstancia
        {
            internal static readonly MainInfo instancia = new MainInfo();
        }
    }
}
