using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SokobanGameManager : MonoBehaviour
{
    GameObject objProximo, objProximoProximo;
    Nivel nivel, nivelAux;
    GameObject casillero, casilleroTarget, pared, jugador, bloque;
    List<Vector2> posOcupadasEsperadasCasillerosTarget;
    List<Vector2> posOcupadasBloques;
    int contador;
 
    float mostrarMensaje = 0;
    Tablero tablAux;


    Stack   pilaTablerosAnteriores= new Stack();

    Tablero tableroInstanciable;
    
 
    Vector2 posicionJugador;

    string orientacionJugador;
    string nombreNivelActual = "Nivel1";
    bool gameOver = false;
    bool estoyDeshaciendo = false;

    private void Start()
    {
       
        casillero = SokobanLevelManager.instancia.dameLstPrefabsSokoban().Find(x => x.name == "Casillero");
        casilleroTarget = SokobanLevelManager.instancia.dameLstPrefabsSokoban().Find(x => x.name == "CasilleroTarget");
        pared = SokobanLevelManager.instancia.dameLstPrefabsSokoban().Find(x => x.name == "Pared");
        jugador = SokobanLevelManager.instancia.dameLstPrefabsSokoban().Find(x => x.name == "Jugador");
        bloque = SokobanLevelManager.instancia.dameLstPrefabsSokoban().Find(x => x.name == "Bloque");
        CargarNivel(nombreNivelActual);
    }

    private void CargarNivel(string nombre)
    {
        nivel = SokobanLevelManager.instancia.dameNivel(nombre);
        posOcupadasEsperadasCasillerosTarget = nivel.Tablero.damePosicionesObjetos("CasilleroTarget");
        
        InstanciadorPrefabs.instancia.graficarCasilleros(nivel.Tablero, casillero);
        InstanciadorPrefabs.instancia.graficarCasillerosTarget(nivel.Tablero, casilleroTarget);
        InstanciadorPrefabs.instancia.graficarObjetosTablero(nivel.Tablero, SokobanLevelManager.instancia.dameLstPrefabsSokoban());
    }

    private void Update()
    {
        posOcupadasBloques = nivel.Tablero.damePosicionesObjetos("Bloque");

        
       
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            orientacionJugador = "derecha";
            mover();
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            orientacionJugador = "arriba";
            mover();
        }
       
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            orientacionJugador = "abajo";
            mover();
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            orientacionJugador = "izquierda";
            mover();
        }
        if (Input.GetKeyDown(KeyCode.Z) && contador>0)
        {

            estoyDeshaciendo = true;
            contador--;
            mover();
        }


    }

    private void mover()
    {
        
        if (estoyDeshaciendo == false)
        {
            tablAux = new Tablero(nivel.Tablero.casilleros.GetLength(0), nivel.Tablero.casilleros.GetLength(1));
            tablAux.setearObjetos(casillero, nivel.Tablero.damePosicionesObjetos("Casillero"));
            tablAux.setearObjetos(casilleroTarget, nivel.Tablero.damePosicionesObjetos("CasilleroTarget"));
            tablAux.setearObjetos(bloque, nivel.Tablero.damePosicionesObjetos("Bloque"));
            tablAux.setearObjetos(pared, nivel.Tablero.damePosicionesObjetos("Pared"));
            tablAux.setearObjetos(jugador, nivel.Tablero.damePosicionesObjetos("Jugador"));


            posicionJugador = new Vector2(nivel.Tablero.damePosicionObjeto("Jugador").x, nivel.Tablero.damePosicionObjeto("Jugador").y);
            
            objProximo = nivel.Tablero.dameObjeto(posicionJugador, orientacionJugador, 1);
            objProximoProximo = nivel.Tablero.dameObjeto(posicionJugador, orientacionJugador, 2);

            

            if (objProximo != null && objProximo.CompareTag("casillero"))
            {
                nivel.Tablero.setearObjeto(casillero, posicionJugador);
                nivel.Tablero.setearObjeto(jugador, posicionJugador, orientacionJugador, 1);
                guardarPosiciones();

            }
            else
            {
                
                if (objProximo != null && objProximo.CompareTag("bloque") && objProximoProximo != null  )
                {
                    //no mover mas de un bloque, chquear que la proxima casilla es un casillero vacio
                    if (objProximoProximo.CompareTag("casillero"))
                    {
                        nivel.Tablero.setearObjeto(jugador, posicionJugador, orientacionJugador, 1);
                        {
                            guardarPosiciones();
                            nivel.Tablero.setearObjeto(casillero, posicionJugador);
                            nivel.Tablero.setearObjeto(bloque, posicionJugador, orientacionJugador, 2); ;
                        }
                    }
                }
            }
            InstanciadorPrefabs.instancia.graficarObjetosTablero(nivel.Tablero, SokobanLevelManager.instancia.dameLstPrefabsSokoban());

            if (ChequearVictoria(nivel.Tablero) && mostrarMensaje==1)
            {
                Debug.Log("Gané");
               
            }
        }
        else 
        {
            //Deshacer los movimientos y guardar el nuevo estado del tablero
            Debug.Log("estoy volviendo para atras");
            Tablero tableroPila = tableroAux();
            InstanciadorPrefabs.instancia.graficarObjetosTablero(tableroPila, SokobanLevelManager.instancia.dameLstPrefabsSokoban());
            nivel.Tablero = tableroPila;
            estoyDeshaciendo = false;
            
        }
    }

    private bool SonIgualesLosVectores(Vector2 v1, Vector2 v2)
    {
        return (v1.x == v2.x && v1.y == v2.y);
    }

    private bool ChequearVictoria(Tablero tablero)
    {
    
        //Chequar las posiciones de los bloques y las casillas donde deben dejarse
      for(int i=0; i < posOcupadasEsperadasCasillerosTarget.Count; i++)
        {
            
                if (posOcupadasBloques[i].x == posOcupadasEsperadasCasillerosTarget[i].x && posOcupadasBloques[i].y == posOcupadasEsperadasCasillerosTarget[i].y && 
                i==posOcupadasEsperadasCasillerosTarget.Count-1)
            {
              
                mostrarMensaje ++;
                return true;
               
            }

        }
        return false;
    }
    
        void guardarPosiciones()
    {
        pilaTablerosAnteriores.Push(tablAux);
        contador++;
    }

    private Tablero tableroAux()
    {

        Tablero tablero = (Tablero) pilaTablerosAnteriores.Pop();

        return tablero;

    }
}

