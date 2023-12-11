using System.Data;
using Microsoft.Data.SqlClient;

using MySqlConnector;

public class DbConnect(IConfiguration configuration)
{
    private readonly string? connectionString = configuration.GetConnectionString("DefaultDBConnectionString");
    private DataTable? dataTable;

    public MySqlConnection CreateConnection()
    {
        return new MySqlConnection(connectionString);
    }

    public Part[] GetTreeData()
    {
        List<Part> parts = new List<Part>(0);
        List<BomRecord> bomRecords = new List<BomRecord>(0);

        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var bomsCommand = new MySqlCommand("SELECT * FROM Boms", connection))
            {
                using (var bomsReader = bomsCommand.ExecuteReader())
                {
                    while (bomsReader.Read())
                    {
                        bomRecords.Add(new BomRecord((string)bomsReader["PARENT_NAME"], ushort.Parse((string)bomsReader["quantity"]), (string)bomsReader["COMPONENT_NAME"]));
                    }
                    bomsReader.Close();
                }
            }
            
            foreach (var bomRecord in bomRecords)
            {
                using (var partsCommand = new MySqlCommand("select * from parts where name = @componentName", connection)) 
                {
                    partsCommand.Parameters.AddWithValue("@componentName", bomRecord.componentName);

                    using (var partsReader = partsCommand.ExecuteReader()) {
                        while(partsReader.Read())
                        {
                            Part part = new Part(
                                (int)partsReader["id"], 
                                bomRecord.parentName, 
                                bomRecord.quantity, 
                                bomRecord.componentName, 
                                (string)partsReader["type"], 
                                (string)partsReader["item"], 
                                (string)partsReader["PART_NUMBER"], 
                                (string)partsReader["title"], 
                                (string)partsReader["material"], 
                                new List<Part>()
                            );
                            parts.Add(part);
                        }
                        partsReader.Close();
                    }
                }
            }
            connection.Close();
        }
        
        return BuildPartTree(parts);
    }
    private Part[] BuildPartTree(List<Part> parts)
    {
        if (parts.Count == 0) return parts.ToArray();
        List<Part> rootParts = parts.Where(part => part.parentName == "").ToList();
        foreach (var rootPart in rootParts)
        {
            NestParts(parts, rootPart);
        }
        return rootParts.ToArray();
    }
    private void NestParts(List<Part> parts, Part parent)
    {
        List<Part> children = parts.Where(part => part.parentName == parent.componentName).ToList();

        foreach (var child in children)
        {
            parent.children.Add(child);
            NestParts(parts, child);
        }
    }

}
