using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semanticabbc
{
    public class Sintaxis : Lexico
    {
        public Sintaxis()
        {
            nextToken();
        }
        public Sintaxis(string nombre) : base(nombre)
        {
            nextToken();
        }
        public void match(string espera)
        {
            if (getContenido() == espera)
            {
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un "+espera+" en la linea " +linea,log);
            }
        }
        public void match(Token.Tipos espera)
        {
            if (getClasificacion() == espera)
            {
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un "+espera+" en la linea " +linea,log);
            }
        }
    }
}