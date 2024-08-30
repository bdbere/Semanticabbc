using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;


namespace Semanticabbc
{


    public class Lenguaje : Sintaxis
    {


        public Lenguaje()
        {
            //prueba comentario para git
        }


        public Lenguaje(string nombre) : base(nombre)
        {

        }
        // Programa -> Librerías? Variables? Main
        public void Programa()
        {
            //Librerias? Variables? Main
            if (getContenido() == "using")
            {
                Librerias();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            Main();
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
        // ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        // Variables -> tipoDato ListaIdentificadores; Variables?
        private void Variables()
        {
            match(Tipos.TipoDato);
            ListaIdentificadores(tipoDato);
            match(";");
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
        }

        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores()
        {
            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores();
            }
        }
        // BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
            match("}");
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
        }
        // Instruccion -> Console | If | While | Do | For | Asignacion
        private void Instruccion()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            else if (getContenido() == "Console")
            {
                console();
            }
            else if (getContenido() == "if")
            {
                If(evaluacion);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion);
            }
            else
            {
                Asignacion(evaluacion);
            }
        }
        // Asignacion -> Identificador = Expresion;
        //               Identificador = Console.ReadLine();

        private void Asignacion(bool evaluacion)
        {
            string nombre = getContenido();
            float resultado = 0;
            log.Write(nombre + "=");

            if (!ExisteVariable(nombre))
            {
                throw new Error("de sintaxis, la variable <" + nombre + "> no está declarada", log);
            }
            match(Tipos.Identificador);
            if (getContenido() == "=")
            {
                match("=");
                if (getContenido() == "Console")
                {
                    match("Console");
                    match(".");
                    if (getContenido() == "Read")
                    {
                        match("Read");
                    }
                    else if (getContenido() == "ReadLine")
                    {
                        match("ReadLine");
                    }
                    match("(");
                    match(")");
                    string value = "" + Console.ReadLine();
                    s.Push(float.Parse(value));
                    asm.WriteLine("call scan_num");
                    asm.WriteLine("printn \"\"");
                    asm.WriteLine("MOV AX, CX");
                    asm.WriteLine("PUSH AX");

                    resultado = s.Pop();
                    modificaVariable(nombre, resultado);

                }
                else
                {
                    Expresion();
                    resultado = s.Pop();
                    modificaVariable(nombre, resultado);

                }
                asm.WriteLine("POP AX");
                asm.WriteLine("MOV " + nombre + ",AX");

            }
            else if (getContenido() == "++")
            {
                match(Tipos.IncTermino);
                resultado = Valor(nombre) + 1;

                modificaVariable(nombre, resultado);
                asm.WriteLine("INC " + nombre);
                asm.WriteLine("MOV AX, " + nombre);
                asm.WriteLine("PUSH AX");
                asm.WriteLine("POP AX");
                asm.WriteLine("MOV " + nombre + ",AX");

            }
            else if (getContenido() == "--")
            {
                match(Tipos.IncTermino);
                resultado = Valor(nombre) - 1;
                modificaVariable(nombre, resultado);
                asm.WriteLine("DEC " + nombre);
                asm.WriteLine("MOV AX, " + nombre);
                asm.WriteLine("PUSH AX");
                asm.WriteLine("POP AX");
                asm.WriteLine("MOV " + nombre + ",AX");
            }
            else
            {
                if (getContenido() == "+=")
                {
                    match("+=");
                    Expresion();
                    resultado = Valor(nombre) + s.Pop();
                    modificaVariable(nombre, resultado);
                    asm.WriteLine("POP AX");
                    asm.WriteLine("ADD " + nombre + ",AX");
                    asm.WriteLine("MOV AX, " + nombre);
                    asm.WriteLine("PUSH AX");
                    asm.WriteLine("POP AX");
                    asm.WriteLine("MOV " + nombre + ",AX");
                }
                else if (getContenido() == "-=")
                {
                    match("-=");
                    Expresion();
                    resultado = Valor(nombre) - s.Pop();
                    modificaVariable(nombre, resultado);
                    asm.WriteLine("POP AX");
                    asm.WriteLine("SUB " + nombre + ",AX");
                    asm.WriteLine("MOV AX, " + nombre);
                    asm.WriteLine("PUSH AX");
                    asm.WriteLine("POP AX");
                    asm.WriteLine("MOV " + nombre + ",AX");
                }
                else if (getContenido() == "*=")
                {
                    match("*=");
                    Expresion();
                    resultado = Valor(nombre) * s.Pop();
                    modificaVariable(nombre, resultado);
                    asm.WriteLine("POP AX");
                    asm.WriteLine("MOV BX, " + nombre);
                    asm.WriteLine("MUL BX");
                    asm.WriteLine("PUSH AX");
                    asm.WriteLine("MOV " + nombre + ",AX");
                }
                else if (getContenido() == "/=")
                {
                    match("/=");
                    Expresion();
                    resultado = Valor(nombre) / s.Pop();
                    modificaVariable(nombre, resultado);
                    asm.WriteLine("POP BX");
                    asm.WriteLine("MOV AX, " + nombre);
                    asm.WriteLine("DIV BX");
                    asm.WriteLine("PUSH BX");
                    asm.WriteLine("MOV " + nombre + ",AX");
                }
                else if (getContenido() == "%=")
                {
                    match("%=");
                    Expresion();
                    resultado = Valor(nombre) % s.Pop();
                    modificaVariable(nombre, resultado);
                    asm.WriteLine("POP AX");
                    asm.WriteLine("MOV BX, " + nombre);
                    asm.WriteLine("DIV BX");
                    asm.WriteLine("PUSH DX");
                    asm.WriteLine("MOV " + nombre + ", DX");
                }


            }
            match(";");
            asm.WriteLine();



        }
        // If -> if (Condicion) bloqueInstrucciones | instruccion
        //       (else bloqueInstrucciones | instruccion)?
        private void If(bool evaluacion)
        {
            match("if");
            match("(");
            bool EvaluaIF = Condicion() && evaluacion;
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(EvaluaIF);
            }
            else
            {
                Instruccion(EvaluaIF);
            }
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!EvaluaIF);
                }
                else
                {
                    Instruccion(!EvaluaIF);
                }
            }
        }

        // Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion(bool evaluacion)
        {
            Expresion(); // E1
            string operador = getContenido();
            match(Tipos.OpRelacional);
            Expresion(); // E2
            float R2 = s.Pop();
            float R1 = s.Pop();
            switch (operador)
            {
                case ">": return R1 > R2;
                case ">=": return R1 >= R2;
                case "<": return R1 < R2;
                case "<=": return R1 <= R2;
                case "==": return R1 == R2;
                default: return R1 != R2;
            }

        }

        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            bool EvaluaWHILE = Condicion(evaluacion) && evaluacion;
            Console.WriteLine(Condicion(evaluacion));
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(EvaluaWHILE);
            }
            else
            {
                Instruccion(EvaluaWHILE);
            }
        }
        // Do -> do 
        //         bloqueInstrucciones | instruccion 
        //       while(Condicion);
        private void Do(bool evaluacion)

        {
            match("do");
            bool EvaluaDO = Condicion(evaluacion) && evaluacion;
            if (getContenido() == "{")
            {
                BloqueInstrucciones(EvaluaDO);
            }
            else
            {
                Instruccion(EvaluaDO);
            }
            match("while");
            match("(");
            Console.WriteLine(Condicion(evaluacion));
            match(")");

        }

        // For -> for(Asignacion Condicion; Incremento) 
        //        BloqueInstrucciones | Instruccion 
        private void For(bool evaluacion)
        {
            match("for");
            match("(");
            Asignacion(evaluacion);
            bool EvaluaFOR = Condicion(evaluacion) && evaluacion;
            Console.WriteLine(Condicion(evaluacion));
            match(";");
            Incremento();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(EvaluaFOR);
            }
            else
            {
                Instruccion(EvaluaFOR);
            }
        }
        // Incremento -> Identificador ++ | --
        private void Incremento()
        {

            string nombre = getContenido();
            float resultado = 0;
            if (!ExisteVariable(nombre))
            {
                throw new Error("de sintaxis, la variable <" + nombre + "> no está declarada", log);
            }
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                asm.WriteLine("INC " + nombre + "\n");
                resultado = Valor(nombre) + 1;
                match("++");
            }
            else
            {
                asm.WriteLine("DEC " + nombre + "\n");
                resultado = Valor(nombre) - 1;
                match("--");
            }
        }
        // console -> Console.(WriteLine|Write) (cadena); |
        //            Console.(Read | ReadLine) ();
        private void console(bool evaluacion)
        {
            match("Console");
            match(".");
            if (getContenido() == "WriteLine" || getContenido() == "Write")
            {
                match(getContenido());
                match("(");
                if (getClasificacion() == Tipos.Cadena)
                {
                    match(Tipos.Cadena); //optativa
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
            }


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
        // Expresion -> Termino MasTermino
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
                match(Tipos.OpTermino);
                Termino();
            }
        }

        // Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        // PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OpFactor)
            {

                match(Tipos.OpFactor);
                Factor();

            }
        }

        // Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match("(");

            }
        }


    }
}