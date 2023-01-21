using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace WallE
{
    class Program
    {
        //constantes
        const int dimPlaces = 100;
        const int dimItems = 50;
        static void Main(string[] args)
        {
            string file;
            //Inicializamos las clase Mapa y WallE
            Map mapa = new Map(dimPlaces, dimItems);
            WallE wallE = new WallE();
            //Leemos el mapa y lo asociamos a sus respectivas variables
            mapa.ReadMap(out file);

            string com = "";
            //Bucle principal
            while (wallE.juego)
            {   //Leer comando y procesarlo
                Console.Write(">");
                com = Console.ReadLine();
                WallE.ProcesaInput(com, wallE, mapa);
            }
        }
    }
    public enum Direction { North, South, East, West };
    class Map
    {
        const int dimPlaces = 100;
        const int dimItems = 50;
        const int dimConexiones = 4;
        // items basura
        struct Item
        {
            public string name, description;
        }

        // lugares del mapa
        struct Place
        {
            public string name, description;
            public bool spaceShip;
            public int[] connections;
            public ListaEnlazada itemsInPlace;
        }

        Place[] places;
        Item[] items;
        int nPlaces, nItems;

        //constructora de la clase genera un mapa vacio con el numero de lugares e items indicados
        public Map(int numPlaces = dimPlaces, int numItems = dimItems)
        {
            //inicializamos numero total de lugares e items a las constantes
            places = new Place[numPlaces];
            items = new Item[numItems];
        }

        #region ReadMap
        //Leer linea a linea , cuando encuentra palabra street , place o garbage llama al metodo determinado y hasta que acabe
        public void ReadMap(out string file)
        {
            file = "madrid.map";
            StreamReader f = new StreamReader(file);
            //array de strings con las palabras, inicializamos variables
            string[] s;
            string linea, name, descripcion = "";
            int numeroLugar = 0;
            bool spaceShip;
            //condiciones de busqueda, mientras queden lineas por mirar
            while (!f.EndOfStream)
            {
                //Almacenamos en string linea , una linea de madrid.map
                linea = f.ReadLine();
                //divide en s cada palabra de la linea
                s = linea.Split(' ');

                if (s[0] == "place")
                {
                    //asociamos el nombre del struct places a la posicion del name
                    name = s[2];
                    //segun si lee "noSpaceShip" o "SpaceShip" pondrá el bool a true o false
                    spaceShip = (s[3] == "spaceShip");
                    //leer descripcion y la asocias a la variable description de el lugar
                    descripcion = ReadDescription(f);
                    //ponemos toda la informacion del lugar devolviendo un lugar con la informacion ya actualizada
                    places[nPlaces] = CreatePlace(name, descripcion, spaceShip);
                    //se añade un lugar encontrado
                    nPlaces++;
                }

                else if (s[0] == "street")
                {
                    int numeroLugarConexion;
                    //almacenamos el numero del lugar y su conexion
                    numeroLugar = int.Parse(s[1]);
                    numeroLugarConexion = int.Parse(s[3]);
                    //le asignas una direccion inicial en string
                    string dir = s[2];
                    //direccion que convierte en string en enum
                    Direction direction;
                    Enum.TryParse(dir, true, out direction);
                    //mete la direccion y los parametros
                    CreateStreet(numeroLugar, numeroLugarConexion, direction);
                }

                else if (s[0] == "garbage")
                {
                    //colocamos el nombre del item en el lugar 2
                    numeroLugar = int.Parse(s[2]);
                    //añadimos el numero del lugar a la lista
                    places[numeroLugar].itemsInPlace.InsertaFinal(nItems);
                    //guardamos el nombre del item y añadimos un espacio a la lista itemsInPlace
                    name = s[3];
                    descripcion = '"' + linea.Split('"')[1] + '"';
                    //Creamos nuevo item
                    items[nItems] = CreateItem(name, descripcion);
                    nItems++;
                }
            }
        }
        //Devuelve la descripción en diferentes líneas
        private string ReadDescription(StreamReader f)
        {
            string saltoLinea = "\n";
            //almacenamos la linea leida en madrid.map
            string linea = f.ReadLine() + saltoLinea;
            string lineaNueva = " ";
            //mientras que la ultima linea no sea vacia(significa que has acabado la descripcion)
            while (lineaNueva != saltoLinea)
            {
                //se almacena la linea nueva
                lineaNueva = f.ReadLine() + saltoLinea;
                //se añade en string linea
                linea += lineaNueva;
            }
            return linea;
        }

        //cuando encuentra la palabra place, le asocia cada campo del struct place
        private Place CreatePlace(string name, string descripcion, bool spaceship)
        {
            Place Lugar;
            Lugar.itemsInPlace = new ListaEnlazada();
            Lugar.name = name;
            Lugar.description = descripcion;
            Lugar.spaceShip = spaceship;
            // inicializamos las conexiones en 4(norte, sur, este y oeste) y las ponemos nulas
            Lugar.connections = new int[dimConexiones] { -1, -1, -1, -1 };

            return Lugar;
        }

        //cuando encuentra la palabra street, asociamos las conexiones entre los lugares
        private void CreateStreet(int numeroLugar, int numeroLugarConexion, Direction direction)
        {
            places[numeroLugar].connections[(int)direction] = numeroLugarConexion;
            Direction dircontraria = DirContraria(direction);
            //se crea la conexion entre las mismas calles pero con dirección opuesta
            places[numeroLugarConexion].connections[(int)dircontraria] = numeroLugar;
        }

        //cuando encuentra la palabra garbage, le asocia cada campo del struct item
        private Item CreateItem(string name, string descripcion)
        {
            //creamos una instancia de item que igualaremos a el item de la posicion que sea
            Item item;
            item.name = name;
            item.description = descripcion;

            return item;
        }
        //Devuelve la dirección contraria
        public Direction DirContraria(Direction direction)
        {
            if (direction == Direction.North)
            {
                direction = Direction.South;
            }
            else if (direction == Direction.South)
            {
                direction = Direction.North;
            }
            else if (direction == Direction.East)
            {
                direction = Direction.West;
            }
            else if (direction == Direction.West)
            {
                direction = Direction.East;
            }

            return direction;
        }
        #endregion
        //devuelve toda la información del lugar indicado 
        public string GetPlaceInfo(int pl)
        {
            //almacenamos en una linea su descripcion
            string linea = places[pl].description;

            return linea;
        }

        //devuelve los movimientos posibles desde el lugar pl en un string
        public string GetMoves(int pl)
        {
            string LugarConexion = "";

            for (int i = 0; i < dimConexiones; i++)
            {
                //hemos encontrado una direccion valida
                switch (i)
                { //si existe una conexion almacenamos en un string dicha dirección y el nombre de la conexión
                    case 0:

                        if (places[pl].connections[0] != -1)
                        {
                            LugarConexion += "north: " + places[places[pl].connections[0]].name + '\n';
                        }
                        break;

                    case 1:
                        if (places[pl].connections[1] != -1)
                        {
                            LugarConexion += "south: " + places[places[pl].connections[1]].name + '\n';
                        }
                        break;

                    case 2:
                        if (places[pl].connections[2] != -1)
                        {
                            LugarConexion += "east: " + places[places[pl].connections[2]].name + '\n';
                        }
                        break;

                    case 3:
                        if (places[pl].connections[3] != -1)
                        {
                            LugarConexion += "west: " + places[places[pl].connections[3]].name + '\n';
                        }
                        break;
                }
            }

            return LugarConexion;
        }

        //devuelve la información sobre los ítems que hay en el lugar pl
        public string GetItemsPlace(int pl)
        {
            string itemsLugar = "";
            //accedes a metodo de Listas que te dice el numero de elementos
            int n = places[pl].itemsInPlace.NumElems();

            //vamos contando en el numero del lugar pl específico el numero de items n y su numero
            for (int i = 0; i < n; i++)
            {
                //devuelve numero de la lista 
                int k = places[pl].itemsInPlace.Nesimo(i);
                string name = items[k].name;
                string description = items[k].description;

                itemsLugar += k + " " + name + " " + description + '\n';
            }
            return itemsLugar;
        }
        #region metodos propios
        //Devuelve un booleano, que será true si encuentra el objeto en la lista de items del lugar
        public bool ExisteItem(Map m, int pl, int it)
        {
            bool DiferenteNull = m.places[pl].itemsInPlace.BuscaDato(it);

            return DiferenteNull;
        }
        #endregion
        //devuelve el nombre y la descripcion de un item concreto indicando su posicion del lugar y su numero de posicion en la lista
        public string ItemInfo(int it)
        {
            string info = " ";
            info += " " + items[it].name + " " + items[it].description;

            return info;
        }

        //elimina el ítem it del lugar pl.
        public void PickItemPlace(int pl, int it)
        {
            places[pl].itemsInPlace.EliminaElto(it);
        }

        //deja el ítem it en el lugar pl.
        public void DropItemPlace(int pl, int it)
        {
            places[pl].itemsInPlace.InsertaFinal(it);
        }

        //devuelve el lugar al que se llega desde el lugar pl avanzando en la dirección dir
        public int Move(int pl, Direction dir)
        {
            int LugarMove = -1;
            for (int i = 0; i < dimConexiones; i++)
            {
                switch (i)
                {
                    case 0:
                        if (dir == Direction.North)
                        {
                            LugarMove = places[pl].connections[0];
                        }
                        break;

                    case 1:
                        if (dir == Direction.South)
                        {
                            LugarMove = places[pl].connections[1];
                        }
                        break;

                    case 2:
                        if (dir == Direction.East)
                        {
                            LugarMove = places[pl].connections[2];
                        }
                        break;

                    case 3:
                        if (dir == Direction.West)
                        {
                            LugarMove = places[pl].connections[3];
                        }
                        break;
                }
            }

            if (LugarMove != -1)
            {
                return LugarMove;
            }
            else
            {
                return -1;
            }
        }

        //comprueba si el lugar pl es la nave de WALL·E.
        public bool isSpaceShip(int pl)
        {
            bool spaceship;
            spaceship = places[pl].spaceShip;

            return spaceship;
        }
    }

    class WallE
    {
        int pos;
        ListaEnlazada bag;
        public bool juego = true;
        //Método constructor
        public WallE()
        {   //Inicializamos las variables
            pos = 0;
            bag = new ListaEnlazada();
        }
        //Devuelve la posición de Walle
        public int GetPosition()
        {
            return pos;
        }
        //Mueve al walle en la dirección dada a partir del mapa y de la posicion actual
        public void Move(Map m, Direction dir)
        {
            pos = m.Move(pos, dir);

            if (pos == -1)
            {
                //Si no existe la conexion, excepcion
                throw new Exception("No existe una conexión con este lugar");
            }
        }

        //Recoge el item it, lo almacena en el inventario y elimina ese mismo item del mapa
        public void PickItem(Map m, int it)
        {
            m.PickItemPlace(pos, it);
            bag.InsertaFinal(it);
        }

        //Deja el item it, lo almacena en la lista del mapa y elimina ese mismo item de la lista inventario
        public void DropItem(Map m, int it)
        {
            m.DropItemPlace(pos, it);
            bag.EliminaElto(it);
        }

        //Devuelve los items que tienes en el inventario
        public string Bag(Map m)
        {
            string bagString = "";
            int n = bag.NumElems();

            //vamos contando en el numero del lugar pl específico el numero de items n y su numero
            for (int i = 0; i < n; i++)
            {
                bagString += m.ItemInfo(i) + "\n";
            }

            return bagString;
        }
        //Indica si la nave esta en el lugar
        public bool AtSpaceShip(Map map)
        {
            bool Spaceship = map.isSpaceShip(pos);

            return Spaceship;
        }

        //Procesamos cada orden
        public static void ProcesaInput(string com, WallE w, Map m)
        {
            int it = 0;
            switch (com)
            {
                //Ir a una dirección concreta, comprobar si isSpaceShip=true
                //y devuelve los items que tenemos en la mochila
                case "go north":
                    w.Move(m, Direction.North);

                    if (w.AtSpaceShip(m))
                    {
                        Console.Clear();
                        Console.WriteLine("Wall-E ha llegado a la nave");
                        Console.Write(w.Bag(m));
                        w.juego = false;
                    }
                    break;

                case "go south":
                    w.Move(m, Direction.South);
                    if (w.AtSpaceShip(m))
                    {
                        Console.Clear();
                        Console.WriteLine("Wall-E ha llegado a la nave");
                        Console.Write(w.Bag(m));
                        w.juego = false;
                    }
                    break;

                case "go east":
                    w.Move(m, Direction.East);
                    if (w.AtSpaceShip(m))
                    {
                        Console.Clear();
                        Console.WriteLine("Wall-E ha llegado a la nave");
                        Console.Write(w.Bag(m));
                        w.juego = false;
                    }
                    break;

                case "go west":
                    w.Move(m, Direction.West);
                    if (w.AtSpaceShip(m))
                    {
                        Console.Clear();
                        Console.WriteLine("Wall-E ha llegado a la nave");
                        Console.Write(w.Bag(m));
                        w.juego = false;
                    }
                    break;

                case "info":
                    Console.Write(m.GetPlaceInfo(w.pos));
                    break;

                case "items":
                    Console.Write(m.GetItemsPlace(w.pos));
                    break;

                //Salir del juego
                case "quit":
                    w.juego = false;
                    break;

                case "bag":
                    string bag = w.Bag(m);
                    Console.WriteLine(bag);
                    break;
            }
            //Dividimos el string para quedarnos con el número del item
            string[] s = com.Split();

            if (s.Length > 1 && s[0] != "go")
            {
                string pick = "pick " + s[1];
                string drop = "drop" + s[1];
                it = Convert.ToInt32(s[1]);

                //Condición de búsqueda de pick y drop
                if (com == pick)
                {
                    w.PickItem(m, it);
                    //comprobacion de si en ese sitio hay item
                    //if(m.ExisteItem(m, w.pos, it)) 
                    {
                        Console.WriteLine("Item " + it + " recogido");
                    }
                    //else
                    {
                        throw new Exception("El objeto no existe en este lugar");
                    }
                }

                else if (com == drop)
                {
                    w.DropItem(m, it);
                    //Comprobación de si en la bolsa tenemos ese item
                    //if(m.ExisteItem(m, w.pos, it)) NO ES EL MISMO METODO, HABRIA QUE COMPROBAR SI ESTA EN LA LISTA BOLSA
                    {
                        Console.WriteLine("Item " + it + " devuelto");
                    }
                    //else
                    {
                        throw new Exception("No tienes este item en tu bolsa");
                    }
                }
            }
        }
    }
}