namespace CarPricePredictor.Web.Data;

public static class CarModelMetadata
{
    public record ModelInfo(string BodyType, int TypicalSeats);

    public static readonly Dictionary<string, Dictionary<string, ModelInfo>> Data = new()
    {
        ["Audi"] = new()
        {
            ["A1"] = new("Hatchback", 5),
            ["A3"] = new("Hatchback", 5),
            ["A4"] = new("Sedan/Combi", 5),
            ["A5"] = new("Coupe", 4),
            ["A6"] = new("Sedan/Combi", 5),
            ["A7"] = new("Sportback", 5),
            ["A8"] = new("Sedan", 5),
            ["Q2"] = new("SUV", 5),
            ["Q3"] = new("SUV", 5),
            ["Q5"] = new("SUV", 5),
            ["Q7"] = new("SUV", 7),
            ["Q8"] = new("SUV", 5),
            ["TT"] = new("Coupe", 4),
            ["R8"] = new("Coupe", 2),
            ["e-tron"] = new("SUV", 5),
        },
        ["BMW"] = new()
        {
            ["1 Series"] = new("Hatchback", 5),
            ["2 Series"] = new("Coupe", 4),
            ["3 Series"] = new("Sedan", 5),
            ["4 Series"] = new("Coupe", 4),
            ["5 Series"] = new("Sedan", 5),
            ["6 Series"] = new("Coupe", 4),
            ["7 Series"] = new("Sedan", 5),
            ["8 Series"] = new("Coupe", 4),
            ["X1"] = new("SUV", 5),
            ["X2"] = new("SUV", 5),
            ["X3"] = new("SUV", 5),
            ["X4"] = new("SUV", 5),
            ["X5"] = new("SUV", 5),
            ["X6"] = new("SUV", 5),
            ["X7"] = new("SUV", 7),
            ["Z4"] = new("Roadster", 2),
            ["i3"] = new("Hatchback", 4),
            ["i8"] = new("Coupe", 2),
        },
        ["Mercedes-Benz"] = new()
        {
            ["A-Class"] = new("Hatchback", 5),
            ["B-Class"] = new("MPV", 5),
            ["C-Class"] = new("Sedan", 5),
            ["CLA"] = new("Coupe", 5),
            ["CLS"] = new("Coupe", 5),
            ["E-Class"] = new("Sedan", 5),
            ["S-Class"] = new("Sedan", 5),
            ["GLA"] = new("SUV", 5),
            ["GLB"] = new("SUV", 7),
            ["GLC"] = new("SUV", 5),
            ["GLE"] = new("SUV", 5),
            ["GLS"] = new("SUV", 7),
            ["G-Class"] = new("SUV", 5),
            ["SL"] = new("Roadster", 2),
            ["SLK"] = new("Roadster", 2),
            ["AMG GT"] = new("Coupe", 2),
        },
        ["Volkswagen"] = new()
        {
            ["Polo"] = new("Hatchback", 5),
            ["Golf"] = new("Hatchback", 5),
            ["Jetta"] = new("Sedan", 5),
            ["Passat"] = new("Sedan/Combi", 5),
            ["Arteon"] = new("Sedan", 5),
            ["Tiguan"] = new("SUV", 5),
            ["Touareg"] = new("SUV", 5),
            ["T-Roc"] = new("SUV", 5),
            ["T-Cross"] = new("SUV", 5),
            ["ID.3"] = new("Hatchback", 5),
            ["ID.4"] = new("SUV", 5),
            ["Up!"] = new("Hatchback", 4),
            ["Beetle"] = new("Coupe", 4),
            ["Scirocco"] = new("Coupe", 4),
        },
        ["Opel"] = new()
        {
            ["Corsa"] = new("Hatchback", 5),
            ["Astra"] = new("Hatchback", 5),
            ["Insignia"] = new("Sedan/Combi", 5),
            ["Mokka"] = new("SUV", 5),
            ["Crossland"] = new("SUV", 5),
            ["Grandland"] = new("SUV", 5),
            ["Zafira"] = new("MPV", 7),
            ["Meriva"] = new("MPV", 5),
            ["Combo"] = new("MPV", 5),
        },
        ["Ford"] = new()
        {
            ["Fiesta"] = new("Hatchback", 5),
            ["Focus"] = new("Hatchback", 5),
            ["Mondeo"] = new("Sedan/Combi", 5),
            ["Mustang"] = new("Coupe", 4),
            ["Kuga"] = new("SUV", 5),
            ["Puma"] = new("SUV", 5),
            ["EcoSport"] = new("SUV", 5),
            ["Explorer"] = new("SUV", 7),
            ["Edge"] = new("SUV", 5),
            ["Ranger"] = new("Pickup", 5),
            ["Transit"] = new("Van", 3),
        },
        ["Renault"] = new()
        {
            ["Clio"] = new("Hatchback", 5),
            ["Megane"] = new("Hatchback", 5),
            ["Scenic"] = new("MPV", 5),
            ["Kadjar"] = new("SUV", 5),
            ["Captur"] = new("SUV", 5),
            ["Koleos"] = new("SUV", 5),
            ["Talisman"] = new("Sedan", 5),
            ["Zoe"] = new("Hatchback", 5),
            ["Twingo"] = new("Hatchback", 4),
        },
        ["Peugeot"] = new()
        {
            ["208"] = new("Hatchback", 5),
            ["308"] = new("Hatchback", 5),
            ["508"] = new("Sedan/Combi", 5),
            ["2008"] = new("SUV", 5),
            ["3008"] = new("SUV", 5),
            ["5008"] = new("SUV", 7),
            ["Rifter"] = new("MPV", 5),
            ["Partner"] = new("Van", 3),
            ["Expert"] = new("Van", 3),
        },
        ["Citroen"] = new()
        {
            ["C1"] = new("Hatchback", 4),
            ["C3"] = new("Hatchback", 5),
            ["C4"] = new("Hatchback", 5),
            ["C5"] = new("Sedan/Combi", 5),
            ["Berlingo"] = new("MPV", 5),
            ["Jumpy"] = new("Van", 3),
            ["SpaceTourer"] = new("MPV", 9),
        },
        ["Skoda"] = new()
        {
            ["Fabia"] = new("Hatchback", 5),
            ["Octavia"] = new("Sedan/Combi", 5),
            ["Superb"] = new("Sedan/Combi", 5),
            ["Kodiaq"] = new("SUV", 7),
            ["Karoq"] = new("SUV", 5),
            ["Kamiq"] = new("SUV", 5),
            ["Scala"] = new("Hatchback", 5),
            ["Enyaq"] = new("SUV", 5),
        },
        ["SEAT"] = new()
        {
            ["Ibiza"] = new("Hatchback", 5),
            ["Leon"] = new("Hatchback", 5),
            ["Arona"] = new("SUV", 5),
            ["Ateca"] = new("SUV", 5),
            ["Tarraco"] = new("SUV", 7),
            ["Mii"] = new("Hatchback", 4),
            ["Exeo"] = new("Sedan", 5),
        },
        ["Hyundai"] = new()
        {
            ["i10"] = new("Hatchback", 4),
            ["i20"] = new("Hatchback", 5),
            ["i30"] = new("Hatchback", 5),
            ["Elantra"] = new("Sedan", 5),
            ["Tucson"] = new("SUV", 5),
            ["Santa Fe"] = new("SUV", 7),
            ["Kona"] = new("SUV", 5),
            ["Ioniq"] = new("Hatchback", 5),
            ["Nexo"] = new("SUV", 5),
        },
        ["Kia"] = new()
        {
            ["Picanto"] = new("Hatchback", 4),
            ["Rio"] = new("Hatchback", 5),
            ["Ceed"] = new("Hatchback", 5),
            ["Stinger"] = new("Sedan", 5),
            ["Sportage"] = new("SUV", 5),
            ["Sorento"] = new("SUV", 7),
            ["Niro"] = new("SUV", 5),
            ["e-Niro"] = new("SUV", 5),
            ["Soul"] = new("Hatchback", 5),
        },
        ["Toyota"] = new()
        {
            ["Aygo"] = new("Hatchback", 4),
            ["Yaris"] = new("Hatchback", 5),
            ["Corolla"] = new("Hatchback", 5),
            ["Camry"] = new("Sedan", 5),
            ["C-HR"] = new("SUV", 5),
            ["RAV4"] = new("SUV", 5),
            ["Highlander"] = new("SUV", 7),
            ["Land Cruiser"] = new("SUV", 7),
            ["Prius"] = new("Hatchback", 5),
        },
        ["Mazda"] = new()
        {
            ["2"] = new("Hatchback", 5),
            ["3"] = new("Hatchback", 5),
            ["6"] = new("Sedan/Combi", 5),
            ["CX-3"] = new("SUV", 5),
            ["CX-5"] = new("SUV", 5),
            ["CX-30"] = new("SUV", 5),
            ["CX-60"] = new("SUV", 5),
            ["MX-5"] = new("Roadster", 2),
        },
        ["Honda"] = new()
        {
            ["Jazz"] = new("Hatchback", 5),
            ["Civic"] = new("Hatchback", 5),
            ["Accord"] = new("Sedan", 5),
            ["CR-V"] = new("SUV", 5),
            ["HR-V"] = new("SUV", 5),
            ["e"] = new("Hatchback", 4),
        },
        ["Nissan"] = new()
        {
            ["Micra"] = new("Hatchback", 5),
            ["Juke"] = new("SUV", 5),
            ["Qashqai"] = new("SUV", 5),
            ["X-Trail"] = new("SUV", 7),
            ["Leaf"] = new("Hatchback", 5),
            ["370Z"] = new("Coupe", 2),
            ["GT-R"] = new("Coupe", 4),
        },
        ["Volvo"] = new()
        {
            ["V40"] = new("Hatchback", 5),
            ["V60"] = new("Combi", 5),
            ["V90"] = new("Combi", 5),
            ["S60"] = new("Sedan", 5),
            ["S90"] = new("Sedan", 5),
            ["XC40"] = new("SUV", 5),
            ["XC60"] = new("SUV", 5),
            ["XC90"] = new("SUV", 7),
        },
        ["Fiat"] = new()
        {
            ["500"] = new("Hatchback", 4),
            ["Panda"] = new("Hatchback", 4),
            ["Tipo"] = new("Hatchback", 5),
            ["500X"] = new("SUV", 5),
            ["500L"] = new("MPV", 5),
            ["Ducato"] = new("Van", 3),
        },
        ["Alfa Romeo"] = new()
        {
            ["Giulietta"] = new("Hatchback", 5),
            ["Giulia"] = new("Sedan", 5),
            ["Stelvio"] = new("SUV", 5),
            ["Tonale"] = new("SUV", 5),
        },
        ["Mini"] = new()
        {
            ["Cooper"] = new("Hatchback", 4),
            ["Clubman"] = new("Combi", 5),
            ["Countryman"] = new("SUV", 5),
            ["Paceman"] = new("Coupe", 4),
            ["Roadster"] = new("Roadster", 2),
        },
        ["Porsche"] = new()
        {
            ["911"] = new("Coupe", 4),
            ["718"] = new("Coupe", 2),
            ["Cayenne"] = new("SUV", 5),
            ["Macan"] = new("SUV", 5),
            ["Panamera"] = new("Sedan", 4),
            ["Taycan"] = new("Sedan", 4),
        },
        ["Tesla"] = new()
        {
            ["Model 3"] = new("Sedan", 5),
            ["Model S"] = new("Sedan", 5),
            ["Model X"] = new("SUV", 7),
            ["Model Y"] = new("SUV", 5),
        },
        ["Dacia"] = new()
        {
            ["Sandero"] = new("Hatchback", 5),
            ["Logan"] = new("Sedan", 5),
            ["Duster"] = new("SUV", 5),
            ["Spring"] = new("Hatchback", 4),
        },
        ["Suzuki"] = new()
        {
            ["Swift"] = new("Hatchback", 5),
            ["Vitara"] = new("SUV", 5),
            ["S-Cross"] = new("SUV", 5),
            ["Ignis"] = new("Hatchback", 5),
            ["Jimny"] = new("SUV", 4),
        },
        ["Mitsubishi"] = new()
        {
            ["Space Star"] = new("Hatchback", 5),
            ["ASX"] = new("SUV", 5),
            ["Eclipse Cross"] = new("SUV", 5),
            ["Outlander"] = new("SUV", 7),
        },
        ["Jeep"] = new()
        {
            ["Renegade"] = new("SUV", 5),
            ["Compass"] = new("SUV", 5),
            ["Cherokee"] = new("SUV", 5),
            ["Grand Cherokee"] = new("SUV", 5),
            ["Wrangler"] = new("SUV", 5),
        },
        ["Land Rover"] = new()
        {
            ["Defender"] = new("SUV", 5),
            ["Discovery"] = new("SUV", 7),
            ["Discovery Sport"] = new("SUV", 7),
            ["Range Rover"] = new("SUV", 5),
            ["Range Rover Sport"] = new("SUV", 5),
            ["Range Rover Evoque"] = new("SUV", 5),
        },
    };

    public static ModelInfo? Get(string make, string model)
        => Data.GetValueOrDefault(make)?.GetValueOrDefault(model);
}
