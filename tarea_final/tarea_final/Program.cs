using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("****     Bienvenido al restaurante El Sazón del Barrio     *****");
        int total_pedidos = 5;
        Random rnd = new Random();
        var cts = new CancellationTokenSource();
        List<Task> tareas = new List<Task>();
        bool[] pedidosCancelados = new bool[total_pedidos];

        for (int i = 0; i < total_pedidos; i++)
        {
            int num_pedido = i;
            var tarea = Task.Run(async () =>
            {
                string cliente = $"Cliente {num_pedido}";
                Console.WriteLine($"{cliente} hizo un pedido");
                Console.WriteLine("validando ingredientes");

                await Task.Delay(1000);
                if (rnd.NextDouble() < 0.2)
                {
                    pedidosCancelados[num_pedido] = true;
                    Console.WriteLine($"{cliente} se fue, no habia ingredientes para su pedido");
                    return;
                }

                Console.WriteLine($"{cliente} su orden fue aceptada y se esta cocinando");

                int tiempo_cocinar = rnd.Next(3000, 7000);
                await Task.Delay(tiempo_cocinar);

                if (rnd.NextDouble() < 0.1)
                {
                    pedidosCancelados[num_pedido] = true;
                    Console.WriteLine($"El chef quemo el pedido de {cliente}, se cancela este pedido");
                    return;
                }

                Console.WriteLine($"{cliente} su comida esta lista despues de {tiempo_cocinar}");
            }, cts.Token);
            tareas.Add(tarea);
        }

        await Task.WhenAny(tareas);
        Console.WriteLine("\nEl primer pedido ha sido entregado!");

        try
        {
            await Task.WhenAll(tareas);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\n******* Fin de pedidos *******");
        }

        for (int i = 0; i < total_pedidos; i++)
        {
            if (pedidosCancelados[i])
            {
                Console.WriteLine($"El pedido {i} no se completo");
            }
        }
    }
}
