using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SistemaExperto
{
    public class BCObject
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public List<string> Props { get; set; }
    }

    class Program
    {
        private List<BCObject> BcObjects = new List<BCObject>();
        private List<string> RefusedProps = new List<string>();
        private List<string> AprovedProps = new List<string>();
        static void Main(string[] args)
        {
          
            int option = 0;
            Program program = new Program();

            program.ReadCSVFile();

            while (option >= 1 || option <= 5)
            {
                option = program.ShowMenu();

                switch (option)
                {
                    case 1:
                        Console.WriteLine("Mostrar Objetos\n\n");
                        program.ShowAllObjects();
                        break;
                    case 2:
                        Console.WriteLine("Consultar al SE");
                        program.QueryObjects();
                        break;
                    case 3:
                        Console.WriteLine("Sistema Experto");
                        break;
                    case 4:
                        Console.WriteLine("Sistema Experto");
                        break;
                    case 5:
                        Console.WriteLine("Salir");
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            }

        }

        public int ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("Sistema Experto");
            Console.WriteLine(" \n 1.Ver Objetos de la BC \n 2.Consultar al SE \n 3.Guardar la BC \n 4.Usar una BC existente \n 5.Salir ");
            int option = Convert.ToInt32(Console.ReadLine());
            return option;
        }
        
        public void ReadCSVFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader("H:/BaseDeConocimientos.csv"))
                {
                    string currentLine;
                    // currentLine will be null when the StreamReader reaches the end of file
                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        string[] line = currentLine.Split(',');
                        CreateNewObject(line);
                        Console.WriteLine(line[0]);
                    }
                }
                Console.WriteLine("Ya esta");
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public int CreateNewObject(string[] currentObject)
        {
            int cont = 0;
            BCObject newObject = new BCObject();
            newObject.Props = new List<string>();

            foreach (string prop in currentObject)
            {
                if (cont == 0)
                {
                    newObject.Value = prop;
                }
                else if(cont == 1)
                {
                    newObject.Description = prop;
                }
                else
                {
                    if(prop != "")
                    {
                        newObject.Props.Add(prop);
                    }
                }
                cont++;
            }

            this.BcObjects.Add(newObject);

            return 1;
        }

        public void ShowAllObjects()
        {
            foreach(BCObject obj in this.BcObjects){
                Console.WriteLine("Titulo: " + obj.Value);
                Console.WriteLine("Descripcion: " + obj.Description);
                Console.WriteLine("Caracteristicas: ");
                foreach(string prop in obj.Props)
                {
                    Console.WriteLine(" : " + prop);
                }
                Console.WriteLine(" ");
            }

            Console.WriteLine("\nPresione ENTER para Continuar...");
            Console.ReadLine().ToLower();
        }


        public int QueryObjects()
        {
            this.RefusedProps = new List<string>();
            this.AprovedProps = new List<string>();
            string response = "";
            bool containsRefusedProp;
            bool containsAprovedProp;
            bool firstsAsk = true;
            bool foundObject = false;
            int aprovedProps;
            string instructions;

            foreach (BCObject obj in BcObjects)
            {
                containsRefusedProp = false;
                containsAprovedProp = true;
                aprovedProps = 0;

                if (foundObject == true)
                {
                    Console.WriteLine("\nExisten otras soluciones, desea continuar la busqueda? \nIngresa 's' para continuar la busqueda, cualquier otra tecla para salir...");
                    response = Console.ReadLine().ToLower();
                    if (response != "s")
                    {
                        break;
                    }
                    else
                    {
                        foundObject = false;
                    }
                }

                #region search for refused props
                foreach (string refusedProp in this.RefusedProps)
                {
                    if (obj.Props.Contains(refusedProp))
                    {
                        containsRefusedProp = true;
                        break;
                    }
                }
                if (containsRefusedProp)
                {
                    continue;
                }
                #endregion

                #region search for aproved props
                foreach(string aprovedProp in this.AprovedProps)
                {
                    if (!obj.Props.Contains(aprovedProp))
                    {
                        containsAprovedProp = false;
                        break;
                    }
                }
                if (!containsAprovedProp)
                {
                    continue;
                }
                #endregion

                foreach (string prop in obj.Props)
                {
                    

                    response = "";
                    if (this.AprovedProps.Contains(prop))
                    {
                        aprovedProps++;
                        continue;
                    }

                    instructions = firstsAsk ? "Ingresa 's' si tu respuesta es si o 'n' para no." : "Ingresa 's' si tu respuesta es si o 'n' para no. Ingresa 'i' Para solicitar información del proceso.";

                    while (response != "s" && response != "n")
                    {
                        Console.WriteLine("Es " + prop + "?");
                        Console.WriteLine(instructions);
                        response = Console.ReadLine().ToLower();

                        if(response == "i")
                        {
                            response = ShowProcess(obj, prop);
                        }
                    }

                    firstsAsk = false;

                    if (response == "n")
                    {
                        this.RefusedProps.Add(prop);
                        break;
                    }
                    else
                    {
                        aprovedProps++;
                        this.AprovedProps.Add(prop);
                    }

                }
                if(aprovedProps == obj.Props.Count())
                {
                    foundObject = true;
                    Console.WriteLine("El objeto buscado es: " + obj.Value + "\nPresione ENTER para Continuar...");
                    Console.ReadLine().ToLower();
                }                    
            }

            if (!foundObject)
            {
                Console.WriteLine("No se encontro el objeto  \nPresione ENTER para Continuar...");
                response = Console.ReadLine().ToLower();
            }
            return 1;
        }

        public string ShowProcess(BCObject obj, string prop)
        {
            string response = "s";

            Console.WriteLine("Se asume que el resultado es " + obj.Value);
            if(this.AprovedProps.Count() > 0)
            {
                Console.WriteLine("Porque es : ");
                foreach(string aprovedProp in this.AprovedProps)
                {
                    Console.WriteLine(" - " + aprovedProp);
                }
            }
            if(this.RefusedProps.Count() > 0)
            {
                Console.WriteLine("Y porque NO es : ");
                foreach (string refusedProp in this.RefusedProps)
                {
                    Console.WriteLine(" - " + refusedProp);
                }
            }

            Console.WriteLine("\nEs " + prop + "?");
            Console.WriteLine("Ingresa 's' si tu respuesta es si o 'n' para no.");
            response = Console.ReadLine().ToLower();

            return response;
        }
    }
}
