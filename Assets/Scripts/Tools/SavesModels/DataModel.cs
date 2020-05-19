using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;







public class EnvironementDataModel 
{
  public float time { get; set; }
  public List<float> sunPowerhistory { get; set; }

  public void GetValues()
  {
      time = Environement.time;
      sunPowerhistory = Environement.sunPowerhistory;
  }

  public void SetValues()
  {
      Environement.time = time;
      Environement.sunPowerhistory = sunPowerhistory;
  }

  public static EnvironementDataModel Deserialize(string json)
  {
      return JsonConvert.DeserializeObject<EnvironementDataModel>(json);
  }
}








public class WorldDataModel
{
    public int size { get; set; }
    public List<Chunk> chunks { get; set; }
    public List<Tree.SaveStruct> trees { get; set; } 
    
    public Dictionary<Liquid.Type, List<((int,int),int)>> liquids { get; set; }
    
    public Vector2 worldSpawn { get; set; }
    public void GetValues()
    {
        size = World.size;
        chunks = World.chunks;
        trees = new List<Tree.SaveStruct>();
        foreach (var tree in World.trees)
        {
            trees.Add(tree.GetSaveStruct());
        }
        
        liquids = new Dictionary<Liquid.Type, List<((int, int), int)>>();
        foreach (var lq in Liquid.list)
        {
            List<((int, int), int)> liquid = new List<((int, int), int)>();
            foreach (var coord in lq.Value.listLiquid)
            {
                liquid.Add(((coord.Item1, coord.Item2), Liquid.list[lq.Key].map[coord.Item1, coord.Item2]));
            }
            liquids.Add(lq.Key,liquid);
        }

        worldSpawn = World.spawn;
    }

    public void SetValues()
    {
        World.size = size;
        World.chunks = chunks;
        foreach (var tree in World.trees)
        {
            tree.QueueFree();
        }
        World.trees.Clear();
        foreach (var saveTree in trees)
        {
            Tree t = (Tree)GD.Load<PackedScene>("res://Assets/Objects/Autres/Tree/Tree.tscn").Instance();
            World.trees.Add(t);
            t.treeNumber = saveTree.treeNumber;
            t.treeSize = saveTree.treeSize;
            t.Place((int)saveTree.location.x, (int)saveTree.location.y);
        }
        
        Liquid.Init();
        foreach (var lq in Liquid.list)
        {
            int[,] map = new int[World.size * Chunk.size + 1, Chunk.height];
            foreach (var values in liquids[lq.Key])
            {
                Liquid.list[lq.Key].listLiquid
                    .Add(new Tuple<int, int>(values.Item1.Item1, values.Item1.Item2));
                map[values.Item1.Item1, values.Item1.Item2] = values.Item2;
            }
            Liquid.list[lq.Key].map = map;
        }
        World.spawn = worldSpawn;
        World.GrassGenerate();
    }

    
    
    public static WorldDataModel Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<WorldDataModel>(json);
    }
}



public class PlayerDataModel
{
    public float healthMax;
    public float health;
    public float oxygeneMax;
    public float oxygene;
    public float energyMax;
    public float energy;

    public Vector2 playerPosition;
    
    public Dictionary<Usable.Type, int> storageUsables;
    public Dictionary<Item.Type, int> storageItems;
    public Dictionary<Liquid.Type, float> storageLiquids;
    public Dictionary<Building.Type, int> storageBuilding;
    
    public void GetValues()
    {

        healthMax = Player.healthMax;
        health = Player.health;
        oxygeneMax = Player.oxygeneMax;
        oxygene = Player.oxygene;
        energyMax = Player.energyMax;
        energy = Player.energy;
        
        playerPosition = new Vector2(PlayerMouvements.GetX(), PlayerMouvements.GetY());
    
        storageUsables = Player.inventoryUsables.stokage;
        storageItems = Player.inventoryItems.stokage;
        storageLiquids = Player.inventoryLiquids.stokage;
        storageBuilding = Player.inventoryBuildings.storage;
    }
    
