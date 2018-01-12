using System;
using Microsoft.Extensions.DependencyInjection;

namespace dotnet_core_di
{
    class Program
    {
        static void Main(string[] args)
        {
            //var bdi = new BasicDiExample();
            //bdi.Run();
            var us = new UsingFactory();
            us.Run("Testing testing 123");
        }

        public class UsingFactory
        {
            public void Run(string toPrint)
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddTransient<Transient>();
                serviceCollection.AddTransient<ClassWithDependencies>(sp =>
                {
                    var t = sp.GetRequiredService<Transient>();
                    return new ClassWithDependencies(t, toPrint);
                });

                var serviceProvider = serviceCollection.BuildServiceProvider();
                var classWithDependencies = serviceProvider.GetRequiredService<ClassWithDependencies>();
                Console.Out.WriteLine(classWithDependencies.StringToPrint);
            }

        }

        public class BasicDiExample
        {
            public void Run()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddSingleton<ISingleton, Singleton>();
                serviceCollection.AddScoped<Scoped>();
                serviceCollection.AddTransient<Transient>();

                var serviceProvider = serviceCollection.BuildServiceProvider();
                var singleton = serviceProvider.GetRequiredService<ISingleton>();
                var singleton2 = serviceProvider.GetRequiredService<ISingleton>();
                Console.Out.WriteLine($"Singleton's equal = {object.ReferenceEquals(singleton, singleton2)}.");
                var transient1 = serviceProvider.GetRequiredService<Transient>();
                var transient2 = serviceProvider.GetRequiredService<Transient>();
                Console.Out.WriteLine($"Transient1.Id={transient1.Id}, Transient2.Id={transient2.Id}");
                using (var scope1 = serviceProvider.CreateScope())
                {
                    Console.Out.WriteLine("In Scope1......");
                    var scoped1 = scope1.ServiceProvider.GetRequiredService<Scoped>();
                    var scoped2 = scope1.ServiceProvider.GetRequiredService<Scoped>();
                    Console.Out.WriteLine($"Scoped1.Id={scoped1.Id}, Scoped2.Id={scoped2.Id}");
                }
                using (var scope2 = serviceProvider.CreateScope())
                {
                    Console.Out.WriteLine("In Scope2.....");
                    var scoped1 = scope2.ServiceProvider.GetRequiredService<Scoped>();
                    var scoped2 = scope2.ServiceProvider.GetRequiredService<Scoped>();
                    Console.Out.WriteLine($"Scoped1.Id={scoped1.Id}, Scoped2.Id={scoped2.Id}");
                }
            }
        }

        public interface ISingleton { }
        public class Singleton : ISingleton
        {
            private static int _instanceCount = 0;
            public Singleton()
            {
                _instanceCount++;
            }
        }
        public class Scoped
        {
            public int Id { get; set; }
            public Scoped()
            {
                Id = new Random().Next();
            }
        }

        public class Transient
        {
            public int Id { get; set; }
            public Transient()
            {
                Id = new Random().Next();
            }
        }

        public class ClassWithDependencies
        {
            public string StringToPrint { get; private set; }
            public ClassWithDependencies(Transient t, string stringToPrint = "")
            {
                StringToPrint = $"{stringToPrint}, Transient.Id={t.Id}";
            }
        }

    }
}
