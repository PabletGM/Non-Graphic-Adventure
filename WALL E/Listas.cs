using System;

namespace WallE
{
	// listas enlazadas de ENTEROS (fácilmente adaptable a cualquier otro tipo)
	class ListaEnlazada
	{

		// CLASE NODO (clase privada para los nodos de la lista)
		private class Nodo
		{
			public int dato;   // información del nodo (podría ser de cualquier tipo)
			public Nodo sig;   // referencia al siguiente

			// la constructora por defecto sería:
			// public Nodo() {} // por defecto

			// implementamos nuestra propia constructora para nodos
			public Nodo(int _dato = 0, Nodo _sig = null)
			{  // valores por defecto dato=0; y sig=null
				dato = _dato;
				sig = _sig;
			}
		}
		//he añadido otra clase para que la lista lea elementos string
		
		
		// FIN CLASE NODO

		// CAMPOS Y MÉTODOS DE LA CLASE Lista

		// campo pri: referencia al primer nodo de la lista
		Nodo pri;
		

		// constructora de la clase Lista
		public ListaEnlazada()
		{
			pri = null;   //  lista vacia
			
		}


		// insertar elemento e al principio de la lista
		public void InsertaPpio(int e)
		{
			//dices que la clase aux tenga como campo de dato el nuevo elemento de la lista leido por consola y como avanzar sig = null
			Nodo aux = new Nodo(e, pri);
			//creamos esa clase nodo a la variable pri
			pri = aux;
		}
		
		


		// añadir elto e al final de la lista
		public void InsertaFinal(int e)
		{
			// distinguimos dos casos

			// lista vacia
			if (pri == null)
			{
				//dices que pri tenga como campo de dato el nuevo elemento de la lista leido por consola y como avanzar sig = null
				pri = new Nodo(e, null); // creamos nodo en pri

				// lista no vacia				
			}
			else
			{
				Nodo aux = pri;   // recorremos la lista hasta el ultimo nodo



				//hasta que no llegue a el ultimo nodo (el cual tendrá aux.sig==null)
				while (aux.sig != null) aux = aux.sig;
				// aux apunta al último nodo
				aux.sig = new Nodo(e, null); // creamos el nuevo a continuación, osea que el contador o campo sig va a ser el siguiente nodo
			}
		}



		// buscar elto e
		public bool BuscaDato(int e)
		{
			Nodo aux = pri; // referencia al primero para buscar de ppio a fin
							// búsqueda de nodo con elto y se avanza a siguiente posicion o nodo hasta que aux.dato==e
			while (aux != null && aux.dato != e) aux = aux.sig;

			// termina con aux==null (elto no encontrado)
			// o bien con aux apuntando al primer nodo con elto e
			return aux != null;
		}


		// Conversion a string
		// método ToString que se invoca implícitamente cuando se hace Console.Write
		public override string ToString()
		{
			string salida = "\nLista: ";
			Nodo aux = pri;
			while (aux != null)
			{
				salida += aux.dato + " ";
				aux = aux.sig;
			}
			salida += "\n\n";
			return salida;
		}

		// elimina elto e (la primera aparición) de la lista, si está, y devuelve true
		// no hace nada en otro caso y devuelve false
		public bool EliminaElto(int e)
		{
			if (pri == null) return false; // si la lista es vacia no hay nada que eliminar
			else
			{
				// si es el primero puenteamos pri al siguiente
				if (e == pri.dato)
				{
					pri = pri.sig;
					return true;
				}
				else
				{ // eliminar otro distinto al primero				
				  // busqueda desde el ppio de la lista con pri
					Nodo aux = pri;


					//no entiendo

					// recorremos lista buscando el ANTERIOR al que hay que eliminar (para poder luego enlazar)
					while (aux.sig != null && e != aux.sig.dato)
						//así lo pasas al siguiente nodo y vuelta a buscar
						aux = aux.sig;
					// si lo encontramos
					if (aux.sig != null)
					{
						aux.sig = aux.sig.sig; // puenteamos al siguiente
						return true;
					}
					else return false;
				}
			}
		}

		// devuelve el num de eltos de la lista
		public int NumElems()
		{
			int n = 0;
			Nodo aux = pri;
			while (aux != null)
			{
				aux = aux.sig;
				n++;
			}
			return n;
		}


		private Nodo NesimoNodo(int n)
		{
			//si no existe el elemento  o dato en la lista , es decir ==-1
			if (n < 0) throw new Exception($"Error: no existe {n}-esimo en la lista");
			//si existe
			else
			{
				//referencia al primer nodo
				Nodo aux = pri;
				//mientras exista el nodo y el elemento  a buscar >0
				while (aux != null && n > 0)
				{
					//se pasa al siguiente nodo
					aux = aux.sig;
					//del 3 va al nodo 2 , del 2 al 1...
					n--;
				}
				//
				if (aux == null) throw new Exception($"Error: no existe {n}-esimo en la lista");
				else return aux;
			}
		}

		public int Nesimo(int n)
		{
			try
            {
				//devuelve el dato del nodo(que devuelve el metodo NesimoNodo)
				return NesimoNodo(n).dato;
			}
			catch
			{
				throw new Exception($"Error: no existe {n}-ésimo de la lista");
			}
		}

		public void InsertaNesimo(int pos, int e)
		{
			//si la posicion de la lista es 0 , es que inicia la lista
			if (pos == 0) pri = new Nodo(e, pri);
			//la lista ya existe y la expande
			else
			{
				try
				{
					Nodo aux = NesimoNodo(pos - 1);
					aux.sig = new Nodo(e, aux.sig);
				}
				catch
				{
					throw new Exception($"Error: no existe esa posicion en la lista");
				}
			}
		}
	    
	}

}