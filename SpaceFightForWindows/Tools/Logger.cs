using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SpaceShooter.Tools
{
    // klasa odpowiadająca zapisywanie logów
    class Logger
    {
        /// <summary>
        /// Przekazuje informacje do zapisania w logach
        /// </summary>
        /// <param name="message">Treść wiadomości</param>
        static public void Info(string message)
        {
            WriteLog(message, "INFO");
        }

        /// <summary>
        /// Przekazuje treść błędu do zapisania w logach
        /// </summary>
        /// <param name="ex">Błąd</param>
        /// <param name="extraData">Dodatkowe informacje</param>
        static public void Error(Exception ex, string extraData = null)
        {
            WriteLog(ex.Message, "ERROR", extraData);
            WriteLog(ex.StackTrace, "STACKTRACE");
        }
        /// <summary>
        /// Zapisuje do logów wiadomość
        /// </summary>
        /// <param name="message">Treść wiadomości</param>
        /// <param name="type">Typ np. error, info</param>
        /// <param name="extraData">Dodatkowe informacje</param>
        static private void WriteLog(string message, string type, string extraData = null) {
            string new_message;
            new_message = "| " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | [" + type + "] " + message + " | " + extraData;
            Trace.WriteLine(new_message);
        }
    }
}
