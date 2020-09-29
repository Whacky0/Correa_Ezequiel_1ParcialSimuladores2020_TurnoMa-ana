using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nivel2 : MonoBehaviour
{
    public Texture2D mapa;
    public ColorAPrefab[] colorMappings;
    Tablero tablero;
    int cantidadX;
    int cantidadY;
    void Start()
    {
        GenerarNivel();
        cantidadX = tablero.casilleros.GetLength(0);
        cantidadY = tablero.casilleros.GetLength(1);
    }
    void GenerarNivel()
    {
        for (int x = 0; x < cantidadX; x++)
        {
            for (int y = 0; y < cantidadY; y++)
            {
                GenerarTile(x, y);
            }
        }
    }

    void GenerarTile(int x, int y)
    {
        Color pixelColor = mapa.GetPixel(x, y);
        if (pixelColor.a == 0)
        {
            return;
        }
        foreach (ColorAPrefab colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(pixelColor))
            {
                Vector2 position = new Vector2(x, y);
                Instantiate(colorMapping.prefab, position, Quaternion.identity, transform);
            }
        }

    }
}