    public void SetValues()
    {
        Player.healthMax = healthMax;
        Player.health = health;
        Player.oxygeneMax = oxygeneMax;
        Player.oxygene = oxygene;
        Player.energyMax = energyMax;
        Player.energy = energy;

        PlayerMouvements.initialPosition = playerPosition;
        
        Player.inventoryUsables.stokage = storageUsables;
        Player.inventoryItems.stokage = storageItems;
        Player.inventoryLiquids.stokage = storageLiquids;
        Player.inventoryBuildings.storage = storageBuilding;
    }
    
    public static PlayerDataModel Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<PlayerDataModel>(json);
    }
}






public class SpaceshipDataModel
{

    public Vector2 structurePosition { get; set; }
    public int composite { get; set; }
    public float fuel { get; set; }
    public float energy { get; set; }
    public void GetValues()
    {
        structurePosition = Structure.structurePos;
        composite = SpaceShip.composite;
        fuel = SpaceShip.fuel;
        energy = SpaceShip.energy;
    }
    public void SetValues()
    {
        Structure.structurePos = structurePosition;
        SpaceShip.composite = composite;
        SpaceShip.fuel = fuel;
        SpaceShip.energy = energy;
    }

    public static SpaceshipDataModel Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<SpaceshipDataModel>(json);
    }
}


public class BuildingsDataModel
{

    public int nbSolarPanels;
    public List<SolarPanel.SaveStruct> solarPanels;
    
    public int nbStorages;
    public List<Storage.SaveStruct> storages;
    
    public int nbPrinter3d;
    public List<Printer3D.SaveStruct> printers3d;
    
    public int nbCompactors;
    public List<Compactor.SaveStruct> compactors;
    
    public int nbInfirmary;
    public List<Infirmary.SaveStruct> infirmaries;
    
    public int nbO2Generator;
    public List<O2Generator.SaveStruct> O2Generators;
    
    public int nbPetrolGenerator;
    public List<PetrolGenerator.SaveStruct> petrolGenerators;



    //public Dictionary<(Building.Type, int), List<(Building.Type, int)>> buildingsLinks;
    public Dictionary<Building.Type, Dictionary<int, List<(Building.Type, int)>>> buildingsLinks;


