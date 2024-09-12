using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Semanticabbc;
/*
    1.Colocar el numero de linea de errores lexicos y sintacticos
    2.Cambiar la clase token por atributos privados(get, set)
    3.Cambiar los constructores de la clase lexico usando parametros
    por default
    4. Error semantico
    que es el postfijo
    
    char - 0...255 (1 byte) listo
    int - De 0 a 65.535 listo
   
    
*/

namespace Semanticabbc
{

    public class Lenguaje : Sintaxis
    {
        Token token = new Token();
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
            if (Contenido == "using")
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
            if (Contenido == "using")
            {
                Librerias();
            }
        }
        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
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
            throw new Error("La variable " + nombre + " no ha sido declarada en la linea "+linea,log);
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            listaVariables.Add(new Variable(Contenido, t));
            match(Tipos.Identificador);
            if (Contenido == ",")
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
            Variable.TipoDato tipo = getTipo(Contenido);
            match(Tipos.TipoDato);
            ListaIdentificadores(tipo);
            match(";");
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (Contenido != "}")
            {
                ListaInstrucciones();
            }
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {
            match("{");
            if (Contenido != "}")
            {
                ListaInstrucciones();
            }
            match("}");
        }
        //Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion()
        {
            if (Contenido == "console")
            {
                Console();
            }
            else if (Contenido == "if")
            {
                If();
            }
            else if (Contenido == "while")
            {
                While();
            }
            else if (Contenido == "do")
            {
                Do();
            }
            else if (Contenido == "for")
            {
                For();
            }
            else if (Clasificacion == Tipos.TipoDato)
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
            string variable = Contenido;
            match(Tipos.Identificador);
    
            if (Contenido == "++")
            {
                match("++");

            }
            else if (Contenido == "--")
            {
                match("--");
            }
            else if (Contenido == "=")
            {
                match("=");
            }
            else if (Contenido == "+=")
            {
                match("+=");

            }
            else if (Contenido == "-=")
            {
                match("-=");

            }
            else if (Contenido == "*=")
            {
                match("*=");

            }
            else if (Contenido == "/=")
            {
                match("/=");

            }
            else if (Contenido == "%=")
            {
                match("%=");

            }
            if (Clasificacion == Tipos.Numero || Clasificacion == Tipos.Identificador || Contenido == "(")
            {
                Expresion();
                float value = s.Pop();
                Variable.TipoDato tipo = buscarVariable(variable);

                switch (tipo)
                {
                    case Variable.TipoDato.Char:
                        if (value < 0 || value > 255)
                        {
                            throw new Error("El valor asignado a " + variable + " excede el rango de un char en la linea "+linea,log);
                        }
                        break;
                    case Variable.TipoDato.Int:
                        if (value < 0 || value > 65535 || value != Math.Floor(value))
                        {
                            throw new Error("El valor asignado a " + variable + " excede el rango de un int o no es un valor entero en la linea "+linea,log);
                        }
                        break;

                }
            }
            match(";");
            //imprimeStack();
            
        }
        // If -> if (Condicion) bloqueInstrucciones | instruccion
        //    (else bloqueInstrucciones | instruccion)?
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }

            if (Contenido == "else")
            {
                match("else");
                if (Contenido == "{")
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
            if (Contenido == "{")
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
            if (Contenido == "{")
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
            if (Contenido == "{")
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
            if (Contenido == "++")
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
            if (Contenido == "WriteLine" || Contenido == "Write")
            {
                match(Contenido);
                match("(");
                if (Clasificacion == Tipos.Cadena)
                {
                    match(Tipos.Cadena);
                }
                match(")");
            }
            else
            {
                if (Contenido == "ReadLine")
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
            if (Clasificacion == Tipos.OpTermino)
            {
                string operador = Contenido;
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
            if (Clasificacion == Tipos.OpFactor)
            {
                string operador = Contenido;
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
            if (Clasificacion == Tipos.Numero)
            {
                s.Push(float.Parse(Contenido));
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                if (Clasificacion == Tipos.TipoDato)
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