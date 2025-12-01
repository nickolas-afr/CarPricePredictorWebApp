namespace CarPricePredictor.Web.Data;

public static class CarBrandData
{
    public static Dictionary<string, List<string>> BrandModels { get; } = new()
    {
        { "Audi", new List<string> { "A1", "A3", "A4", "A5", "A6", "A7", "A8", "Q2", "Q3", "Q5", "Q7", "Q8", "TT", "R8", "e-tron" } },
        { "BMW", new List<string> { "1 Series", "2 Series", "3 Series", "4 Series", "5 Series", "6 Series", "7 Series", "8 Series", "X1", "X2", "X3", "X4", "X5", "X6", "X7", "Z4", "i3", "i8" } },
        { "Mercedes-Benz", new List<string> { "A-Class", "B-Class", "C-Class", "CLA", "CLS", "E-Class", "S-Class", "GLA", "GLB", "GLC", "GLE", "GLS", "G-Class", "SL", "SLK", "AMG GT" } },
        { "Volkswagen", new List<string> { "Polo", "Golf", "Jetta", "Passat", "Arteon", "Tiguan", "Touareg", "T-Roc", "T-Cross", "ID.3", "ID.4", "Up!", "Beetle", "Scirocco" } },
        { "Opel", new List<string> { "Corsa", "Astra", "Insignia", "Mokka", "Crossland", "Grandland", "Zafira", "Meriva", "Combo" } },
        { "Ford", new List<string> { "Fiesta", "Focus", "Mondeo", "Mustang", "Kuga", "Puma", "EcoSport", "Explorer", "Edge", "Ranger", "Transit" } },
        { "Renault", new List<string> { "Clio", "Megane", "Scenic", "Kadjar", "Captur", "Koleos", "Talisman", "Zoe", "Twingo" } },
        { "Peugeot", new List<string> { "208", "308", "508", "2008", "3008", "5008", "Rifter", "Partner", "Expert" } },
        { "Citroen", new List<string> { "C1", "C3", "C4", "C5", "Berlingo", "Jumpy", "SpaceTourer" } },
        { "Skoda", new List<string> { "Fabia", "Octavia", "Superb", "Kodiaq", "Karoq", "Kamiq", "Scala", "Enyaq" } },
        { "SEAT", new List<string> { "Ibiza", "Leon", "Arona", "Ateca", "Tarraco", "Mii", "Exeo" } },
        { "Hyundai", new List<string> { "i10", "i20", "i30", "Elantra", "Tucson", "Santa Fe", "Kona", "Ioniq", "Nexo" } },
        { "Kia", new List<string> { "Picanto", "Rio", "Ceed", "Stinger", "Sportage", "Sorento", "Niro", "e-Niro", "Soul" } },
        { "Toyota", new List<string> { "Aygo", "Yaris", "Corolla", "Camry", "C-HR", "RAV4", "Highlander", "Land Cruiser", "Prius" } },
        { "Mazda", new List<string> { "2", "3", "6", "CX-3", "CX-5", "CX-30", "CX-60", "MX-5" } },
        { "Honda", new List<string> { "Jazz", "Civic", "Accord", "CR-V", "HR-V", "e" } },
        { "Nissan", new List<string> { "Micra", "Juke", "Qashqai", "X-Trail", "Leaf", "370Z", "GT-R" } },
        { "Volvo", new List<string> { "V40", "V60", "V90", "S60", "S90", "XC40", "XC60", "XC90" } },
        { "Fiat", new List<string> { "500", "Panda", "Tipo", "500X", "500L", "Ducato" } },
        { "Alfa Romeo", new List<string> { "Giulietta", "Giulia", "Stelvio", "Tonale" } },
        { "Mini", new List<string> { "Cooper", "Clubman", "Countryman", "Paceman", "Roadster" } },
        { "Porsche", new List<string> { "911", "718", "Cayenne", "Macan", "Panamera", "Taycan" } },
        { "Tesla", new List<string> { "Model 3", "Model S", "Model X", "Model Y" } },
        { "Dacia", new List<string> { "Sandero", "Logan", "Duster", "Spring" } },
        { "Suzuki", new List<string> { "Swift", "Vitara", "S-Cross", "Ignis", "Jimny" } },
        { "Mitsubishi", new List<string> { "Space Star", "ASX", "Eclipse Cross", "Outlander" } },
        { "Jeep", new List<string> { "Renegade", "Compass", "Cherokee", "Grand Cherokee", "Wrangler" } },
        { "Land Rover", new List<string> { "Defender", "Discovery", "Discovery Sport", "Range Rover", "Range Rover Sport", "Range Rover Evoque" } }
    };
}