    public void GetValues()
    {
        solarPanels = new List<SolarPanel.SaveStruct>();
        nbSolarPanels = SolarPanel.nbSolarPanel;
        foreach (var solarpanel in Building.GetBuildingTypeList<SolarPanel>())
        {
            solarPanels.Add(solarpanel.GetSaveStruct());
        }
        
        storages = new List<Storage.SaveStruct>();
        nbStorages = Storage.nbStorage;
        foreach (var storage in Building.GetBuildingTypeList<Storage>())
        {
            storages.Add(storage.GetSaveStruct());
        }
        
        printers3d = new List<Printer3D.SaveStruct>();
        nbPrinter3d = Printer3D.nbPrinter3D;
        foreach (var printer in Building.GetBuildingTypeList<Printer3D>())
        {
            printers3d.Add(printer.GetSaveStruct());
        }
        
        compactors = new List<Compactor.SaveStruct>();
        nbCompactors = Compactor.nbCompactor;
        foreach (var compactor in Building.GetBuildingTypeList<Compactor>())
        {
            compactors.Add(compactor.GetSaveStruct());
        }
        
        infirmaries = new List<Infirmary.SaveStruct>();
        nbInfirmary = Infirmary.nbInfirmary;
        foreach (var infirmary in Building.GetBuildingTypeList<Infirmary>())
        {
            infirmaries.Add(infirmary.GetSaveStruct());
        }
        
        O2Generators = new List<O2Generator.SaveStruct>();
        nbO2Generator = O2Generator.nbO2Generator;
        foreach (var o2generator in Building.GetBuildingTypeList<O2Generator>())
        {
            O2Generators.Add(o2generator.GetSaveStruct());
        }
        
        petrolGenerators = new List<PetrolGenerator.SaveStruct>();
        nbPetrolGenerator = PetrolGenerator.nbPetrolGenerator;
        foreach (var petrolgenerator in Building.GetBuildingTypeList<PetrolGenerator>())
        {
            petrolGenerators.Add(petrolgenerator.GetSaveStruct());
        }
        
        
        buildingsLinks = new Dictionary<Building.Type, Dictionary<int, List<(Building.Type, int)>>>();
        foreach (var suit in Enum.GetValues(typeof(Building.Type)))
        {
            Building.Type t = (Building.Type) suit;
            buildingsLinks.Add(t, new Dictionary<int, List<(Building.Type, int)>>());
            List<Building> buildings = Building.GetBuildingTypeList(t);
            foreach (var b in buildings)
            {
                buildingsLinks[t].Add(b.id, new List<(Building.Type, int)>());
                foreach (var bb in b.linkedBuildings)
                {
                    buildingsLinks[t][b.id].Add((bb.type, bb.id));
                }
            }
        }
    }
    public void SetValues()
    {
        foreach (var c in World.chunks)
        {
            World.placedBuildingByChunk.Add(c, new List<Building>());
        }

        SolarPanel.nbSolarPanel = nbSolarPanels;
        foreach (var saveSolarPanel in solarPanels)
        {
            SolarPanel sp = (SolarPanel)Building.prefabs[Building.Type.SolarPanel].Instance();
            sp.type = saveSolarPanel.buildingSave.type;
            sp.Place(saveSolarPanel.buildingSave.location);
            sp.id = saveSolarPanel.buildingSave.id;
            sp.healthMax = saveSolarPanel.buildingSave.healthMax;
            sp.health = saveSolarPanel.buildingSave.health;
            sp.energyMax = saveSolarPanel.buildingSave.energyMax;
            sp.energy = saveSolarPanel.buildingSave.energy;
            sp.isLinked = saveSolarPanel.buildingSave.isLinked;
        }
        
        Storage.nbStorage = nbStorages;
        foreach (var saveStorage in storages)
        {
            Storage sp = (Storage)Building.prefabs[Building.Type.Storage].Instance();
            sp.type = saveStorage.buildingSave.type;
            sp.Place(saveStorage.buildingSave.location);
            sp.id = saveStorage.buildingSave.id;
            sp.healthMax = saveStorage.buildingSave.healthMax;
            sp.health = saveStorage.buildingSave.health;
            sp.energyMax = saveStorage.buildingSave.energyMax;
            sp.energy = saveStorage.buildingSave.energy;
            sp.isLinked = saveStorage.buildingSave.isLinked;
        }
        
        Printer3D.nbPrinter3D = nbPrinter3d;
        foreach (var savePrinter in printers3d)
        {
            Printer3D sp = (Printer3D)Building.prefabs[Building.Type.Printer3D].Instance();
            sp.type = savePrinter.buildingSave.type;
            sp.isPrinting = savePrinter.isPrinting;
            sp.isInPause = savePrinter.isInPause;
            sp.isFinish = savePrinter.isFinish;
            sp.printingLevel = savePrinter.printingLevel;
            sp.printingType = savePrinter.printingType;
            sp.Place(savePrinter.buildingSave.location);
            sp.id = savePrinter.buildingSave.id;
            sp.healthMax = savePrinter.buildingSave.healthMax;
            sp.health = savePrinter.buildingSave.health;
            sp.energyMax = savePrinter.buildingSave.energyMax;
            sp.energy = savePrinter.buildingSave.energy;
            sp.isLinked = savePrinter.buildingSave.isLinked;
        }
        
        Compactor.nbCompactor = nbCompactors;
        foreach (var saveCompactor in compactors)
        {
            Compactor sp = (Compactor)Building.prefabs[Building.Type.Compactor].Instance();
            sp.type = saveCompactor.buildingSave.type;
            sp.Place(saveCompactor.buildingSave.location);
            sp.id = saveCompactor.buildingSave.id;
            sp.healthMax = saveCompactor.buildingSave.healthMax;
            sp.health = saveCompactor.buildingSave.health;
            sp.energyMax = saveCompactor.buildingSave.energyMax;
            sp.energy = saveCompactor.buildingSave.energy;
            sp.isLinked = saveCompactor.buildingSave.isLinked;
        }
        
        Infirmary.nbInfirmary = nbInfirmary;
        foreach (var saveInfirmary in infirmaries)
        {
            Infirmary sp = (Infirmary)Building.prefabs[Building.Type.Infirmary].Instance();
            sp.type = saveInfirmary.buildingSave.type;
            sp.togive = saveInfirmary.togive;
            sp.Place(saveInfirmary.buildingSave.location);
            sp.id = saveInfirmary.buildingSave.id;
            sp.healthMax = saveInfirmary.buildingSave.healthMax;
            sp.health = saveInfirmary.buildingSave.health;
            sp.energyMax = saveInfirmary.buildingSave.energyMax;
            sp.energy = saveInfirmary.buildingSave.energy;
            sp.isLinked = saveInfirmary.buildingSave.isLinked;
        }
        
        O2Generator.nbO2Generator = nbO2Generator;
        foreach (var saveO2generator in O2Generators)
        {
            O2Generator sp = (O2Generator)Building.prefabs[Building.Type.O2Generator].Instance();
            sp.type = saveO2generator.buildingSave.type;
            sp.togive = saveO2generator.togive;
            sp.o2 = saveO2generator.o2;
            sp.on = saveO2generator.@on;
            sp.Place(saveO2generator.buildingSave.location);
            sp.id = saveO2generator.buildingSave.id;
            sp.healthMax = saveO2generator.buildingSave.healthMax;
            sp.health = saveO2generator.buildingSave.health;
            sp.energyMax = saveO2generator.buildingSave.energyMax;
            sp.energy = saveO2generator.buildingSave.energy;
            sp.isLinked = saveO2generator.buildingSave.isLinked;
        }
        
        PetrolGenerator.nbPetrolGenerator = nbPetrolGenerator;
        foreach (var savePetrolGenerator in petrolGenerators)
        {
            PetrolGenerator sp = (PetrolGenerator)Building.prefabs[Building.Type.OilPump].Instance();
            sp.type = savePetrolGenerator.buildingSave.type;
            sp.togive = savePetrolGenerator.togive;
            sp.oil = savePetrolGenerator.oil;
            sp.on = savePetrolGenerator.@on;
            sp.Place(savePetrolGenerator.buildingSave.location);
            sp.id = savePetrolGenerator.buildingSave.id;
            sp.healthMax = savePetrolGenerator.buildingSave.healthMax;
            sp.health = savePetrolGenerator.buildingSave.health;
            sp.energyMax = savePetrolGenerator.buildingSave.energyMax;
            sp.energy = savePetrolGenerator.buildingSave.energy;
            sp.isLinked = savePetrolGenerator.buildingSave.isLinked;
        }



        foreach (var type in buildingsLinks)
        {
            foreach (var id in type.Value)
            {
                Building b = Building.GetBuildingById(type.Key, id.Key);
                if (b==null)
                    continue;
                b.linkedBuildings = new List<Building>();
                foreach (var building in id.Value)
                {
                    Building bb = Building.GetBuildingById(building.Item1, building.Item2);
                    if (bb==null)
                        continue;
                    b.linkedBuildings.Add(bb);
                }
            }
        }
    }

    public static BuildingsDataModel Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<BuildingsDataModel>(json);
    }
}