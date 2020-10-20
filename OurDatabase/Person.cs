namespace OurDatabase
{
    public class Person
    {
        public Person(int id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return $"{{Id: {Id} ,Name: {Name} Age: {Age}}}";
        }
    }
}