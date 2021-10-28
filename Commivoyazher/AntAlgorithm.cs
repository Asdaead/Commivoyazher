using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Commivoyazher
{
    class AntAlgorithm
    {
        const int MaxTime = 10;
        const int VisitMax = 3;
        const int Alpha = 1;
        const int Beta = 1;
        const double Rho = 0.5;
        const int citiesCount = 6;
        Random random = new Random();

        const int startTown = 1;
        const int finishTown = 2;
        double[,] PheromoneLevel = new double[citiesCount, citiesCount] {
            {0.0, 0.5, 0.5, 0.0, 0.0, 0.0},
            {0.5, 0.0, 0.0, 0.0, 0.5, 0.5},
            {0.5, 0.0, 0.0, 0.5, 0.5, 0.0},
            {0.0, 0.0, 0.5, 0.0, 0.5, 0.5},
            {0.0, 0.5, 0.5, 0.5, 0.0, 0.0},
            {0.0, 0.5, 0.0, 0.5, 0.0, 0.0}
        };
        //А Б В Г Д Е
        static double[,] Map = new double[citiesCount, citiesCount] {
            {0, 7.0, 3.0, 0, 0, 0},
            {7.0, 0, 0, 0, 4.0, 7.0},
            {3.0, 0, 0, 2.0, 5.0, 0},
            {0, 0, 2.0, 0, 3.0, 3.0},
            {0, 4.0, 5.0, 3.0, 0, 0},
            {0, 7.0, 0, 3.0, 0, 0}
        };
        static int[,] routeUsing = new int[citiesCount, citiesCount] {
            {0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0}
        };
        static Ant ant = new Ant();
        public void Start()
        {
            Console.WriteLine("Начало работы алгоритма");
            
            for (var timer = 0; timer <= MaxTime; timer++)
            {
                ant.CurrentTown = startTown;
                TravelAnt(finishTown);
                TravelAnt(startTown);
            }
            Console.WriteLine("Алгоритм завершен");
        }
        private void TravelAnt(int destination)
        {
            do
            {
                var startTown = ant.CurrentTown;
                var endTown = GetDestination(startTown, ant);
                if (endTown != startTown)
                {
                    routeUsing[startTown, endTown] = 1;
                    ant.TownVisits[endTown] += 1;
                    ant.CurrentTown = endTown;
                }
            } while (ant.CurrentTown != destination);
            UpdatePheromones();
        }
        private void UpdatePheromones()
        {
            for (var i = 0; i < citiesCount; i++)
            {
                for (var j = 0; j < citiesCount; j++)
                {
                    if (PheromoneLevel[i, j] > 0)
                    {
                        var addition = routeUsing[i, j] == 1
                            ? 1.0 / Map[i, j]
                            : 0.0;
                        PheromoneLevel[i, j] = (1 - Rho) * PheromoneLevel[i, j] + addition;
                        PheromoneLevel[j, i] = (1 - Rho) * PheromoneLevel[i, j] + addition;
                    }
                    routeUsing[i, j] = 0;
                }
            }
        }
        private int GetDestination(int cityStart, Ant ant)
        {
            var variants = new List<Variant>();
            var sumPropability = 0.0;
            for (var i = 0; i < citiesCount; i++)
            {
                if (Map[cityStart, i] != 0)
                {
                    var propability = Math.Pow(PheromoneLevel[cityStart, i], Alpha) * Math.Pow(1 / Map[cityStart, i], Beta);
                    sumPropability += propability;
                    variants.Add(new Variant
                    {
                        CityIndex = i,
                        Propability = propability
                    }
                    );
                }
            }
            
            foreach (var variant in variants)
            {
                variant.Propability /= sumPropability;
            }
            var chance = random.NextDouble();
            var lowerLine = 0.0;
            foreach (var variant in variants)
            {
                variant.LowerChanceLine = lowerLine;
                var higherLine = lowerLine + variant.Propability;
                variant.HigherChanceLine = higherLine;
                lowerLine = higherLine;
            }
            variants = variants.Where(x => 
            ant.TownVisits[x.CityIndex] <= VisitMax && 
            x.LowerChanceLine <= chance && 
            x.HigherChanceLine >= chance)
                .ToList();
            return variants.First().CityIndex;
        }
        public class Ant
        {
            public int CurrentTown { get; set; }
            public int[] TownVisits { get; set; } = { 0, 0, 0, 0, 0, 0 };
        }
        public class Variant
        {
            public int CityIndex { get; set; }
            public double Propability { get; set; }
            public double LowerChanceLine { get; set; }
            public double HigherChanceLine { get; set; }
        }
    }
}