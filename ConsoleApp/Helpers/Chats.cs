namespace ConsoleApp.Helpers;

public class Chats
{
    public string ImageProcessingGroup { get; set; }
    public string DistributedGroup { get; set; }
    public string Nm { get; set; }
    public string Nmv3 { get; set; }
    public string P { get; set; }
    public SuperGroup Q4kRevisionGroup { get; set; }
    public string SEMcQsGroup { get; set; }
    public string Ggghhhggg { get; set; }
    public string Ghjkl { get; set; }
}

public class SuperGroup
{
    public string Id { get; set; }
    public List<Topic> Topics { get; set; }
}

public class Topic
{
    public string Name { get; set; }
    public int MessageId { get; set; }
}