using System;
using System.Collections.Generic;
using System.Text;

namespace BuilderPatternLab
{
    // -----------------------------------------------------------
    // 1. PRODUCT (Продукт)
    // Складний об'єкт, який ми хочемо створити.
    // -----------------------------------------------------------
    public class Rocket
    {
        // Властивості ракети
        public string Name { get; set; }
        public List<string> Stages { get; set; } = new List<string>();
        public string EngineType { get; set; }
        public string Payload { get; set; }
        public double TotalWeight { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"--- Конфігурація Ракети: {Name} ---");
            sb.AppendLine($" Тип двигунів: {EngineType}");
            sb.AppendLine($" Корисний вантаж: {Payload}");
            sb.AppendLine($" Загальна вага: {TotalWeight} т");
            sb.AppendLine(" Ступені:");
            if (Stages.Count > 0)
            {
                for (int i = 0; i < Stages.Count; i++)
                {
                    sb.AppendLine($"   {i + 1}. {Stages[i]}");
                }
            }
            else
            {
                sb.AppendLine("   [Ступені відсутні]");
            }
            sb.AppendLine("-----------------------------------");
            return sb.ToString();
        }
    }

    // -----------------------------------------------------------
    // 2. BUILDER INTERFACE (Абстрактний будівельник)
    // Визначає методи для створення частин продукту.
    // -----------------------------------------------------------
    public interface IRocketBuilder
    {
        // Скидає об'єкт (починає нову збірку)
        void Reset();

        // Кроки будівництва
        void SetRocketName(string name);
        void AddStage(string stageDescription, double stageWeight);
        void InstallEngines();
        void LoadPayload(string payloadName, double payloadWeight);

        // Отримання результату
        Rocket GetRocket();
    }

    // -----------------------------------------------------------
    // 3. CONCRETE BUILDERS (Конкретні будівельники)
    // Реалізують кроки для різних варіацій ракет.
    // -----------------------------------------------------------

    /// <summary>
    /// Будівельник для важкої ракети (наприклад, для польоту на Місяць/Марс).
    /// Використовує рідинні двигуни та багато ступенів.
    /// </summary>
    public class HeavyLiftRocketBuilder : IRocketBuilder
    {
        private Rocket _rocket = new Rocket();

        public void Reset()
        {
            _rocket = new Rocket();
        }

        public void SetRocketName(string name)
        {
            _rocket.Name = name;
        }

        public void AddStage(string stageDescription, double stageWeight)
        {
            _rocket.Stages.Add(stageDescription);
            _rocket.TotalWeight += stageWeight;
        }

        public void InstallEngines()
        {
            // Специфіка цього будівельника - потужні рідинні двигуни
            _rocket.EngineType = "Рідинні двигуни високої тяги (Liquid Propellant)";
            _rocket.TotalWeight += 50; // Вага самих двигунів
        }

        public void LoadPayload(string payloadName, double payloadWeight)
        {
            _rocket.Payload = payloadName;
            _rocket.TotalWeight += payloadWeight;
        }

        public Rocket GetRocket()
        {
            Rocket result = _rocket;
            Reset(); // Підготовка до наступного будівництва
            return result;
        }
    }

    /// <summary>
    /// Будівельник для легкої ракети (наприклад, для виведення супутника на орбіту).
    /// Використовує твердопаливні прискорювачі.
    /// </summary>
    public class LightCargoRocketBuilder : IRocketBuilder
    {
        private Rocket _rocket = new Rocket();

        public void Reset()
        {
            _rocket = new Rocket();
        }

        public void SetRocketName(string name)
        {
            _rocket.Name = name;
        }

        public void AddStage(string stageDescription, double stageWeight)
        {
            // У легких ракет ступені менші
            _rocket.Stages.Add($"Light Stage: {stageDescription}");
            _rocket.TotalWeight += stageWeight;
        }

        public void InstallEngines()
        {
            _rocket.EngineType = "Твердопаливні прискорювачі (Solid Fuel)";
            _rocket.TotalWeight += 10;
        }

        public void LoadPayload(string payloadName, double payloadWeight)
        {
            // Перевірка на перевантаження (бізнес-логіка будівельника)
            if (payloadWeight > 500)
            {
                Console.WriteLine("Warning: Занадто важкий вантаж для легкої ракети!");
            }
            _rocket.Payload = payloadName;
            _rocket.TotalWeight += payloadWeight;
        }

        public Rocket GetRocket()
        {
            Rocket result = _rocket;
            Reset();
            return result;
        }
    }

    // -----------------------------------------------------------
    // 4. DIRECTOR (Директор / Центр управління польотами)
    // Керує порядком будівництва. Він знає "рецепти", але не деталі реалізації.
    // -----------------------------------------------------------
    public class MissionControlDirector
    {
        private IRocketBuilder _builder;

        // Директор може змінювати будівельника динамічно
        public void SetBuilder(IRocketBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// Рецепт створення ракети для місії на Марс (максимальна комплектація)
        /// </summary>
        public void BuildMarsMissionRocket()
        {
            if (_builder == null) throw new InvalidOperationException("Builder is not set");

            _builder.SetRocketName("Mars Explorer V");
            _builder.AddStage("Stage 1: Super Heavy Booster", 2000);
            _builder.AddStage("Stage 2: Interplanetary Transfer Stage", 500);
            _builder.AddStage("Stage 3: Landing Module", 100);
            _builder.InstallEngines();
            _builder.LoadPayload("Scientific Rover & Life Support", 50);
        }

        /// <summary>
        /// Рецепт створення ракети для запуску супутника зв'язку (мінімальна комплектація)
        /// </summary>
        public void BuildSatelliteRocket()
        {
            if (_builder == null) throw new InvalidOperationException("Builder is not set");

            _builder.SetRocketName("StarLink Carrier");
            _builder.AddStage("Main Booster", 300);
            _builder.AddStage("Orbital Insertion Stage", 50);
            _builder.InstallEngines();
            _builder.LoadPayload("Communication Satellite Array", 15);
        }
    }

    // -----------------------------------------------------------
    // 5. CLIENT (Клієнтський код)
    // -----------------------------------------------------------
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Створюємо Директора (Менеджер місії)
            var director = new MissionControlDirector();

            // Сценарій 1: Будуємо важку ракету для Марса
            Console.WriteLine("1. Готуємо місію на Марс...");
            var heavyBuilder = new HeavyLiftRocketBuilder();
            
            director.SetBuilder(heavyBuilder); // Призначаємо підрядника (HeavyBuilder)
            director.BuildMarsMissionRocket(); // Даємо команду будувати за рецептом "Марс"
            
            Rocket marsRocket = heavyBuilder.GetRocket(); // Забираємо готовий продукт
            Console.WriteLine(marsRocket.ToString());


            // Сценарій 2: Будуємо легку ракету для супутника
            Console.WriteLine("\n2. Готуємо запуск супутника...");
            var lightBuilder = new LightCargoRocketBuilder();

            director.SetBuilder(lightBuilder); // Змінюємо підрядника на LightBuilder
            director.BuildSatelliteRocket();   // Даємо команду будувати за рецептом "Супутник"

            Rocket satRocket = lightBuilder.GetRocket();
            Console.WriteLine(satRocket.ToString());


            // Сценарій 3: Кастомна збірка без Директора
            // Патерн Builder дозволяє клієнту будувати об'єкт вручну, якщо потрібна унікальна конфігурація
            Console.WriteLine("\n3. Експериментальна кастомна збірка...");
            heavyBuilder.Reset();
            heavyBuilder.SetRocketName("Test Prototype X");
            heavyBuilder.AddStage("Single Stage To Orbit (SSTO)", 1000);
            heavyBuilder.InstallEngines();
            // Вантаж не додаємо
            
            Rocket customRocket = heavyBuilder.GetRocket();
            Console.WriteLine(customRocket.ToString());

            Console.ReadLine();
        }
    }
}
