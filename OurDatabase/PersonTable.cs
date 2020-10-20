#nullable enable
using System;

namespace OurDatabase
{
    public class PersonTable
    {
        private readonly GenericTree<int, Person> idTree;
        private readonly GenericTree<string, Person> nameTree;
        private readonly GenericTree<int, Person> ageTree;

        public PersonTable()
        {
            ageTree = new GenericTree<int, Person>($"{GetType().Name}-Age.db", (i, i1) => i-i1);
            idTree = new GenericTree<int, Person>($"{GetType().Name}-Id.db", (i, i1) => i-i1);
            nameTree = new GenericTree<string, Person>($"{GetType().Name}-Name.db", string.Compare);
        }

        public void AddData(Person person)
        {
            if (SearchById(person.Id) != null)
                throw new Exception($"Person with id {person.Id} already exists");
            
            idTree.AddData(person.Id, person);
            nameTree.AddData(person.Name, person);
            ageTree.AddData(person.Age, person);
        }

        public Person? SearchById(int id) => idTree.Search(id)?.Value;
        public Person? SearchByAge(int age) => ageTree.Search(age)?.Value;
        public Person? SearchByName(string name) => nameTree.Search(name)?.Value;

        public override string ToString()
        {
            return idTree.ToString();
        }
    }
}