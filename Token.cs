using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semanticabbc
{
    public class Token
    {
        public enum Tipos
        {
            Identificador, Numero, FinSentencia, OpTermino, OpFactor,
            OpLogico, OpRelacional, OpTernario, Asignacion, IncTermino,
            IncFactor, Cadena, Inicio, Fin, Caracter, TipoDato, Ciclo, 
            Condicion
        };
        private string _contenido;
        private Tipos _clasificacion;
        public string Contenido
        {
            get => _contenido;
            set => _contenido = value;
        }
        public Tipos Clasificacion
        {
            get => _clasificacion;
            set => _clasificacion = value;
        }
        public Token()
        {
            _contenido = "";
        }
        
    }
}