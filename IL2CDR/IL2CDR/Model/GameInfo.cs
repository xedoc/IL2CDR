using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public enum GameObjectClass
    {
        Other,
        Plane,
        Static,
        Tank,
        TrainLocomotive,
        TrainWagon,
        Truck,
        Turret,
    }

    public class GameObjectItem
    {
        public GameObjectItem(GameObjectClass classification, string purpose)
        {
            Classification = classification;
            Purpose = purpose;
        }
        public GameObjectClass Classification { get; set; }
        public string Purpose { get; set; }
    }

    public class GameInfo
    {
        public static Dictionary<string, GameObjectItem> ObjectsClassification = new Dictionary<string, GameObjectItem>()
        {            
            { "Bf 109 F-4", new  GameObjectItem(GameObjectClass.Plane, "Fighter aircraft")},
            { "Bf 109 G-2", new  GameObjectItem(GameObjectClass.Plane, "Fighter aircraft")},
            { "Fw 190 A-3", new  GameObjectItem(GameObjectClass.Plane, "Fighter aircraft")},
            { "He 111 H-6", new  GameObjectItem(GameObjectClass.Plane, "Bomber")},
            { "Il-2 mod.1942", new  GameObjectItem(GameObjectClass.Plane, "Attacker aircraft")},
            { "Ju 87 D-3", new  GameObjectItem(GameObjectClass.Plane, "Attacker aircraft")},
            { "La-5 ser.8", new  GameObjectItem(GameObjectClass.Plane, "Fighter aircraft")},
            { "LaGG-3 ser.29", new  GameObjectItem(GameObjectClass.Plane, "Fighter aircraft")},
            { "Pe-2 ser.87", new  GameObjectItem(GameObjectClass.Plane, "Bomber")},
            { "Yak-1 ser.69", new  GameObjectItem(GameObjectClass.Plane, "Fighter aircraft")},
            { "52-K", new  GameObjectItem(GameObjectClass.Static, "AAA")},
            { "61-K", new  GameObjectItem(GameObjectClass.Static, "AAA")},
            { "DShK", new  GameObjectItem(GameObjectClass.Static, "Firing point")},
            { "Flak 37", new  GameObjectItem(GameObjectClass.Static, "AAA")},
            { "Flak 38", new  GameObjectItem(GameObjectClass.Static, "AAA")},
            { "German Landlight", new  GameObjectItem(GameObjectClass.Static, "Landlight")},
            { "German Searchlight", new  GameObjectItem(GameObjectClass.Static, "Searchlight")},
            { "leFH 18", new  GameObjectItem(GameObjectClass.Static, "Artillery")},
            { "MG 34", new  GameObjectItem(GameObjectClass.Static, "Firing point")},
            { "MG 34 AA", new  GameObjectItem(GameObjectClass.Static, "AAA")},
            { "ML-20", new  GameObjectItem(GameObjectClass.Static, "Artillery")},
            { "Pak 38", new  GameObjectItem(GameObjectClass.Static, "Artillery")},
            { "Pak 40", new  GameObjectItem(GameObjectClass.Static, "Artillery")},
            { "Radio Beacon", new  GameObjectItem(GameObjectClass.Static, "Radio beacon")},
            { "Soviet Landlight", new  GameObjectItem(GameObjectClass.Static, "Landlight")},
            { "Soviet Searchlight", new  GameObjectItem(GameObjectClass.Static, "Searchlight")},
            { "ZIS-2", new  GameObjectItem(GameObjectClass.Static, "Artillery")},
            { "ZIS-3", new  GameObjectItem(GameObjectClass.Static, "Artillery")},
            { "BT-7M", new  GameObjectItem(GameObjectClass.Tank, "")},
            { "KV-1-42", new  GameObjectItem(GameObjectClass.Tank, "")},
            { "PzKpfw III Ausf.L", new  GameObjectItem(GameObjectClass.Tank, "")},
            { "PzKpfw IV Ausf.G", new  GameObjectItem(GameObjectClass.Tank, "")},
            { "Sd Kfz 10 Flak 38", new  GameObjectItem(GameObjectClass.Tank, "AAA")},
            { "Sd Kfz 251 Wurfrahmen 40", new  GameObjectItem(GameObjectClass.Tank, "Artillery")},
            { "StuG III Ausf.C-D", new  GameObjectItem(GameObjectClass.Tank, "")},
            { "StuG III Ausf.F", new  GameObjectItem(GameObjectClass.Tank, "")},
            { "T-34-76 STZ", new  GameObjectItem(GameObjectClass.Tank, "")},
            { "T-70", new  GameObjectItem(GameObjectClass.Tank, "")},
            { "Locomotive_E", new  GameObjectItem(GameObjectClass.TrainLocomotive, "")},
            { "Locomotive_G8", new  GameObjectItem(GameObjectClass.TrainLocomotive, "")},
            { "AA Platform 61K", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "AA Platform Flak 38", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "AA Platform M4", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "AA Platform MG 34", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "Wagon_BoxB", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "Wagon_BoxNB", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "Wagon_ET", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "Wagon_G8T", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "Wagon_GondolaB", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "Wagon_GondolaNB", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "Wagon_Pass", new  GameObjectItem(GameObjectClass.TrainWagon, "Passenger train wagon")},
            { "Wagon_Pass", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "Wagon_PassC", new  GameObjectItem(GameObjectClass.TrainWagon, "Passenger train wagon")},
            { "Wagon_PlatformB", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "Wagon_PlatformEmptyB", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "Wagon_PlatformEmptyNB", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "Wagon_PlatformNB", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "Wagon_TankB", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "Wagon_TankNB", new  GameObjectItem(GameObjectClass.TrainWagon, "")},
            { "BA-10M", new  GameObjectItem(GameObjectClass.Truck, "Transport")},
            { "BA-64", new  GameObjectItem(GameObjectClass.Truck, "Transport")},
            { "Ford G917", new  GameObjectItem(GameObjectClass.Truck, "Transport")},
            { "GAZ-AA", new  GameObjectItem(GameObjectClass.Truck, "Transport")},
            { "GAZ-AA M4", new  GameObjectItem(GameObjectClass.Truck, "AAA")},
            { "GAZ-M", new  GameObjectItem(GameObjectClass.Truck, "Transport")},
            { "Horch 830", new  GameObjectItem(GameObjectClass.Truck, "Transport")},
            { "Opel Blitz", new  GameObjectItem(GameObjectClass.Truck, "Transport")},
            { "Sd Kfz 251-1 Ausf.C", new  GameObjectItem(GameObjectClass.Truck, "Transport")},
            { "ZiS-5", new  GameObjectItem(GameObjectClass.Truck, "")},
            { "ZiS-6 BM-13", new  GameObjectItem(GameObjectClass.Truck, "Artillery")},
            { "Turret_He111H6_1", new  GameObjectItem(GameObjectClass.Turret, "")},
            { "Turret_He111H6_1M", new  GameObjectItem(GameObjectClass.Turret, "")},
            { "Turret_He111H6_2", new  GameObjectItem(GameObjectClass.Turret, "")},
            { "Turret_He111H6_3", new  GameObjectItem(GameObjectClass.Turret, "")},
            { "Turret_He111H6_3M", new  GameObjectItem(GameObjectClass.Turret, "")},
            { "Turret_He111H6_4", new  GameObjectItem(GameObjectClass.Turret, "")},
            { "Turret_Il2m42", new  GameObjectItem(GameObjectClass.Turret, "")},
            { "Turret_Ju87D3", new  GameObjectItem(GameObjectClass.Turret, "")},
        };

    }
}
