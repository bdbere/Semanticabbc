using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Semanticabbc;
/*
    1.Colocar el numero de linea de errores lexicos y sintacticos
    2.Cambiar la clase token por atributos publicos(get, set)
    3.Cambiar los constructores de la clase lexico usando parametros
    por default
    4. Error semantico
    que es el postfijo
    char - 0...255 (1 byte)
    int - De 0 a 65.535
    float - De -3.4028234663852886E+38 a 3.4028234663852886E+38
    
*/

namespace Semanticabbc
{

    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        private Stack<float> s;

        public Lenguaje()
        {
            listaVariables = new List<Variable>();
            s = new Stack<float>();
        }

        public Lenguaje(string nombre) : base(nombre)
        {
            listaVariables = new List<Variable>();
            s = new Stack<float>();
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (getContenido() == "using")
            {
                Librerias();
            }
            Main();
            imprimeVariables();
        }
        //Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");
            if (getContenido() == "using")
            {
                Librerias();
            }
        }
        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        private void imprimeVariables()
        {
            foreach (Variable v in listaVariables)
            {
                log.WriteLine(v.getNombre() + " ( " + v.getTipo() + " ) = " + v.getValor());
            }
        }
        //Metodo para error semantico - Busqueda de Variable
        private Variable.TipoDato buscarVariable(string nombre)
        {
            foreach (Variable v in listaVariables)
            {
                if (v.getNombre() == nombre)
                {
                    return v.getTipo();
                }
            }
            throw new Exception("La variable " + nombre + " no ha sido declarada");
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            listaVariables.Add(new Variable(getContenido(), t));
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }
        Variable.TipoDato getTipo(string TipoDato)
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;
            switch (TipoDato)
            {
                case "int":
                    tipo = Variable.TipoDato.Int;
                    break;
                case "float":
                    tipo = Variable.TipoDato.Float;
                    break;
            }
            return tipo;
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            Variable.TipoDato tipo = getTipo(getContenido());
            match(Tipos.TipoDato);
            ListaIdentificadores(tipo);
            match(";");
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
            match("}");
        }
        //Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion()
        {
            if (getContenido() == "console")
            {
                Console();
            }
            else if (getContenido() == "if")
            {
                If();
            }
            else if (getContenido() == "while")
            {
                While();
            }
            else if (getContenido() == "do")
            {
                Do();
            }
            else if (getContenido() == "for")
            {
                For();
            }
            else if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
            }
        }
        // Asignacion -> Identificador = Expresion; |Solo esta opción se evalua semanticamente 
        // Id ++;
        // Id --;
        //Id += Expresion;
        //Id -= Expresion;
        //Id *= Expresion;
        //Id /= Expresion;
        //Id %= Expresion;
        /* Meter todos los casos*/


        private void Asignacion()
        {
            string variable = getContenido();
            match(Tipos.Identificador);
            if (getContenido() == "=")
            {
                match("=");
                Expresion();
            }
            else if (getContenido() == "++")
            {
                match("++");

            }
            else if (getContenido() == "--")
            {
                match("--");
            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion();

            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion();

            }
            else if (getContenido() == "*=")
            {
                match("*=");
                Expresion();

            }
            else if (getContenido() == "/=")
            {
                match("/=");
                Expresion();

            }
            else if (getContenido() == "%=")
            {
                match("%=");
                Expresion();

            }
            match(";");
            imprimeStack();
            float value = s.Pop();
            Variable.TipoDato tipo = buscarVariable(variable);

            switch (tipo)
            {
                case Variable.TipoDato.Char:
                    if (value < 0 || value > 255)
                    {
                        throw new Exception("El valor asignado a " + variable + " excede el rango de un char");
                    }
                    break;
                case Variable.TipoDato.Int:
                    if (value < 0 || value > 65535 || value != Math.Floor(value))
                    {
                        throw new Exception("El valor asignado a " + variable + " excede el rango de un int o no es un valor entero.");
                    }
                    break;
                   
            }
        }
        // If -> if (Condicion) bloqueInstrucciones | instruccion
        //    (else bloqueInstrucciones | instruccion)?
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }

            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
        }
        //Condicion -> Expresion operadorRelacional Expresion
        private void Condicion()
        {
            Expresion();
            match(Tipos.OpRelacional);
            Expresion();
        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }

        }
        //Do -> do 
        //        bloqueInstrucciones | intruccion 
        //    while(Condicion);
        private void Do()
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) 
        //    BloqueInstrucciones | Intruccion 
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            Condicion();
            match(";");
            Incremento();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }
        // Incremento -> Identificador ++ | --
        private void Incremento()
        {
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");
            }
            else
            {
                match("--");
            }
        }
        //Console -> Console.(WriteLine|Write) (cadena?); |
        //        Console.(Read | ReadLine) ();
        private void Console()
        {
            match("Console");
            match(".");
            if (getContenido() == "WriteLine" || getContenido() == "Write")
            {
                match(getContenido());
                match("(");
                if (getClasificacion() == Tipos.Cadena)
                {
                    match(Tipos.Cadena);
                }
                match(")");
            }
            else
            {
                if (getContenido() == "ReadLine")
                {
                    match("ReadLine");
                }
                else
                {
                    match("Read");
                }
                match("(");
                match(")");
            }
            match(";");
        }
        // Main      -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones();
        }
        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        // MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OpTermino)
            {
                string operador = getContenido();
                match(Tipos.OpTermino);
                Termino();
                float R1 = s.Pop();
                float R2 = s.Pop();
                switch (operador)
                {
                    case "+": s.Push(R2 + R1); break;
                    case "-": s.Push(R2 - R1); break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OpFactor)
            {
                string operador = getContenido();
                match(Tipos.OpFactor);
                Factor();
                float R1 = s.Pop();
                float R2 = s.Pop();
                switch (operador)
                {
                    case "*": s.Push(R2 * R1); break;
                    case "/": s.Push(R2 / R1); break;
                    case "%": s.Push(R2 % R1); break;
                }
            }
        }

        private void imprimeStack()
        {
            log.Write("Stack: ");
            foreach (float e in s.Reverse())
            {
                log.Write(e + " ");
            }
            log.WriteLine();
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                s.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                if (getClasificacion() == Tipos.TipoDato)
                {
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
            }
        }

    }
}