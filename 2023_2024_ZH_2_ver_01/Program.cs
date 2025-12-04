using _2023_2024_ZH_2_ver_01;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace _2023_24_ZH_2_ver01 {
    internal class Program {
        static void Main(string[] args) {
            List<Meal> meals = new List<Meal>();

            StreamReader streamReader = new StreamReader("MP1_ZH2_2023_24_input.csv", encoding: Encoding.UTF8);
            streamReader.ReadLine();

            while (!streamReader.EndOfStream) {
                Meal meal = new Meal();
                string[] line = streamReader.ReadLine().Split(";");

                meal.Name = line[0];
                meal.Cathegory = line[1];
                meal.IsNormal = line[2] == "normál" ? true : false;
                meal.Mass = int.Parse(line[3]);
                meal.OrderDate = DateTime.Parse(line[4]);
                meal.Allergens = GetAllergens(line[5]);
                meal.Energy = int.Parse(line[6]);
                meal.Price = int.Parse(line[7]);

                meals.Add(meal);
            }

            streamReader.Close();

            // 3. feladat
            DecorateText("3. feladat");
            DecorateText("Fullos ételek a hét végére:", true, false, false);

            int energyLimit = 700;
            int sumPrice = 0;
            int countOfFullEnergyMeals = 0;

            foreach (Meal meal in meals) {
                int fullEnergy = F2_GetFullEnergy(meal);

                if (meal.OrderDate.Day > 13 && meal.OrderDate.Day < 16 && fullEnergy > energyLimit) {
                    //Console.WriteLine($"{meal.Name} ({GetPortion(meal.IsNormal)}: {meal.Mass}) - {fullEnergy} kcal, {meal.Price} Ft");
                    Console.WriteLine($"{meal.Name} ({(meal.IsNormal ? "normál" : "kicsi")}: {meal.Mass}) - {fullEnergy} kcal, {meal.Price} Ft");
                    sumPrice += meal.Price;
                    countOfFullEnergyMeals++;
                }
            }

            //Console.WriteLine($"A fenti ételek átlagos ára {sumPrice / countOfFullEnergyMeals} Ft.");
            // A Convert.ToInt32 rendesen kerekít felfele is.
            Console.WriteLine($"A fenti ételek átlagos ára {Convert.ToInt32((double)sumPrice / countOfFullEnergyMeals)} Ft.");

            // 4. feladat
            DecorateText("\n4. feladat");

            int littleFriedMealIndex = 0;
            int littleFriedMealPrice = int.MaxValue;
            int littleNormalVegetableIndex = 0;
            int littleNormalVegetablePrice = int.MaxValue;

            for (int i = 0; i < meals.Count; i++) {
                if (meals[i].Cathegory == "rántott" && !meals[i].IsNormal && littleFriedMealPrice > meals[i].Price) {
                    littleFriedMealPrice = meals[i].Price;
                    littleFriedMealIndex = i;
                }
                if (meals[i].Cathegory == "főzelék" && meals[i].IsNormal && littleNormalVegetablePrice > meals[i].Price) {
                    littleNormalVegetablePrice = meals[i].Price;
                    littleNormalVegetableIndex = i;
                }
            }

            Console.WriteLine($"A legolcsóbb kis adag rántott a(z) {meals[littleFriedMealIndex].Name}, az ára pedig {meals[littleFriedMealIndex].Price} Ft.");
            Console.WriteLine($"A legolcsóbb normal adag főzelék a(z) {meals[littleNormalVegetableIndex].Name}, az ára pedig {meals[littleNormalVegetableIndex].Price} Ft.");

            // ez nem jó, mert ha egyenlő az nincs vizsgálva!!!
            //string expensiveMeal = meals[littleFriedMealIndex].Price > meals[littleNormalVegetableIndex].Price ? meals[littleFriedMealIndex].Name : meals[littleNormalVegetableIndex].Name;
            //Console.WriteLine($"A(z) {expensiveMeal} drágább.");

            if (meals[littleFriedMealIndex].Price > meals[littleNormalVegetableIndex].Price)
                Console.WriteLine($"A(z) {meals[littleFriedMealIndex].Name} drágább.");
            else if (meals[littleFriedMealIndex].Price < meals[littleNormalVegetableIndex].Price)
                Console.WriteLine($"A(z) {meals[littleNormalVegetableIndex].Name} drágább.");
            else
                Console.WriteLine($"A(z) {meals[littleNormalVegetableIndex].Name} ugyanannyiba kerül, mint a {meals[littleFriedMealIndex].Name}.");


            // 6. feladat
            DecorateText("\n6. feladat");
            DecorateText("Adja meg az Ön számára veszélyes allergéneket:", true, false, false);

            List<string> allergensInputList = new List<string>();
            string allergenInput;

            do {
                allergenInput = Console.ReadLine();
                if (allergenInput != "-" && allergenInput != "") allergensInputList.Add(allergenInput);
            } while (allergenInput != "-");

            DecorateText("\nAz Ön számára javasolt ételek:", true, false, false);

            foreach (Meal meal in meals) {
                if (!F5_IsAllergicMeal(meal, allergensInputList)) {
                    string allergens = meal.Allergens.Length >= 1 ? string.Join(",", meal.Allergens) : "nincs";
                    Console.WriteLine($"{meal.Name} - allergének: {allergens}");
                }
            }

            // 8. feladat
            Console.WriteLine();
            DecorateText("8. feladat");

            List<string> categories = F7_GetCartegories(meals);
            List<string> mushroomAllergen = new List<string>() { "gomba" };

            foreach (string category in categories) {
                int index = 0;
                int lowestEnergy = int.MaxValue;

                for (int i = 0; i < meals.Count; i++) {
                    if (meals[i].Cathegory == category && !F5_IsAllergicMeal(meals[i], mushroomAllergen) && lowestEnergy > F2_GetFullEnergy(meals[i])) {
                        lowestEnergy = F2_GetFullEnergy(meals[i]);
                        index = i;
                    }
                }

                DecorateText(category, true, true, false, false);
                Console.WriteLine($": {meals[index].Name} ({meals[index].Mass} gramm), {lowestEnergy} kcal - {meals[index].Price} Ft.");
            }

            // 9. feladat
            Console.WriteLine();
            DecorateText("9. feladat");
            int[,] indexes = new int[5, 4];
            int days = 5;
            int dayNumber = 11;
            // teszteltem nem magyar alapbeállítású gépen is, azért tettem bele a CultureInfot
            CultureInfo cultureInfo = new CultureInfo("hu-HU");

            for (int i = 0; i < days; i++) {
                int cheapestSoup = int.MaxValue;
                int cheapestMainMeal = int.MaxValue;
                int cheapestDessert = int.MaxValue;
                int indexSoup = 0;
                int indexMainMeal = 0;
                int indexDessert = 0;

                string mainMealRandom = GetRandomMealCathegory();

                for (int j = 0; j < meals.Count; j++) {

                    if (meals[j].OrderDate.Day == dayNumber) {
                        if (meals[j].Cathegory == "leves" && cheapestSoup > meals[j].Price) {
                            cheapestSoup = meals[j].Price;
                            indexSoup = j;
                        }

                        if (meals[j].Cathegory == mainMealRandom && cheapestMainMeal > meals[j].Price) {
                            cheapestMainMeal = meals[j].Price;
                            indexMainMeal = j;
                        }

                        if (meals[j].Cathegory == "desszert" && cheapestDessert > meals[j].Price) {
                            cheapestDessert = meals[j].Price;
                            indexDessert = j;
                        }
                    }
                }

                indexes[i, 0] = indexSoup;
                indexes[i, 1] = indexMainMeal;
                indexes[i, 2] = indexDessert;
                indexes[i, 3] = cheapestSoup + cheapestMainMeal + cheapestDessert;

                dayNumber++;
            }

            for (int i = 0; i < indexes.GetLength(0); i++) {
                DecorateText(meals[indexes[i, 0]].OrderDate.ToString("dddd"), true, false, true, false);
                DecorateText($" ({indexes[i, 3]} Ft)", true, false, false, true);
                DecorateText(meals[indexes[i, 0]].Cathegory + ": ", true, false, false, false);
                Console.WriteLine($"{meals[indexes[i, 0]].Name} - {meals[indexes[i, 0]].Price} Ft");
                DecorateText(meals[indexes[i, 1]].Cathegory + ": ", true, false, false, false);
                Console.WriteLine($"{meals[indexes[i, 1]].Name} - {meals[indexes[i, 1]].Price} Ft");
                DecorateText(meals[indexes[i, 2]].Cathegory + ": ", true, false, false, false);
                Console.WriteLine($"{meals[indexes[i, 2]].Name} - {meals[indexes[i, 2]].Price} Ft");
                Console.WriteLine();
            }

            Console.ReadLine();
        }

        private static string GetRandomMealCathegory() {
            Random rnd = new Random();
            int choise = rnd.Next(0, 3);

            switch (choise) {
                default:
                case 0:
                    return "rántott";
                case 1:
                    return "főzelék";
                case 2:
                    return "saláta";
            }
        }

        static int F2_GetFullEnergy(Meal meal) {
            return (int)(meal.Mass / 100.0 * meal.Energy);
            //return meal.Mass * meal.Energy / 100;
        }

        static bool F5_IsAllergicMeal(Meal meal, List<string> allergens) {
            if (meal.Allergens.Length == 0) {
                return false;
            }
            else {
                for (int i = 0; i < meal.Allergens.Length; i++) {
                    foreach (string allergen in allergens) {
                        if (meal.Allergens[i] == allergen) return true;
                    }
                }

                return false;
            }
        }
        private static List<string> F7_GetCartegories(List<Meal> meals) {
            List<string> cathegories = new List<string>();

            foreach (Meal meal in meals) {
                if (!cathegories.Contains(meal.Cathegory)) {
                    cathegories.Add(meal.Cathegory);
                }
            }

            cathegories.Sort();

            return cathegories;
        }

        static string[] GetAllergens(string line) {
            if (line.Contains(',')) {
                return line.Split(',');
            }
            else if (line != "") {
                return new string[] { line };
            }

            return new string[0];
        }

        // nem kell, mert zárójellel () be lehet tenni a Ternary a {} közé
        static string GetPortion(bool isNormal) {
            return isNormal ? "normál" : "kicsi";
        }

        static void DecorateText(string text, bool bold = true, bool underline = true, bool red = true, bool nextLine = true) {
            string decoration = "";

            if (red)
                decoration += "\u001b[31m";
            if (bold)
                decoration += "\u001b[1m";
            if (underline)
                decoration += "\u001b[4m";

            string sufix = decoration.Length == 0 ? "" : "\u001b[0m";

            if (nextLine)
                Console.WriteLine($"{decoration}{text}{sufix}");
            else
                Console.Write($"{decoration}{text}{sufix}");
        }
    }
}
