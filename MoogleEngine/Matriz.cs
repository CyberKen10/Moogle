using System.Text;
using System.IO;
using System.Linq;

namespace MoogleEngine;

using System;

class Matriz
{
    private int tamaño;
    private double[,] datos;

    public Matriz(int n)
    {
        tamaño = n;
        datos = new double[n, n];
    }

    public Matriz(double[,] datos)
    {
        tamaño = datos.GetLength(0);
        this.datos = datos;
    }

    public void Llenar(double valor)
    {
        for (int i = 0; i < tamaño; i++)
        {
            for (int j = 0; j < tamaño; j++)
            {
                datos[i, j] = valor;
            }
        }
    }

    public Matriz Sumar(Matriz otra)
    {
        if (tamaño != otra.tamaño)
        {
            throw new ArgumentException("Las matrices deben tener el mismo tamaño.");
        }

        Matriz resultado = new Matriz(tamaño);

        for (int i = 0; i < tamaño; i++)
        {
            for (int j = 0; j < tamaño; j++)
            {
                resultado.datos[i, j] = datos[i, j] + otra.datos[i, j];
            }
        }

        return resultado;
    }

    public Matriz Restar(Matriz otra)
    {
        if (tamaño != otra.tamaño)
        {
            throw new ArgumentException("Las matrices deben tener el mismo tamaño.");
        }

        Matriz resultado = new Matriz(tamaño);

        for (int i = 0; i < tamaño; i++)
        {
            for (int j = 0; j < tamaño; j++)
            {
                resultado.datos[i, j] = datos[i, j] - otra.datos[i, j];
            }
        }

        return resultado;
    }

    public Matriz Multiplicar(Matriz otra)
    {
        if (tamaño != otra.tamaño)
        {
            throw new ArgumentException("Las matrices deben tener el mismo tamaño.");
        }

        Matriz resultado = new Matriz(tamaño);
        
        for (int i = 0; i < tamaño; i++)
        {
            for (int j = 0; j < tamaño; j++)
            {
                double sum = 0;
                
                for (int k = 0; k < tamaño; k++)
                {
                    sum += datos[i, k] * otra.datos[k, j];
                }
                
                resultado.datos[i, j] = sum;
            }
        }

        return resultado;
    }

    public Matriz Transponer()
    {
        Matriz resultado = new Matriz(tamaño);
        
        for (int i = 0; i < tamaño; i++)
        {
            for (int j = 0; j < tamaño; j++)
            {
                resultado.datos[i, j] = datos[j, i];
            }
        }

        return resultado;
    }

    public double Determinante()
    {
        double det = 0;
        
        if (tamaño == 1)
        {
            det = datos[0, 0];
        }
        else if (tamaño == 2)
        {
            det = datos[0, 0] * datos[1, 1] - datos[0, 1] * datos[1, 0];
        }
        else
        {
                    for (int j = 0; j < tamaño; j++)
        {
            Matriz submatriz = Submatriz(0, j);
            det += Math.Pow(-1, j) * datos[0, j] * submatriz.Determinante();
        }
    }

    return det;
}

private Matriz Submatriz(int fila, int columna)
{
    Matriz resultado = new Matriz(tamaño - 1);
    
    int i2 = 0;
    for (int i = 0; i < tamaño; i++)
    {
        if (i == fila) continue;

        int j2 = 0;
        for (int j = 0; j < tamaño; j++)
        {
            if (j == columna) continue;

            resultado.datos[i2, j2] = datos[i, j];
            j2++;
        }
        i2++;
    }

    return resultado;
}
}